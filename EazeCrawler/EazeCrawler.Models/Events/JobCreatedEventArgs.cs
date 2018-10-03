using System;
using EazeCrawler.Common.Interfaces;

namespace EazeCrawler.Common.Events
{
    public class JobCreatedEventArgs: EventArgs, IJobEventArgs
    {
        public IJobDetail JobDetail { get; set; }
        public IJobResult Results { get; set; }
    }
}
