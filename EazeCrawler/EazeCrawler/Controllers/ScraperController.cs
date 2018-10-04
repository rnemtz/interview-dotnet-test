using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EazeCrawler.Common.Interfaces;
using EazeCrawler.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace EazeCrawler.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ScraperController : ControllerBase
    {
        private readonly IScheduler _schedulerService;

        public ScraperController(IScheduler schedulerService)
        {
            _schedulerService = schedulerService;
        }

        // GET api/v1/scraper
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IJobDetail>>> Get()
        {
            var job = new JobDetail {Id = Guid.NewGuid(), Name = "Test"};
            var result = await _schedulerService.ScheduleJob(job);
            return null;
        }

        // POST api/v1/scraper
        [HttpPost]
        public async Task<IActionResult> Post(IJobDetail jobDetail)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
             //await _schedulerService.ExecuteJob(jobDetail);
            return CreatedAtAction("", "", new {JobDetail = jobDetail}, jobDetail);
        }
    }
}