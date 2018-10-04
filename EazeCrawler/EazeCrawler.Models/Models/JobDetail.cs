using System;
using EazeCrawler.Common.Interfaces;

namespace EazeCrawler.Common.Models
{
    public class JobDetail : IJobDetail
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public JobStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime CompletedAt { get; set; }
    }

    public enum JobStatus
    {
        Pending = 0,
        Running = 1,
        Completed = 2,
        Failed = 3
    }
}