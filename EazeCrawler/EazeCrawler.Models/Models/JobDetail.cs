using System;
using EazeCrawler.Common.Interfaces;

namespace EazeCrawler.Common.Models
{
    public class JobDetail : IJobDetail
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public JobStatus Status { get; set; }
    }

    public enum JobStatus
    {
        Pending = 1,
        Running = 2,
        Completed = 3,
        Failed = 0
    }
}