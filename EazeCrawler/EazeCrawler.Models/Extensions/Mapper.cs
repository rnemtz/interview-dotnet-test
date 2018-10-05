using EazeCrawler.Common.Interfaces;
using EazeCrawler.Common.Models;

namespace EazeCrawler.Common.Extensions
{
    public static class Mapper
    {
        public static IScrapedUrlResult ToScrapedUrlResult(this IScrapedUrl scrapedUrl)
        {
            var scrapedUrlResult = new ScrapedUrlResult
            {
                Title = scrapedUrl.Title, Description = scrapedUrl.Description, Url = scrapedUrl.Url
            };
            return scrapedUrlResult;

        }
    }
}
