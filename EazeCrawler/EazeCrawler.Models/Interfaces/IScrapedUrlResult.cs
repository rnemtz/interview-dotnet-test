namespace EazeCrawler.Common.Interfaces
{
    public interface IScrapedUrlResult
    {
        string Title { get; set; }
        string Description { get; set; }
        string Url { get; set; }
    }
}
