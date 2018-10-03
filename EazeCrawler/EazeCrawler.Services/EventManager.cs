using System;
using EazeCrawler.Common.Events;
using EazeCrawler.Common.Interfaces;

namespace EazeCrawler.Services
{
    public class EventManager : IEventManager
    {
        public event EventHandler<IJobEventArgs> JobCompleted;
        public event EventHandler<IJobEventArgs> JobCreated;
        public event EventHandler<IJobEventArgs> JobRunning;

        /// <summary>
        /// Publish Job Event
        /// </summary>
        /// <typeparam name="T">Job Event Arguments Type</typeparam>
        /// <param name="args">Job Event Arguments</param>
        public void PublishEvent<T>(IJobEventArgs args)
        {
            if (typeof(T) == typeof(JobCompletedEventArgs)) OnJobCompleted(args);
            else if (typeof(T) == typeof(JobCreatedEventArgs)) OnJobCreated(args);
            else if (typeof(T) == typeof(JobRunningEventArgs)) OnJobRunning(args);
        }

        public virtual void OnJobCompleted(IJobEventArgs e)
        {
            JobCompleted?.Invoke(this, e);
        }

        public virtual void OnJobCreated(IJobEventArgs e)
        {
            JobCreated?.Invoke(this, e);
        }

        public virtual void OnJobRunning(IJobEventArgs e)
        {
            JobRunning?.Invoke(this, e);
        }
    }
}