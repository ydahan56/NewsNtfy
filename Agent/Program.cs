using DotNetEnv;
using FluentScheduler;
using Hanssens.Net;
using NewsNotify.Registries;
using NewsNotify.Services;
using Sdk.Articles;
using Sdk.Base;
using System.Diagnostics;
using System.Reflection;

Console.Title = "NewsNtfy";

Env.Load();

var ntfy = new NtfySh();
var cache = new LocalStorage();

object _update_mutex = new object();

var minutes = Env.GetInt("minutes");

var Registries = GetJobs()
    .Select(x => new SiteRegistry(x, minutes))
    .ToArray();


JobManager.Initialize(Registries);

var instance = Process.GetCurrentProcess();

Console.WriteLine($"{Registries.Count()} Registries initialized, Press any key to exit... PID is {instance.Id}");
Console.ReadKey();

List<IJob?> GetJobs()
{
#if DEBUG
    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Websites");
#else
      var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Websites");
#endif
    var modules = Directory.EnumerateFiles(path, "*.Lib.dll", SearchOption.AllDirectories).ToList();

    var items = modules.Select(path =>
    {
        var assembly = Assembly.LoadFrom(path);
        var typeName = assembly.ExportedTypes
            .Single(x => x.Name.Equals("DllMain")).FullName;
        var instance = (IDllMain)assembly.CreateInstance(typeName);

        // init dll
        instance.Initialize(Path.GetDirectoryName(path));

        // set update callback
        instance.SetUpdateCallback(UpdateCallback);

        // return job for manager
        return (IJob)instance;
    }).ToList();

    return items!;
}



void UpdateCallback(IArticle article)
{
    lock (_update_mutex)
    {
        if (article is ExceptionArticle)
        {
            // print the exception
            Console.WriteLine(article.OptionalInfo);
            return;
        }

        var hashCode = article.LinkHashCode;

        if (cache.Exists(article.Key))
        {
            var cachedArticle = article.GetCached(cache);
            var cachedHashCode = cachedArticle.LinkHashCode;

            if (hashCode == cachedHashCode)
            {
                // same article, we exit
                Console.WriteLine($"{hashCode} same article for {article.Key}, exit.");
                return;
            }
        }

        Console.WriteLine($"{hashCode} new article for {article.Key}, publish.");

        // create the text to display
        var text = $"{article.Title}\n\n{article.Entry}";

        // if a headline is available
        if (!string.IsNullOrWhiteSpace(article.Headline))
        {
            // insert headline at the beginning
            text.Insert(0, article.Headline + "\n");
        }

        // publish update to user's
        ntfy.notifyArticleChanged(article.SiteName, text, article.ImgSrc, article.Link);

        // update article in local storage
        cache.Store(article.Key, article);
        cache.Persist();
    }
}