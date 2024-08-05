using Agent.Jobs;
using DotNetEnv;
using FluentScheduler;
using Hanssens.Net;
using ntfy;
using Sdk.Articles;
using Sdk.Contracts;
using System.Diagnostics;
using System.Reflection;
using System.Text;

Console.Title = "NewsNtfy";

Env.Load();

// must be here, prevent error "unassigned variable"
Mutex _invokeMutex = new Mutex(false);

var ntfy = new Client("https://ntfy.sh");
var cache = new LocalStorage();

var interval_minutes = Env.GetInt("interval_minutes");
var ntfy_topic = Env.GetString("ntfy_topic");

var Registries = GetMonitoringJobs();

JobManager.Initialize([.. Registries]);

var instance = Process.GetCurrentProcess();

var sb = new StringBuilder();
sb.AppendLine($"{Registries.Count} websites initialized, PID {instance.Id}");
sb.AppendLine("Press any key to exit...");

Console.WriteLine(sb.ToString());
Console.ReadKey();

List<MonitorJob> GetMonitoringJobs()
{
#if DEBUG
    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Websites");
#else
      var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Websites");
#endif
    var modules = Directory.EnumerateFiles(path, "*.Lib.dll", SearchOption.AllDirectories).ToList();

    var items = modules
        .Select(path =>
        {
            Console.WriteLine(path);
            var assembly = Assembly.LoadFrom(path);
            var typeName = assembly.ExportedTypes.Single(x => x.FullName.Contains("DllMain"));
            var instance = (IDllMain)assembly.CreateInstance(typeName.FullName);

            // init dll
            instance.Initialize(Path.GetDirectoryName(path));

            // set update callback
            instance.SetUpdateCallback(UpdateCallback);

            // return transformed monitor job
            return new MonitorJob(instance, interval_minutes);
        })
        .ToList();

    return items!;
}


void UpdateCallback(IArticle article)
{
    _invokeMutex.WaitOne();

    try
    {
        if (article is ExceptionArticle)
        {
            // print the exception
            Console.WriteLine(article.ErrorMessage);

            // exit and wait for next execution
            return;
        }

        // check if article already published
        if (article.IsArticlePublished(cache))
        {
            // print to console
            Console.WriteLine($"Article for {article.SiteName} already Published.");

            // return and wait for next execution
            return;
        }

        // prepare data
        var message = article.ToMessage();

        // execute publish
        var push = ntfy.Publish(ntfy_topic, message);
        push.Wait();

        // print to console
        Console.WriteLine($"Article update for {article.SiteName} Published..");

        // update article in local storage
        cache.Store(article.Key, article);
        cache.Persist();
    }
    finally
    {
        _invokeMutex.ReleaseMutex();
    }
}