using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Iridescent.Ajax
{
    public sealed class Utility
    {
        Utility(){}

        internal static Hashtable UrlNamespaceMappings;
        const string URL_PREFIX = "/Iridescent/Ajax/";

        static Utility()
        {
            UrlNamespaceMappings=new Hashtable();
        }

        public static void RegisterType(Type type)
        {
            RegisterType(type,(Page)HttpContext.Current.Handler);
        }

        public static void RegisterType(Type type,Page page)
        {
            RegisterCommonScript(page);
            string typeAndAssemblyName = type.FullName + "," + type.Assembly.GetName().Name;
            if(!UrlNamespaceMappings.ContainsKey(typeAndAssemblyName))
            {
                UrlNamespaceMappings.Add(typeAndAssemblyName,type.AssemblyQualifiedName);
            }
            RegisterClientScriptBlock(page,typeAndAssemblyName,URL_PREFIX+typeAndAssemblyName+".ashx");
        }

        internal static void RegisterCommonScript(Page page)
        {
            RegisterClientScriptBlock(page, "Ajax.Core", URL_PREFIX+"Core.ashx");
        }

        internal static void RegisterClientScriptBlock(Page page,string key,string url)
        {
            if(!page.ClientScript.IsClientScriptBlockRegistered(key))
            {
                page.ClientScript.RegisterClientScriptBlock(page.GetType(),
                    key, "<script type=\"text/javascript\" src=\"" + url + "\"></script>");
            }
        }
    }
}
