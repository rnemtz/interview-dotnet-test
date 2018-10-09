using System.Net;

namespace EazeCrawler.Common.Interfaces
{
    public interface IJobsDeletedResult
    {
        string Mesasge { get; set; }
        int DeletedResults { get; set; }
        HttpStatusCode StatusCode { get; set; }
    }
}
