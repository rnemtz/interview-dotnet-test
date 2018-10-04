using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EazeCrawler.Common.Interfaces;
using EazeCrawler.Common.Models;

namespace EazeCrawler.Data
{
    public sealed class Collection: ICollection
    {
        public static ICollection Instance => Lazy.Value;
        private static readonly Lazy<ICollection> Lazy = new Lazy<ICollection>(() => new Collection());
        private readonly ConcurrentDictionary<Guid, IJobDetail> _completedJobs = new ConcurrentDictionary<Guid, IJobDetail>();
        private readonly ConcurrentDictionary<Guid, IJobDetail> _runningJobs = new ConcurrentDictionary<Guid, IJobDetail>();
        private readonly ConcurrentDictionary<IJobDetail, IJobResult> _results = new ConcurrentDictionary<IJobDetail, IJobResult>();
        private readonly Mutex _mutex = new Mutex();

        private Collection()
        {
        }

        public void JobStarted(IJobDetail jobDetail)
        {
            _runningJobs.TryAdd(jobDetail.Id, jobDetail);
        }

        public void JobCompleted(IJobDetail jobDetail, IJobResult jobResult)
        {
            _runningJobs.TryRemove(jobDetail.Id, out var _);
            _completedJobs.TryAdd(jobDetail.Id, jobDetail);
            _results.TryAdd(jobDetail, jobResult);
        }

        public IList<IJobDetail> GetCompletedJobs()
        {
            _mutex.WaitOne();
            var results = _completedJobs.Values.ToList();
            _mutex.ReleaseMutex();

            return results;
        }

        public IList<IJobDetail> GetRunningJobs()
        {
            _mutex.WaitOne();
            var results = _runningJobs.Values.ToList();
            _mutex.ReleaseMutex();

            return results;
        }

        public IList<IJobResult> GetResults()
        {
            _mutex.WaitOne();
            var results = _results.Values.ToList();
            _mutex.ReleaseMutex();

            return results;
        }

        public IJobResult GetResults(IJobDetail jobDetail)
        {
            return _results.TryGetValue(jobDetail, out var result) ? result : new JobResult();
        }
    }
}
