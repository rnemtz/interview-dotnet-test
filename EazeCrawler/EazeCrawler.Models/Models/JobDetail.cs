using System;
using EazeCrawler.Common.Interfaces;

namespace EazeCrawler.Common.Models
{
    public class JobDetail:IJobDetail
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
