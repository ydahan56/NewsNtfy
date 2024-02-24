﻿using Hanssens.Net;
using HtmlAgilityPack;
using Sdk.Articles;
using Sdk.Base;
using System.Web;
using Walla_.Models;

namespace Walla_
{
    public class WallaGrabber : IGrabber
    {
        private readonly HtmlDocument _document;

        public WallaGrabber(HtmlDocument document)
        {
            this._document = document;
        }

        public IArticle GrabArticleFirstOrDefault()
        {
            try
            {
                return new ArticleBuilder(new WallaArticle())
                    .SetHeadline(this.GetHeadline())
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

        private string GetHeadline()
        {
            // short headline below article
            var div = this._document.DocumentNode.SelectSingleNode("//div[@class='roof']");

            if (div != null)
            {
                return div.GetDirectInnerText();
            }

            // long headline below media div (outside article)
            var img = this._document.DocumentNode.SelectSingleNode("//picture[contains(@class, 'main-media')]/img");
            var raw = img.GetAttributeValue<string>("title", "");

            return raw;
        }

        private string GetTitle()
        {
            var h2 = this._document.DocumentNode.SelectSingleNode("//body//article/a/h2");
            var raw = h2.GetDirectInnerText();

            return raw;
        }

        private string GetEntry()
        {
            var p = this._document.DocumentNode.SelectSingleNode("//body//article/a/p");
            var raw = p.GetDirectInnerText();

            return raw;
        }

        private string GetHref()
        {
            var a = this._document.DocumentNode.SelectSingleNode("//body//article/a");
            var href = a.GetAttributeValue<string>("href", "");

            return href;
        }

        private string GetSrcAttr()
        {
            var imgNode = this._document.DocumentNode.SelectSingleNode("//picture//img");
            var imgSrc = imgNode.GetAttributeValue<string>("srcset", "");

            var breakIdx = imgSrc.IndexOf(" 969w");
            var srcset = imgSrc.Substring(0, breakIdx);

            return srcset;
        }
    }
}
