namespace EazeCrawler.Common.Interfaces
{
    public interface IJob
    {
        IJobDetail JobDetail { get; set; }
        IJobResult Results { get; set; }
    }
}