using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EazeCrawler.Common.Events;
using EazeCrawler.Common.Interfaces;
using EazeCrawler.Common.Models;

namespace EazeCrawler.Data
{
    public class Collection: ICollection
    {
        private readonly ConcurrentDictionary<Guid, IJobDetail> _completedJobs = new ConcurrentDictionary<Guid, IJobDetail>();
        private readonly ConcurrentDictionary<Guid, IJobDetail> _runningJobs = new ConcurrentDictionary<Guid, IJobDetail>();
        private readonly ConcurrentDictionary<IJobDetail, IJobResult> _results = new ConcurrentDictionary<IJobDetail, IJobResult>();
        private readonly Mutex _mutex = new Mutex();

        public Collection(IEventManager eventManagerService)
        {
            var eventManagerService1 = eventManagerService;
            eventManagerService1.JobCompleted += EventManagerServiceJobCompleted;
            eventManagerService1.JobRunning += EventManagerServiceJobRunning;

        }

        private void EventManagerServiceJobRunning(object sender, IJobEventArgs e)
        {
            if (!(e is JobRunningEventArgs arguments)) return;
            _runningJobs.TryAdd(arguments.JobDetail.Id, arguments.JobDetail);
        }

        private void EventManagerServiceJobCompleted(object sender, IJobEventArgs e)
        {
            if (!(e is JobCompletedEventArgs arguments)) return;

            _runningJobs.TryRemove(arguments.JobDetail.Id, out var _);

            if (!_completedJobs.TryAdd(arguments.JobDetail.Id, arguments.JobDetail))
                _completedJobs[arguments.JobDetail.Id] = arguments.JobDetail;

            _results.TryAdd(arguments.JobDetail, arguments.Results);
        }

        public IJobResult GetResults(IJobDetail jobDetail)
        {
            return _results.TryGetValue(jobDetail, out var results) ? results : new JobResult();
        }

        public IList<IJobResult> GetResults()
        {
            _mutex.WaitOne();

            var result = _results.Values.ToList();

            _mutex.ReleaseMutex();

            return result;
        }
    }
}
