using System;
using System.Collections.Generic;
using EazeCrawler.Common.Interfaces;
using EazeCrawler.Common.Models;
using Moq;
using Xunit;

namespace EazeCrawler.Tests
{
    public class EazeCrawlerUnitTests
    {
        [Fact]
        public void PostJob()
        {
        }

        #region Collections

        [Fact]
        public void NewJobCollectionTest()
        {
            var mData = new Mock<ICollection>();
            var jDetail = GetNewJobDetail();
            mData.Setup(d => d.GetJob(jDetail.Id)).Returns(GetJobDetail(jDetail.Id));
            var data = mData.Object;

            var jResult = data.GetJob(jDetail.Id);

            Assert.NotNull(jResult);
            Assert.IsAssignableFrom<IJob>(jResult);
            Assert.Equal("Google", jResult.JobDetail.Name);
            Assert.Equal(0,jResult.Results.List.Count);
        }
        private IJob GetJobDetail(Guid id)
        {
           return new Job {JobDetail = GetNewJobDetail(id) };
        }

        [Fact]
        public void GetRunningJobsTest()
        {
            var mData = new Mock<ICollection>();
            mData.Setup(d => d.GetRunningJobs()).Returns(GetRunningJobs);
            var data = mData.Object;
            var runningJobs = data.GetRunningJobs();

            Assert.Equal(1, runningJobs.Count);
            Assert.Equal("Google", runningJobs[0].Name);
            Assert.Equal(JobStatus.Running, runningJobs[0].Status);
        }
        private IList<IJobDetail> GetRunningJobs()
        {
            return new List<IJobDetail> {GetNewJobDetail(status: JobStatus.Running)};
        }

        [Fact]
        public void GetCompletedJobsTests()
        {
            var mData = new Mock<ICollection>();
            var jDetail = GetNewJobDetail();
            mData.Setup(d => d.GetResults(jDetail)).Returns(GetResults);
            mData.Setup(d => d.GetResults()).Returns(GetScrapedUrlResults);
            var data = mData.Object;
            var jResult = data.GetResults(jDetail);

            Assert.Equal(2, jResult.List.Count);
            Assert.Equal("Google Search", jResult.List[0].Title);
            Assert.Equal("Bing Search", jResult.List[1].Title);

            var sResult = data.GetResults();
            Assert.Equal(2, jResult.List.Count);
            Assert.Equal("Bing Search", sResult[0].Title);
            Assert.Equal("Google Search", sResult[1].Title);
        }

        private IJobResult GetResults()
        {
            var jResult = new JobResult();
            jResult.List.Add(new ScrapedUrlResult { Description = "Google", Title = "Google Search", Url = "https://www.google.com" });
            jResult.List.Add(new ScrapedUrlResult { Description = "Bing", Title = "Bing Search", Url = "https://www.bing.com" });
            return jResult;
        }

        private IList<IScrapedUrlResult> GetScrapedUrlResults()
        {
            var list = new List<IScrapedUrlResult>
            {
                new ScrapedUrlResult {Description = "Bing", Title = "Bing Search", Url = "https://www.bing.com"},
                new ScrapedUrlResult {Description = "Google", Title = "Google Search", Url = "https://www.google.com"}
            };
            return list;
        }

        private IJobDetail GetNewJobDetail(Guid id = default(Guid), JobStatus status = JobStatus.Pending)
        {
            var job = new JobDetail
            {
                Id = id == default(Guid) ? Guid.NewGuid() : id,
                CreatedAt = DateTime.UtcNow,
                Name = "Google",
                Status = status,
                Url = "https://www.google.com"
            };

            return job;
        }

        #endregion
    }
}
