using Hanssens.Net;
using ntfy.Requests;
using Sdk.Articles;

namespace YNet.Models
{
    public class YNetArticle : IArticle
    {
        public YNetArticle()
        {
            this.Key = "YNET";
            this.SiteName = "ynet";
        }

        public override bool IsArticlePublished(ILocalStorage storage)
        {
            if (storage.Exists(this.Key))
            {
                var cachedArticle = storage.Get<YNetArticle>(this.Key);

                return (this.LinkHashCode == cachedArticle.LinkHashCode);
            }

            // we assume entry does not exist in the cache
            return false;
        }

        public override SendingMessage ToMessage()
        {
            var message = new SendingMessage()
            {
                Title = this.SiteName,
                Message = this.Title + "\n\n" + this.Entry,
                Attach = new Uri(this.ImgSrc),
                Click = new Uri(this.Link)
            };

            return message;
        }
    }
}
