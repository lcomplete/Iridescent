using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Examples
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["abc"] = "sessionValue";
            Iridescent.Ajax.Utility.RegisterType(typeof(_Default));
            Iridescent.Ajax.Utility.RegisterType(typeof(Ajax));
        }

        [Iridescent.Ajax.AjaxMethod]
        public string GetServerTime(string test)
        {
            return DateTime.Now.ToString()+", "+test;
        }

        [Iridescent.Ajax.AjaxMethod(Iridescent.Ajax.HttpSessionState.ReadOnly)]
        public string GetSessionValue()
        {
            return Session["abc"].ToString();
        }
    }
}