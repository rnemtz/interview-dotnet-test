using System;

namespace EazeCrawler.Common.Interfaces
{
    public interface IJobDetail
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }
}
