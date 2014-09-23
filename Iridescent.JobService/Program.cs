using System;
using Iridescent.JobService.Common;
using log4net;
using Topshelf;

namespace Iridescent.JobService
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            HostFactory.Run(x =>
            {
                x.Service<QuartzService>(s =>
                {
                    s.ConstructUsing(
                        name => new QuartzService());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("service description");
                x.SetDisplayName("job service");
                x.SetServiceName("job service");
            });
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject, e.IsTerminating);
        }

        static void HandleException(Exception e, bool isTerminating)
        {
            ILog log = LogManager.GetLogger(typeof(Program));
            log.Fatal("-----------------------Statistics服务出现未捕获异常", e);

            if (isTerminating)
            {
                EmailUtils.SendToMaintainer("Statistics服务出现异常，非正常终止",
                                            "Statistics服务出现非正常终止，异常如下：<br><blockquote>" + e.ToString() + "</blockquote>");
            }
        }
    }
}
