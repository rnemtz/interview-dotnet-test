using System.Net;
using EazeCrawler.Common.Interfaces;

namespace EazeCrawler.Common.Models
{
    public class JobsDeletedResult: IJobsDeletedResult
    {
        public string Mesasge { get; set; }
        public int DeletedResults { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public JobsDeletedResult()
        {
            DeletedResults = 0;
            StatusCode = HttpStatusCode.OK;
        }
    }
}
