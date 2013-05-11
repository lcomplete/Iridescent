using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Examples
{
    public class Ajax
    {
        [Iridescent.Ajax.AjaxMethod]
        public int Add(int a,int b)
        {
            return a + b;
        }
    }
}