using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.MobileControls;
using System.Web.UI.WebControls;
using Iridescent.Cache;

namespace Examples.Cache
{
    [Serializable]
    class MyClass
    {
        public string A { get; set; }

        public MyClass()
        {
            A = "a";
        }

        public MyClass Class { get; set; }

        public int B { get; set; }
    }

    public partial class Test : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            ICache cache = CacheFactory.Create();
            object obj = cache.Get("key");

            if(obj==null)
            {
                obj = new MyClass();
                List<MyClass> objList=new List<MyClass>(){obj as MyClass};
                var sb = (new StringBuilder()).Append("123");

                DataTable dt=new DataTable();
                dt.Columns.Add("c");
                DataRow row= dt.NewRow();
                row["c"] = DateTime.Now;
                dt.Rows.Add(row);

                cache.Set("key",objList ,DateTime.Now.AddSeconds(5));
            }

            Response.Write(cache.Get<List<MyClass>>("key") + "<br>");

        }
    }
}