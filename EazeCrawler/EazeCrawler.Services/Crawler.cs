using System;
using System.Threading;
using System.Threading.Tasks;
using EazeCrawler.Common.Events;
using EazeCrawler.Common.Interfaces;
using EazeCrawler.Common.Models;
using Quartz;

namespace EazeCrawler.Services
{
    public class Crawler : ICrawler, IJob
    {
        public Crawler()
        {
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() =>
            {
                //var args = new JobRunningEventArgs { JobDetail = new JobDetail { Id = Guid.NewGuid(), Name = "Test From Crawler" } };
                Thread.Sleep(3000);
            });
        }
    }
}