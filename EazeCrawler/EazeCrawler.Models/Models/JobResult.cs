using System.Collections.Generic;
using EazeCrawler.Common.Interfaces;

namespace EazeCrawler.Common.Models
{
    public class JobResult : IJobResult
    {
        public IList<string> UrList { get; set; }

        public JobResult()
        {
            UrList = new List<string>();
        }
    }
}