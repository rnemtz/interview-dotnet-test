﻿using EazeCrawler.Common.Interfaces;

namespace EazeCrawler.Common.Models
{
    public class ScrapedUrlResult: IScrapedUrlResult
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}
