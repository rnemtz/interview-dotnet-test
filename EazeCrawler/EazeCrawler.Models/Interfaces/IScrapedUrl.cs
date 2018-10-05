namespace EazeCrawler.Common.Interfaces
{
    public interface IScrapedUrl
    {
        string Title { get; set; }
        string Description { get; set; }
        string Url { get; set; }
    }
}
