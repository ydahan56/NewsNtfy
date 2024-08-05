using HtmlAgilityPack;
using Sdk.Articles;
using Sdk.Contracts;
using System.Web;
using zhk.Models;

namespace zhk
{
    public class DllParser : IArticleParser
    {
        private readonly HtmlDocument _document;
        private const string root_xpath = "//div[@id='main-content']//article";

        public DllParser(HtmlDocument document)
        {
            this._document = document;
        }

        public IArticle FirstOrDefault()
        {
            try
            {
                return new ArticleBuilder(new ZhkArticle())
                    .SetTitle(this.GetTitle())
                    .SetEntry(this.GetEntry())
                    .SetLink(this.GetHref())
                    .SetImgSrc(HttpUtility.UrlPathEncode(this.GetSrcAttr()))
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
            var a = this._document.DocumentNode.SelectSingleNode($"{root_xpath}/h2/a");
            var raw = a.GetDirectInnerText();

            return raw;
        }

        private string GetEntry()
        {
            var p = this._document.DocumentNode.SelectSingleNode($"{root_xpath}/div[@class='entry']/p");
            var raw = p.GetDirectInnerText();

            return raw;
        }

        private string GetHref()
        {
            var a = this._document.DocumentNode.SelectSingleNode($"{root_xpath}/h2/a");
            var href = a.GetAttributeValue<string>("href", "");

            return href;
        }

        private string GetSrcAttr()
        {
            var a = this._document.DocumentNode.SelectSingleNode($"{root_xpath}//img");
            var src = a.GetAttributeValue<string>("src", "");

            return src;
        }
    }
}
