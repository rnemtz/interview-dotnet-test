using System;
using System.Threading.Tasks;
using EazeCrawler.Common.Interfaces;
using Quartz;
using IJobDetail = EazeCrawler.Common.Interfaces.IJobDetail;

namespace EazeCrawler.Services
{
    public class Crawler : ICrawler, IJob
    {
        public async Task<bool> Execute(IJobDetail jobDetail)
        {
            await Task.Run(() =>
            {
                
            });
            return true;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() =>
            {

            });
        }
    }
}