using System;

namespace EazeCrawler.Common.Interfaces
{
    public interface IEventManager
    {
        event EventHandler<IJobEventArgs> JobCompleted;
        event EventHandler<IJobEventArgs> JobCreated;
        event EventHandler<IJobEventArgs> JobRunning;
        void PublishEvent<T>(IJobEventArgs args);
    }
}