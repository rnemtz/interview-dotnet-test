using EazeCrawler.Common.Interfaces;

namespace EazeCrawler.Common.Models
{
    public class Job : IJob
    {
        public IJobDetail JobDetail { get; set; }
        public IJobResult Results { get; set; }

        public Job()
        {
            JobDetail = new JobDetail();
            Results = new JobResult();
        }
    }
}
