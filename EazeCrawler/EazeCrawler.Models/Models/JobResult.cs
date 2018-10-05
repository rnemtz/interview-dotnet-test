using System.Collections.Generic;
using EazeCrawler.Common.Interfaces;

namespace EazeCrawler.Common.Models
{
    public class JobResult : IJobResult
    {
        public JobResult()
        {
            List = new List<IScrapedUrlResult>();
        }

        public IList<IScrapedUrlResult> List { get; set; }
    }
}