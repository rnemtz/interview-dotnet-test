using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EazeCrawler.Common.Events;
using EazeCrawler.Common.Extensions;
using EazeCrawler.Common.Interfaces;
using EazeCrawler.Common.Models;
using HtmlAgilityPack;
using Quartz;
using IJob = Quartz.IJob;
using IJobDetail = EazeCrawler.Common.Interfaces.IJobDetail;

namespace EazeCrawler.Services
{
    public class Crawler : ICrawler, IJob
    {
        private readonly IEventManager _eventManager;
        private const int MaxLevel = 2;
        public Crawler()
        {
            _eventManager = EventManager.Instance;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var result = new JobResult();
            var visited = new HashSet<Uri>();
            var pending = new ConcurrentQueue<Uri>();
            var jobDetail = GetJobDetail(context);
            var requestedUri = new Uri(jobDetail.Url);

            pending.Enqueue(requestedUri);
            pending.Enqueue(null);

            var level = 1;
            while (pending.Count > 0)
            {
                if (!pending.TryDequeue(out var currentUri)) continue;
                if (currentUri == null && level < MaxLevel)
                {
                    level++;
                    if (pending.Count > 0) pending.Enqueue(null);
                }
                else
                {
                    if (currentUri == null || !visited.Add(currentUri)) continue;
                    var scrapedPage = await GetUriDetails(currentUri);
                    if (scrapedPage==null) continue;
                    
                    result.List.Add(scrapedPage.ToScrapedUrlResult());

                    if (level >= MaxLevel) continue;

                    var links = await GetLinksFromBody(currentUri, scrapedPage);
                    var linkList = links.Where( x=> !visited.Contains(x)).ToList();
                    var tasks = linkList.Select(async link =>
                    {
                        await Task.Run(() =>
                        {
                            pending.Enqueue(link);
                        });
                    });
                    await Task.WhenAll(tasks);
                }
            }

            var args = new JobCompletedEventArgs {JobDetail = jobDetail, Results = result};
            ExecuteOnCompleted(args);
        }

        private static async Task<IScrapedUrl> GetUriDetails(Uri url)
        {
            var body = await GetHttpContent(url);
            if (string.IsNullOrWhiteSpace(body)) return null;

            var document = new HtmlDocument();
            document.LoadHtml(body);

            var urlDetails = new ScrapedUrl
            {
                Body = body,
                Url = url.AbsoluteUri,
                Title = document.DocumentNode.SelectSingleNode("//title")?.InnerText ?? string.Empty,
                Description = document.DocumentNode.SelectSingleNode("//meta[@name='description']")?.InnerText ??
                              string.Empty
            };

            return urlDetails;
        }

        private static async Task<IEnumerable<Uri>> GetLinksFromBody(Uri url, IScrapedUrl scrapedUrl)
        {
            var result = new HashSet<Uri>();
            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(scrapedUrl.Body);

                var tags = document.DocumentNode.SelectNodes("//a")
                    .Select(p => p.GetAttributeValue("href", "not found"))
                    .Where(x => x.Length > 1 
                                && !x.ToLower().StartsWith("javascript") 
                                && !x.ToLower().StartsWith("tel:")
                                && !x.ToLower().StartsWith("mailto:")
                                && x != "not found")
                    .Distinct()
                    .ToList();

                var bag = new ConcurrentBag<Uri>();
                var tasks = tags.Select(async tag => { await Task.Run(() => { bag.Add(new Uri(url, tag)); }); });
                await Task.WhenAll(tasks);
                foreach (var tag in bag) result.Add(tag);
            }
            catch (Exception)
            {
                //Handle Exception gracefully
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