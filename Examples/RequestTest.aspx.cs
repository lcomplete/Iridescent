using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Examples
{
    public partial class RequestTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write(Request.Headers["X-IridescentAjax-Method"]);
            Response.Write(Environment.NewLine);
            Response.Write(Request.Params["abc"]);
        }
    }
}