using DotNetEnv;
using Newtonsoft.Json;
using Nito.AsyncEx;
using RestSharp;

namespace Sdk.ImageShack
{
    public class ImageShackClient
    {
        private readonly string apikey;
        private readonly RestClient client;

        public ImageShackClient()
        {
            this.apikey = Env.GetString("imageshack_apikey");
            this.client = new RestClient("https://post.imageshack.us");
        }

        public ImageShackResult UploadImageByFile(string fileName, string filePath)
        {
            if (string.IsNullOrWhiteSpace(this.apikey))
            {
                throw new System.Exception("ImageShack API key is missing");
            }

            var request = new RestRequest("upload_api.php", Method.Post);

            request.AddParameter("key", this.apikey);
            request.AddParameter("format", "json");
            request.AddFile("fileupload", filePath);

            var response = this.client.Execute<ImageShackResult>(request);

            return response.Data;
        }
    }
}
