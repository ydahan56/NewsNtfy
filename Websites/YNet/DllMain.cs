using Sdk.Articles;
using Sdk.Contracts;

namespace YNet
{
    public class DllMain : IDllMain
    {
        public override void Execute()
        {
            try
            {
                var viewDOM = this.GetPageDom("https://www.ynet.co.il/home/0,7340,L-8,00.html");
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
