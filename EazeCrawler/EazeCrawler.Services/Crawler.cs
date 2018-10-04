using System;
using System.Threading;
using System.Threading.Tasks;
using EazeCrawler.Common.Events;
using EazeCrawler.Common.Interfaces;
using EazeCrawler.Common.Models;
using Quartz;
using IJob = Quartz.IJob;
using IJobDetail = EazeCrawler.Common.Interfaces.IJobDetail;

namespace EazeCrawler.Services
{
    [DisallowConcurrentExecution]
    public class Crawler : ICrawler, IJob
    {
        private readonly IEventManager _eventManager;

        public Crawler()
        {
            _eventManager = EventManager.Instance;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() =>
            {
                var result = new JobResult();
                result.UrList.Add("http://www.google.com");

                var args = new JobCompletedEventArgs {JobDetail = GetJobDetail(context), Results = result};
                Thread.Sleep(3000);
                ExecuteOnCompleted(args);
            });
        }

        private void ExecuteOnCompleted(Common.Interfaces.IJob args)
        {
            _eventManager.PublishEvent<JobCompletedEventArgs>(args);
        }

        private static IJobDetail GetJobDetail(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            var jobDetail = new JobDetail
            {
                Id = Guid.Parse(dataMap.GetString("Id")),
                Name = dataMap.GetString("Name"),
                Url = dataMap.GetString("Url"),
                Status = (JobStatus) dataMap.GetIntValue("Status"),
                CreatedAt = DateTime.Parse(dataMap.GetString("CreatedAt")),
                CompletedAt = DateTime.Parse(dataMap.GetString("CompletedAt"))
            };

            return jobDetail;
        }
    }
}