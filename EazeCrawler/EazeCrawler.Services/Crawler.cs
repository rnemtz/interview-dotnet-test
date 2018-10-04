using System;
using System.Collections.Generic;
using System.Net.Http;
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
        private const int MaxLevels = 10;
        public Crawler()
        {
            _eventManager = EventManager.Instance;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() =>
            {
                var jobDetail = GetJobDetail(context);

                var result = new JobResult();
                var visited = new HashSet<Uri>();
                var pending = new Queue<Uri>();

                var requestedUri = new Uri(jobDetail.Url);
                pending.Enqueue(requestedUri);
                var level = 0;
                while (pending.Count > 0 || level >= MaxLevels)
                {
                    var current = pending.Dequeue();
                    visited.Add(current);

                    foreach (var uri in GetListFromUri(current))
                    {
                        if (!visited.Contains(uri)) pending.Enqueue(uri);
                    }
                }
                visited.Remove(requestedUri);
                foreach (var uri in visited) result.UrList.Add(uri.AbsoluteUri);

                var args = new JobCompletedEventArgs {JobDetail = jobDetail, Results = result};
                ExecuteOnCompleted(args);
            });
        }

        private static IEnumerable<Uri> GetListFromUri(Uri url)
        {
            var body = GetHttpContent(url);
            var result = new List<Uri>();

            return result;
        }

        private static async Task<string> GetHttpContent(Uri url)
        {
            string responseBody;
            try
            {
                using (var client = new HttpClient())
                {
                    responseBody = await client.GetStringAsync(url);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return responseBody;
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