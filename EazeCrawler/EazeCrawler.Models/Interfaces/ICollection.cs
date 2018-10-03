using System.Collections.Generic;
using System.Threading.Tasks;

namespace EazeCrawler.Common.Interfaces
{
    public interface ICollection
    {
        IJobResult GetResults(IJobDetail jobDetail);
        IList<IJobResult> GetResults();
    }
}
