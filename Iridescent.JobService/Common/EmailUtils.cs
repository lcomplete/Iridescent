using System.Configuration;
using Iridescent.Utils.FluentEmail;

namespace Iridescent.JobService.Common
{
    internal static class EmailUtils
    {
        /// <summary>
        /// 发送通知给服务维护人员
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isHtml"></param>
        public static void SendToMaintainer(string subject,string body,bool isHtml=true)
        {
            string maintainer = ConfigurationManager.AppSettings["mailReceiver"];
            if(!string.IsNullOrEmpty(maintainer))
            {
                Email.FromDefaultSetting()
                    .To(maintainer)
                    .Subject(subject)
                    .Body(body, isHtml)
                    .Send();
            }
        }
    }
}
