using Hanssens.Net;
using HtmlAgilityPack;
using Sdk.Articles;
using Sdk.Base;
using Sdk.ImageShack;
using YNet.Models;

namespace YNet
{
    public class DllMain : IDllMain
    {
        public override void Execute()
        {
            try
            {
                var pageDom = this.GetPageDom("https://www.ynet.co.il/home/0,7340,L-8,00.html");
                var article = new YNetGrabber(pageDom).GrabArticleFirstOrDefault();

                this.RaiseUpdateEvent(article);
            }
            catch (Exception ex)
            {
                this.RaiseUpdateEvent(new ExceptionArticle()
                {
                    OptionalInfo = ex.ToString()
                });
            }
        }

        public override void Initialize(string workingDirectory)
        {
           // throw new NotImplementedException();
        }
    }
}
