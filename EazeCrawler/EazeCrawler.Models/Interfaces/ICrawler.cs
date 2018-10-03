using System.Threading.Tasks;

namespace EazeCrawler.Common.Interfaces
{
    public interface ICrawler
    {
        Task<bool> Execute(IJobDetail jobDetail);
    }
}
