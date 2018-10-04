using System;

namespace EazeCrawler.Common.Interfaces
{
    public interface IEventManager
    {
        event EventHandler<IJob> JobCompleted;
        event EventHandler<IJob> JobCreated;
        event EventHandler<IJob> JobRunning;
        void PublishEvent<T>(IJob args);
    }
}