using Hanssens.Net;
using ntfy.Requests;
using Sdk.Articles;

namespace zhk.Models
{
    public class ZhkArticle : IArticle
    {
        public ZhkArticle()
        {
            this.Key = "ZHK_Karmiel";
            this.SiteName = "ZoharNet, Karmie'l";
        }

        public override bool IsArticlePublished(ILocalStorage storage)
        {
            if (storage.Exists(this.Key))
            {
                var cachedArticle = storage.Get<ZhkArticle>(this.Key);

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
