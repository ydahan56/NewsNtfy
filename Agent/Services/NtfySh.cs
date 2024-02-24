﻿using DotNetEnv;
using RestSharp;
using System.Web;

namespace NewsNotify.Services
{
    public class NtfySh
    {
        private readonly string _endpoint;
        private readonly RestClient client;

        public NtfySh()
        {
            this._endpoint = Env.GetString("publishEndpoint");
            this.client = new RestClient("https://ntfy.sh");
        }

        public void notifyArticleChanged(string? title, string? text, string? imgUrl, string? link)
        {
            var request = new RestRequest(this._endpoint);

            request.AddHeader("Title", title);
            request.AddHeader("Click", link);

            if (imgUrl.Contains("https"))
            {
                request.Method = Method.Post;
                request.AddHeader("Attach", imgUrl);
            }
            else
            {
                request.Method = Method.Put;
                request.AddFile("Filename", imgUrl);
            }

            request.AddBody(text);

            this.client.Execute(request);
        }
    }
}
