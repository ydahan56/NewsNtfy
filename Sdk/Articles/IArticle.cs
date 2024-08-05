using Hanssens.Net;
using ntfy.Requests;
using System.Security.Cryptography;
using System.Text;

namespace Sdk.Articles
{
    public abstract class IArticle
    {
        public string? Key { get; set; }
        public string? SiteName { get; set; }

        public string? Headline { get; set; } // Article short brief
        public string? Title { get; set; } // Article title
        public string? Entry { get; set; } // Article message body
        public string? Link { get; set; }
        public string? ImgSrc { get; set; }

        public string? ErrorMessage { get; set; } // Used in cases where we need to log errors

        public int LinkHashCode => this.GetIntHashMd5ByStringValue(this.Link);
        public int ImgSrcHashCode => this.GetIntHashMd5ByStringValue(this.ImgSrc);

        public abstract bool IsArticlePublished(ILocalStorage storage);

        public abstract SendingMessage ToMessage();

        protected int GetIntHashMd5ByStringValue(string? value)
        {
            MD5 md5Hasher = MD5.Create();

            var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(value));
            var ivalue = BitConverter.ToInt32(hashed, 0);

            return ivalue;
        }
    }
}
