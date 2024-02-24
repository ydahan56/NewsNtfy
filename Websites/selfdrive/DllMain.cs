using HtmlAgilityPack;
using Sdk.Articles;
using Sdk.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace selfdrive
{
    public class DllMain : IDllMain
    {
        public override void Execute()
        {
            try
            {
                var pageDom = this.GetPageDom("https://selfdrivenews.com/");
                var article = new SdnGrabber(pageDom).GrabArticleFirstOrDefault();

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
