using System.Collections.Generic;

namespace EazeCrawler.Common.Interfaces
{
    public interface ICollection
    {
        void JobStarted(IJobDetail jobDetail);
        void JobCompleted(IJobDetail jobDetail, IJobResult jobResult);
        IList<IJobDetail> GetCompletedJobs();
        IList<IJobDetail> GetRunningJobs();
        IList<IJobResult> GetResults();
        IJobResult GetResults(IJobDetail jobDetail);
    }
}
