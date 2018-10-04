using System;
using EazeCrawler.Common.Models;

namespace EazeCrawler.Common.Interfaces
{
    public interface IJobDetail
    {
        Guid Id { get; set; }
        string Name { get; set; }
        JobStatus Status { get; set; }
    }
}
