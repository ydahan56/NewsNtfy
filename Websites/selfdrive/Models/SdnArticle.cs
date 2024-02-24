using Hanssens.Net;
using Sdk.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace selfdrive.Models
{
    public class SdnArticle : IArticle
    {
        public SdnArticle()
        {
            this.Key = "SDN";
            this.SiteName = "SelfDriveNews";
        }
        public override IArticle GetCached(ILocalStorage storage)
        {
            return storage.Get<SdnArticle>(this.Key);
        }
    }
}
