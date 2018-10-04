using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using EazeCrawler.Common.Events;
using EazeCrawler.Common.Interfaces;
using EazeCrawler.Common.Models;
using EazeCrawler.Common.Structures;
using EazeCrawler.Data;
using Quartz;
using Quartz.Impl;
using IJob = EazeCrawler.Common.Interfaces.IJob;
using IJobDetail = EazeCrawler.Common.Interfaces.IJobDetail;
using IScheduler = EazeCrawler.Common.Interfaces.IScheduler;

namespace EazeCrawler.Services
{
    public class Scheduler : IScheduler
    {
        private readonly EventQueue<IJobDetail> _pendingJobs = new EventQueue<IJobDetail>();
        private readonly IEventManager _eventManager;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ICollection _dataCollection;

        public Scheduler()
        {
            _eventManager = EventManager.Instance;

            _schedulerFactory = new StdSchedulerFactory();

            _dataCollection = Collection.Instance;

            _eventManager.JobCreated += EventManagerJobStarted;
            _eventManager.JobCompleted += EventManagerJobCompleted;
            _pendingJobs.ItemEnqueued += PendingJobsItemEnqueued;
        }

        //Public Methods
        public async Task<IJobResult> GetResults(Guid id)
        {
            return await Task.Run(() => _dataCollection.GetJob(id).Results);
        }

        /// <summary>
        /// Request to return results from Data Collection
        /// </summary>
        /// <returns>List of IJobResult</returns>
        public async Task<IList<IJobResult>> GetResults()
        {
            return await Task.Run(() => _dataCollection.GetResults());
        }

        public async Task<IJob> GetJobStatus(Guid jobId)
        {
            return await Task.Run(() => _dataCollection.GetJob(jobId));
        }

        //Private Methods
        public async Task<IJobDetail> ScheduleJob(IJobDetail jobDetail)
        {
            jobDetail.Status = JobStatus.Pending;
            jobDetail.CreatedAt = DateTime.UtcNow;
            await Task.Run(() =>
            {
                _pendingJobs.Enqueue(jobDetail);
                Task.Run(() => _eventManager.PublishEvent<JobCreatedEventArgs>(new JobCreatedEventArgs {JobDetail = jobDetail}));

            });
            return jobDetail;
        }

        private async Task ExecuteJob(IJobDetail jobDetail)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var job = JobBuilder.Create<Crawler>()
                .WithIdentity(jobDetail.Id.ToString(), jobDetail.Name)
                .UsingJobData("Id", jobDetail.Id.ToString())
                .UsingJobData("Name", jobDetail.Name)
                .UsingJobData("Url", jobDetail.Url)
                .UsingJobData("Status", (int)jobDetail.Status)
                .UsingJobData("CreatedAt", jobDetail.CreatedAt.ToString(CultureInfo.InvariantCulture))
                .UsingJobData("CompletedAt", jobDetail.CompletedAt.ToString(CultureInfo.InvariantCulture))
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity(jobDetail.Id.ToString(), jobDetail.Name)
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(job, trigger);
            await scheduler.Start();
        }

        //Events
        private void PendingJobsItemEnqueued(object sender, IJobDetail e)
        {
            if (_pendingJobs.TryDequeue(out var jobDetail)) Task.Run(() => ExecuteJob(jobDetail));
        }

        private void EventManagerJobCompleted(object sender, IJob e)
        {
            Task.Run(() =>
            {
                var job = (JobCompletedEventArgs) e;
                job.JobDetail.CompletedAt = DateTime.UtcNow;
                _dataCollection.JobCompleted(job.JobDetail, job.Results);
            });
        }

        private void EventManagerJobStarted(object sender, IJob e)
        {
            Task.Run(() =>
            {
                var job = (JobCreatedEventArgs) e;
                _dataCollection.JobStarted(job.JobDetail);
            });
        }
    }
}