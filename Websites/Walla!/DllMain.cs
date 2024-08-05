using Hanssens.Net;
using ImageMagick;
using Nito.AsyncEx;
using Sdk.Articles;
using Sdk.Contracts;
using Sdk.ImageShack;
using Walla_.Models;

namespace Walla_
{
    public class DllMain : IDllMain
    {
        private string? workingDirectory;
        private LocalStorage? cache;
        private ImageShackClient? imageShackClient;

        public override void Execute()
        {
            try
            {
                var viewDOM = this.GetPageDom("https://www.walla.co.il/");
                var article = new DllParser(viewDOM).FirstOrDefault();

                // to prevent duplicate in imageshack's profile
                var isSame = this.IsCacheArticleImgHashEquals(article, out ImgHash cachedImgSrc);

                if (!isSame)
                {
                    this.UpdateArticleImgSrc(ref article, cachedImgSrc);
                }

                this.RaiseUpdateEvent(article);
            }
            catch (Exception ex)
            {
                this.RaiseUpdateEvent(
                    new ExceptionArticle()
                    {
                        ErrorMessage = ex.ToString()
                    }
                );
            }
        }

        public override void Initialize(string workingDirectory)
        {
            this.workingDirectory = workingDirectory;
            this.cache = new LocalStorage(
                new LocalStorageConfiguration()
                {
                    Filename = Path.Combine(this.workingDirectory, ".localstorage")
                }
            );
            this.imageShackClient = new ImageShackClient();
        }

        private bool IsCacheArticleImgHashEquals(IArticle article, out ImgHash cachedImgSrc)
        {
            // init with default value
            cachedImgSrc = new ImgHash();

            // check for existing hash in cache
            if (this.cache.Exists(nameof(cachedImgSrc)))
            {
                cachedImgSrc = this.cache.Get<ImgHash>(nameof(cachedImgSrc));
            }

            // we compare the current hash to the cached copy to prevent duplicate of the article's image!
            // this mechanism is to prevent uploading a duplicate to imageshack of the same article /img
            return cachedImgSrc.hash == article.ImgSrcHashCode;
        }

        private void UpdateArticleImgSrc(ref IArticle article, ImgHash cachedImgSrc)
        {
            var fileName = Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + ".jpg";
            var filePath = Path.Combine(this.workingDirectory, fileName);

            this.WriteImageToDisk(article.ImgSrc, filePath);
            var PutRes = this.imageShackClient.UploadImageByFile(fileName, filePath);

            // delete afterwards
            System.IO.File.Delete(filePath);

            // store new imgsrc to cache
            this.CommitImgSrcCache(cachedImgSrc, article.ImgSrcHashCode, PutRes.links.image_link);

            // update img link in the article
            article.ImgSrc = PutRes.links.image_link;
        }

        private void CommitImgSrcCache(ImgHash cachedImgSrc, int hash, string imagelink)
        {
            // update the cached imgsrc content
            cachedImgSrc.hash = hash;
            cachedImgSrc.url = imagelink;

            // save the updated imgsrc in the cache
            this.cache.Store(nameof(cachedImgSrc), cachedImgSrc);
            this.cache.Persist();
        }

        private void WriteImageToDisk(string imageUrl, string filePath)
        {
            using HttpClient client = new HttpClient();

            var bytes = AsyncContext.Run(
                async () => await client.GetByteArrayAsync(imageUrl)
            );

            using var magick = new MagickImage(bytes)
            {
                Format = MagickFormat.Jpeg
            };

            magick.Write(filePath);
        }
    }
}
