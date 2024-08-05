using Sdk.Articles;
using Sdk.Contracts;

namespace selfdrive
{
    public class DllMain : IDllMain
    {
        public override void Execute()
        {
            try
            {
                var viewDOM = this.GetPageDom("https://selfdrivenews.com/");
                var article = new DllParser(viewDOM).FirstOrDefault();

                this.RaiseUpdateEvent(article);
            }
            catch (Exception ex)
            {
                this.RaiseUpdateEvent(new ExceptionArticle()
                {
                    ErrorMessage = ex.ToString()
                });
            }
        }

        public override void Initialize(string workingDirectory)
        {
            // throw new NotImplementedException();
        }
    }
}
