using System.Collections.Generic;

namespace EazeCrawler.Common.Interfaces
{
    public interface IJobResult
    {
        IList<IScrapedUrlResult> List { get; set; }
    }
}
