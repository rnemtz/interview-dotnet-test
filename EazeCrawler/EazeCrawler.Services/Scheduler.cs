using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EazeCrawler.Common.Events;
using EazeCrawler.Common.Interfaces;
using EazeCrawler.Common.Models;
using EazeCrawler.Common.Structures;
using EazeCrawler.Data;
using Quartz;
using Quartz.Impl;
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

        public Scheduler(IEventManager eventManager)
        {
            _eventManager = eventManager;

            _schedulerFactory = new StdSchedulerFactory();

            _dataCollection = Collection.Instance;

            _eventManager.JobCreated += EventManagerJobCreated;
            _eventManager.JobCompleted += EventManagerJobCompleted;
            _pendingJobs.ItemEnqueued += PendingJobsItemEnqueued;
        }

        //Public Methods
        public async Task<IJobResult> GetResults(IJobDetail jobDetail)
        {
            return await Task.Run(() =>  _dataCollection.GetResults(jobDetail));
        }

        /// <summary>
        /// Request to return results from Data Collection
        /// </summary>
        /// <returns>List of IJobResult</returns>
        public async Task<IList<IJobResult>> GetResults()
        {
            return await Task.Run(() => _dataCollection.GetResults());
        }

        public async Task<IJobDetail> GetJobStatus(Guid jobId)
        {
            return await Task.Run(() => _dataCollection.GetJob(jobId));
        }

        //Private Methods
        public async Task<IJobDetail> ScheduleJob(IJobDetail jobDetail)
        {
            jobDetail.Status = JobStatus.Pending;
            await Task.Run(() =>
            {
                _pendingJobs.Enqueue(jobDetail);
                _eventManager.PublishEvent<JobCreatedEventArgs>(new JobCreatedEventArgs { JobDetail = jobDetail });
            });
            return jobDetail;
        }

        private async Task ExecuteJob(IJobDetail jobDetail)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var job = JobBuilder.Create<Crawler>()
                .WithIdentity(jobDetail.Id.ToString(), jobDetail.Name)
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
            if (_pendingJobs.TryDequeue(out var jobDetail)) ExecuteJob(jobDetail).GetAwaiter();
        }

        private void EventManagerJobCompleted(object sender, IJobEventArgs e)
        {
            Task.Run(() =>
            {
                var job = (JobCompletedEventArgs) e;
                _dataCollection.JobCompleted(job.JobDetail, job.Results);
            });
        }

        private void EventManagerJobCreated(object sender, IJobEventArgs e)
        {
            Task.Run(() =>
            {
                var job = (JobCreatedEventArgs) e;
                _dataCollection.JobStarted(job.JobDetail);
            });
        }
    }
}