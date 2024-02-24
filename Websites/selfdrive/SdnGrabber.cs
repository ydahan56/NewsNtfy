using HtmlAgilityPack;
using Sdk.Articles;
using Sdk.Base;
using selfdrive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace selfdrive
{
    public class SdnGrabber : IGrabber
    {
        private readonly HtmlDocument _document;
        private const string root_xpath = "//main[@id='main']//article";

        public SdnGrabber(HtmlDocument document)
        {
            this._document = document;
        }

        public IArticle GrabArticleFirstOrDefault()
        {
            try
            {
                return new ArticleBuilder(new SdnArticle())
                    .SetTitle(this.GetTitle())
                    .SetEntry(this.GetEntry())
                    .SetLink(this.GetHref())
                    .SetImgSrc(this.GetSrcAttr())
                    .Build();
            }
            catch (Exception ex)
            {
                return new ArticleBuilder(new ExceptionArticle())
                    .SetOptionalInfo(ex.ToString())
                    .Build();
            }
        }

        private string GetTitle()
        {
            var a = this._document.DocumentNode.SelectSingleNode($"{root_xpath}//h2[@class='entry-title']/a");
            var raw = a.GetDirectInnerText();

            return raw;
        }

        private string GetEntry()
        {
            var p = this._document.DocumentNode.SelectSingleNode($"{root_xpath}//div[@class='entry-excerpt']/p");
            var raw = p.GetDirectInnerText();

            return raw;
        }

        private string GetHref()
        {
            var a = this._document.DocumentNode.SelectSingleNode($"{root_xpath}//a[@class='entry-button ct-button']");
            var href = a.GetAttributeValue<string>("href", "");

            return href;
        }

        private string GetSrcAttr()
        {
            var a = this._document.DocumentNode.SelectSingleNode($"{root_xpath}/a/img");
            var src = a.GetAttributeValue<string>("src", "");

            return src;
        }
    }
}
