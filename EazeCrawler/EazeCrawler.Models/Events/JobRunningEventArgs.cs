using System;
using EazeCrawler.Common.Interfaces;

namespace EazeCrawler.Common.Events
{
    public class JobRunningEventArgs : EventArgs, IJob
    {
        public IJobDetail JobDetail { get; set; }
        public IJobResult Results { get; set; }
    }
}