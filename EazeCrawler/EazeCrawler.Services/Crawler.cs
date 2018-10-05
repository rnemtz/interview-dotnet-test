using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EazeCrawler.Common.Events;
using EazeCrawler.Common.Interfaces;
using EazeCrawler.Common.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Quartz;
using IJob = Quartz.IJob;
using IJobDetail = EazeCrawler.Common.Interfaces.IJobDetail;

namespace EazeCrawler.Services
{
    [DisallowConcurrentExecution]
    public class Crawler : ICrawler, IJob
    {
        private readonly IEventManager _eventManager;
        private const int MaxLevel = 3;
        public Crawler()
        {
            _eventManager = EventManager.Instance;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var result = new JobResult();
            var visited = new HashSet<Uri>();
            var pending = new Queue<Uri>();
            var jobDetail = GetJobDetail(context);
            var requestedUri = new Uri(jobDetail.Url);

            pending.Enqueue(requestedUri);
            pending.Enqueue(null);

            var level = 0;
            while (pending.Count > 0)
            {
                var current = pending.Dequeue();
                if (current == null)
                {
                    level++;
                    if (pending.Count > 0) pending.Enqueue(null);
                }
                else
                {
                    if (!visited.Add(current)) continue;
                    //get page details
                    //add to results
                    if (level >= MaxLevel) continue;
                    //get page links
                    var list = await GetListFromUri(current);
                    foreach (var uri in list)
                    {
                        if (visited.Contains(uri)) continue;
                        pending.Enqueue(uri);
                    }
                }

                
            }
            visited.Remove(requestedUri);
            foreach (var uri in visited)
            {
                //result.UrList.Add(uri.AbsoluteUri);
            }

            var args = new JobCompletedEventArgs {JobDetail = jobDetail, Results = result};
            ExecuteOnCompleted(args);
        }

        private static async Task<IScrapedUrl> GetUrlDetails(string body)
        {
            var document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(body);

            var title = document.DocumentNode.SelectSingleNode("//title").InnerText;
            var description = document.DocumentNode.SelectSingleNode("//meta[@name='description']").InnerText;
            var keywords = document.DocumentNode.SelectSingleNode("//meta[@name='keywords']").InnerText;


            return null;
        }


        private static async Task<IEnumerable<Uri>> GetListFromUri(Uri url)
        {
            var result = new HashSet<Uri>();
            try
            {
                var body = await GetHttpContent(url);
                var document = new HtmlAgilityPack.HtmlDocument();

                document.LoadHtml(body);

                var tags = document.DocumentNode.SelectNodes("//a")
                    .Select(p => p.GetAttributeValue("href", "not found"))
                    .ToList();

                foreach (var tag in tags)
                {
                    if (tag.Length == 1) continue;

                    var currentTag = tag;
                    if (tag.StartsWith("//")) currentTag = $"{url.Scheme}:{tag.Substring(2, tag.Length - 1)}";
                    else if (tag.StartsWith("/")) currentTag = $"{url.AbsoluteUri}{tag.Substring(1, tag.Length - 1)}";
                    else if (tag.StartsWith("#")) currentTag = $"{url.AbsoluteUri}{tag}";
                    else if (tag.StartsWith("?")) currentTag = $"{url.AbsoluteUri.Substring(0, url.AbsoluteUri.Length - 2)}{tag}";
                    else if (tag.ToLower().StartsWith("javascript") || tag.StartsWith("not found")) continue;

                    result.Add(new Uri(currentTag));
                }
            }
            catch (Exception)
            {
                //Handle Exception gracefully
                //Log perhaps if needed.
            }
            return result;
        }

        private static async Task<string> GetHttpContent(Uri url)
        {
            string responseBody;
            try
            {
                using (var client = new HttpClient())
                {
                    responseBody = await client.GetStringAsync(url);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return responseBody;
        } 

        private void ExecuteOnCompleted(Common.Interfaces.IJob args)
        {
            _eventManager.PublishEvent<JobCompletedEventArgs>(args);
        }

        private static IJobDetail GetJobDetail(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            var jobDetail = new JobDetail
            {
                Id = Guid.Parse(dataMap.GetString("Id")),
                Name = dataMap.GetString("Name"),
                Url = dataMap.GetString("Url"),
                Status = (JobStatus) dataMap.GetIntValue("Status"),
                CreatedAt = DateTime.Parse(dataMap.GetString("CreatedAt")),
                CompletedAt = DateTime.Parse(dataMap.GetString("CompletedAt"))
            };

            return jobDetail;
        }
    }
}