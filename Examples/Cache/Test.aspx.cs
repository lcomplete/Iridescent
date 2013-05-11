using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Iridescent.Cache;

namespace Examples.Cache
{
    public partial class Test : System.Web.UI.Page
    {
        protected DateTime dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            ICache cache = CacheFactory.Create();
            object obj = cache.Get("key");
            if(obj==null)
            {
                cache.Set("key", DateTime.Now,TimeSpan.FromSeconds(5));
            }
            dt = (DateTime) cache.Get("key");

            Response.Write(dt.ToString());
        }
    }
}