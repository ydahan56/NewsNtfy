using Sdk.Articles;
using Sdk.Contracts;

namespace zhk
{
    public class DllMain : IDllMain
    {
        public override void Execute()
        {
            try
            {
                var viewDOM = this.GetPageDom("https://www.zhk.co.il/category/news/");
                var article = new DllParser(viewDOM).FirstOrDefault();

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
            // throw new NotImplementedException();
        }
    }
}
