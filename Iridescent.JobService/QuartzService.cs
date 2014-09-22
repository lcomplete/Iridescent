using log4net;
using Quartz;
using Quartz.Impl;

namespace Iridescent.JobService
{
    class QuartzService
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(QuartzService));

        private IScheduler scheduler = null;

        public void Start()
        {
            _log.Info("统计服务正在启动");

            scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
        }

        public void Stop()
        {
            if (scheduler != null && !scheduler.IsShutdown)
                scheduler.Shutdown();

            _log.Info("统计服务已停止");
        }
    }
}
