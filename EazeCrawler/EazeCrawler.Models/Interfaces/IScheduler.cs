using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EazeCrawler.Common.Interfaces
{
    public interface IScheduler
    {
        Task<IJobDetail> ExecuteJob(IJobDetail jobDetail);
    }
}
