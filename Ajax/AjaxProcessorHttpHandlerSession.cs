using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.SessionState;

namespace Iridescent.Ajax
{
    internal class AjaxProcessorHttpHandlerSession:AjaxProcessorHttpHandler,IRequiresSessionState
    {
        public AjaxProcessorHttpHandlerSession(Type type) : base(type)
        {
        }
    }
}
