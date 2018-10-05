using System.Collections.Generic;

namespace EazeCrawler.Common.Interfaces
{
    public interface IJobResult
    {
        IList<IScrapedUrl> List { get; set; }
    }
}
