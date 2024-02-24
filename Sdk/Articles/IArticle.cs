using Hanssens.Net;
using RestSharp;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Sdk.Articles
{
    public abstract class IArticle
    {
        public string? Key { get; set; }
        public string? SiteName { get; set; }

        public string? Headline { get; set; } // artile's headline (optional)
        public string? Title { get; set; } // article's title
        public string? Entry { get; set; } // small brief of the body
        public string? Link { get; set; }
        public string? ImgSrc { get; set; }

        public string? OptionalInfo { get; set; }

        public abstract IArticle GetCached(ILocalStorage storage);

        public int LinkHashCode => this.GetIntHashMd5ByStringValue(this.Link);
        public int ImgSrcHashCode => this.GetIntHashMd5ByStringValue(this.ImgSrc);

        private int GetIntHashMd5ByStringValue(string? value)
        {
            MD5 md5Hasher = MD5.Create();

            var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(value));
            var ivalue = BitConverter.ToInt32(hashed, 0);

            return ivalue;
        }
    }
}
