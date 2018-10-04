namespace EazeCrawler.Common.Interfaces
{
    public interface IJobEventArgs
    {
        IJobDetail JobDetail { get; set; }
        IJobResult Results { get; set; }
    }
}