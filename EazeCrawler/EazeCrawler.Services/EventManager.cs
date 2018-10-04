using System;
using EazeCrawler.Common.Events;
using EazeCrawler.Common.Interfaces;

namespace EazeCrawler.Services
{
    public sealed class EventManager : IEventManager
    {
        public event EventHandler<IJob> JobCompleted;
        public event EventHandler<IJob> JobCreated;
        public event EventHandler<IJob> JobRunning;

        public static IEventManager Instance => Lazy.Value;
        private static readonly Lazy<IEventManager> Lazy = new Lazy<IEventManager>(() => new EventManager());
        private EventManager()
        {
        }

        /// <summary>
        /// Publish Job Event
        /// </summary>
        /// <typeparam name="T">Job Event Arguments Type</typeparam>
        /// <param name="args">Job Event Arguments</param>
        public void PublishEvent<T>(IJob args)
        {
            if (typeof(T) == typeof(JobCompletedEventArgs)) OnJobCompleted(args);
            else if (typeof(T) == typeof(JobCreatedEventArgs)) OnJobCreated(args);
            else if (typeof(T) == typeof(JobRunningEventArgs)) OnJobRunning(args);
        }

        public void OnJobCompleted(IJob e)
        {
            JobCompleted?.Invoke(this, e);
        }

        public void OnJobCreated(IJob e)
        {
            JobCreated?.Invoke(this, e);
        }

        public void OnJobRunning(IJob e)
        {
            JobRunning?.Invoke(this, e);
        }
    }
}