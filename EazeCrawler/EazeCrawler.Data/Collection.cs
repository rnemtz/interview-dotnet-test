using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
            if (!_jobs.ContainsKey(jobDetail.Id)) return;

            var currentJobDetail = _jobs[jobDetail.Id];
            jobDetail.Status = JobStatus.Completed;
            
            _jobs.TryUpdate(jobDetail.Id, jobDetail, currentJobDetail);
            _results.TryAdd(jobDetail, jobResult);
        }

        public IList<IJobDetail> GetCompletedJobs()
        {
            var results = _jobs.Values.Where(x => x.Status == JobStatus.Completed).ToList();

            return results;
        }

        public IList<IJobDetail> GetRunningJobs()
        {
            var results = _jobs.Values.Where(x=> x.Status == JobStatus.Running).ToList();

            return results;
        }

        /// <summary>
        /// Get available results from completed jobs
        /// </summary>
        /// <returns>List of IJobResults</returns>
        public IList<IJobResult> GetResults()
        {
            var results = _results.Values.ToList();

            return results;
        }

        public IJobResult GetResults(IJobDetail jobDetail)
        {
            return _results.TryGetValue(jobDetail, out var result) ? result : null;
        }

        public IJob GetJob(Guid jobId)
        {
            var job = new Job();
            if (!_jobs.TryGetValue(jobId, out var jobDetail)) return null;

            job.JobDetail = jobDetail;
            job.Results = jobDetail.Status == JobStatus.Completed ? _results[jobDetail] : null;
            return job;
        }
    }
}
