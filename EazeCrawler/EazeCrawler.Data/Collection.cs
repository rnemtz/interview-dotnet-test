using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            return _jobs.Values.Where(x => x.Status == JobStatus.Completed).ToList();
        }

        public IList<IJobDetail> GetRunningJobs()
        {
            return _jobs.Values.Where(x=> x.Status == JobStatus.Running).ToList();
        }

        public IList<IScrapedUrlResult> GetResults()
        {
            var result = new List<IScrapedUrlResult>();
            foreach (var item in _results.Values.Where(x => x.List.Count > 0).ToList())
                result.AddRange(item.List);

            return result;
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

        public IJobsDeletedResult DeleteResults()
        {
            var jobs = new JobsDeletedResult(); 
            try
            {
                jobs.DeletedResults = _results.Values.Count(x => x.List.Count > 0);
                jobs.Mesasge = "All results were deleted sucessfully";
                foreach (var result in _results) _jobs.TryRemove(result.Key.Id, out _);
                _results.Clear();
            }
            catch (Exception exception)
            {
                jobs.StatusCode = HttpStatusCode.InternalServerError;
                jobs.Mesasge = exception.Message;
            }
            return jobs;
        }
    }
}
