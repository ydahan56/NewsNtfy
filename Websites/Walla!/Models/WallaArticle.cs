using Hanssens.Net;
using ntfy.Requests;
using Sdk.Articles;

namespace Walla_.Models
{
    public class WallaArticle : IArticle
    {
        public WallaArticle()
        {
            this.Key = "WALLA";
            this.SiteName = "Walla! News";
        }

        public override bool IsArticlePublished(ILocalStorage storage)
        {
            if (storage.Exists(this.Key))
            {
                var cachedArticle = storage.Get<WallaArticle>(this.Key);

                return (this.LinkHashCode == cachedArticle.LinkHashCode);
            }

            // we assume entry does not exist in the cache
            return false;
        }

        public override SendingMessage ToMessage()
        {
            var message = new SendingMessage()
            {
                Title = this.Title,
                Message = this.Entry,
                Attach = new Uri(this.ImgSrc),
                Click = new Uri(this.Link)
            };

            return message;
        }
    }
}
