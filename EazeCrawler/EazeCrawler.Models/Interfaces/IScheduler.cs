using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EazeCrawler.Common.Interfaces
{
    public interface IScheduler
    {
        Task<IJobDetail> ScheduleJob (IJobDetail jobDetail);
        Task<IJobResult> GetResults(IJobDetail jobDetail);
        Task<IList<IJobResult>> GetResults();
        Task<IJobDetail> GetJobStatus(Guid jobId);
    }
}
