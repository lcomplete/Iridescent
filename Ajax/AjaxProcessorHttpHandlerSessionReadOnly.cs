using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.SessionState;

namespace Iridescent.Ajax
{
    internal class AjaxProcessorHttpHandlerSessionReadOnly:AjaxProcessorHttpHandler,IRequiresSessionState,IReadOnlySessionState
    {
        public AjaxProcessorHttpHandlerSessionReadOnly(Type type) : base(type)
        {
        }
    }
}
