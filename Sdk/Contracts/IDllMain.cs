using FluentScheduler;
using HtmlAgilityPack;
using Nito.AsyncEx;
using ScrapySharp.Network;
using Sdk.Articles;

namespace Sdk.Contracts
{
    public abstract class IDllMain : IJob
    {
        private Action<IArticle> _updateCallback;
        private readonly ScrapingBrowser _browser;

        protected IDllMain()
        {
            _browser = new ScrapingBrowser();
        }

        protected string GetPageHtmlContent(string pageUrl)
        {
            return AsyncContext.Run(async () => await _browser.AjaxDownloadStringAsync(new Uri(pageUrl)));
        }

        public abstract void Execute();

        public abstract void Initialize(string workingDirectory);

        public void SetUpdateCallback(Action<IArticle> updateCallback)
        {
            _updateCallback = updateCallback;
        }

        protected void RaiseUpdateEvent(IArticle article)
        {
            _updateCallback(article);
        }

        protected HtmlDocument GetPageDom(string pageUrl)
        {
            var content = GetPageHtmlContent(pageUrl);
            var document = new HtmlDocument();

            document.LoadHtml(content);

            return document;
        }
    }
}
