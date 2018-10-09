using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EazeCrawler.Common.Interfaces
{
    public interface IScheduler
    {
        Task<IJobDetail> ScheduleJob (IJobDetail jobDetail);
        Task<IList<IScrapedUrlResult>> GetResults(Guid id);
        Task<IList<IScrapedUrlResult>> GetResults();
        Task<IJob> GetJobStatus(Guid jobId);
        Task<IJobsDeletedResult> DeleteResults();
    }
}
