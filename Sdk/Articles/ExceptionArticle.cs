using Hanssens.Net;
using ntfy.Requests;

namespace Sdk.Articles
{
    public class ExceptionArticle : IArticle
    {
        public override bool IsArticlePublished(ILocalStorage storage)
        {
            throw new NotImplementedException();
        }

        public override SendingMessage ToMessage()
        {
            throw new NotImplementedException();
        }
    }
}
