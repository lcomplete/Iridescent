using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridescent.Ajax
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple = false)]
    public class AjaxMethodAttribute:Attribute
    {
        public HttpSessionState SessionState { get; private set; }

        public AjaxMethodAttribute():this(HttpSessionState.ReadWrite)
        {
            
        }

        public AjaxMethodAttribute(HttpSessionState sessionState)
        {
            SessionState = sessionState;
        }
    }
}
