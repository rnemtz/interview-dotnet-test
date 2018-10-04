using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EazeCrawler.Common.Interfaces
{
    public interface IScheduler
    {
        Task<IJobDetail> ScheduleJob (IJobDetail jobDetail);
        Task<IJobResult> GetResults(Guid id);
        Task<IList<IJobResult>> GetResults();
        Task<IJob> GetJobStatus(Guid jobId);
    }
}
