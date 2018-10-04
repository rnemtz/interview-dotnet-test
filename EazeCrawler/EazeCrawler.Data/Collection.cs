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
        private readonly ConcurrentDictionary<Guid, IJobDetail> _jobs = new ConcurrentDictionary<Guid, IJobDetail>();
        private readonly ConcurrentDictionary<IJobDetail, IJobResult> _results = new ConcurrentDictionary<IJobDetail, IJobResult>();
        private readonly Mutex _mutex = new Mutex();

        private Collection()
        {
        }

        public void JobStarted(IJobDetail jobDetail)
        {
            jobDetail.Status = JobStatus.Running;
            _jobs.TryAdd(jobDetail.Id, jobDetail);
        }

        public void JobCompleted(IJobDetail jobDetail, IJobResult jobResult)
        {
            jobDetail.Status = JobStatus.Completed;
            _jobs.TryAdd(jobDetail.Id, jobDetail);
            _results.TryAdd(jobDetail, jobResult);
        }

        public IList<IJobDetail> GetCompletedJobs()
        {
            _mutex.WaitOne();
            var results = _jobs.Values.Where(x => x.Status == JobStatus.Completed).ToList();
            _mutex.ReleaseMutex();

            return results;
        }

        public IList<IJobDetail> GetRunningJobs()
        {
            _mutex.WaitOne();
            var results = _jobs.Values.Where(x=> x.Status == JobStatus.Running).ToList();
            _mutex.ReleaseMutex();

            return results;
        }

        /// <summary>
        /// Get available results from completed jobs
        /// </summary>
        /// <returns>List of IJobResults</returns>
        public IList<IJobResult> GetResults()
        {
            _mutex.WaitOne();
            var results = _results.Values.ToList();
            _mutex.ReleaseMutex();

            return results;
        }

        public IJobResult GetResults(IJobDetail jobDetail)
        {
            return _results.TryGetValue(jobDetail, out var result) ? result : null;
        }

        public IJobDetail GetJob(Guid jobId)
        {
            return _jobs.TryGetValue(jobId, out var jobDetail) ? jobDetail : null;
        }
    }
}
