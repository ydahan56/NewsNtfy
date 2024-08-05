using Sdk.Articles;

namespace Sdk.Contracts
{
    public interface IArticleParser
    {
        public IArticle FirstOrDefault();
    }
}
