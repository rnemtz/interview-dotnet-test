using System.Collections.Specialized;
using System.Threading.Tasks;
using EazeCrawler.Common.Interfaces;
using Quartz;
using Quartz.Impl;
using IJobDetail = EazeCrawler.Common.Interfaces.IJobDetail;
using IScheduler = EazeCrawler.Common.Interfaces.IScheduler;

namespace EazeCrawler.Services
{
    public class Scheduler : IScheduler
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public Scheduler()
        {
            var props = new NameValueCollection {{"quartz.serializer.type", "binary"}};
            _schedulerFactory = new StdSchedulerFactory(props);
        }


        public async Task<IJobDetail> ExecuteJob(IJobDetail jobDetail)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.Start();

            var job = JobBuilder.Create<Crawler>()
                .WithIdentity(jobDetail.Id.ToString(), jobDetail.Name)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger", jobDetail.Name)
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(job, trigger);
           
            return jobDetail;
        }
    }
}