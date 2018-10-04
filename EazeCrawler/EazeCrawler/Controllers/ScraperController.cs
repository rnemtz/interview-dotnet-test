using System;
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
        // GET api/v1/scraper?id=<id>
        [HttpGet]
        public async Task<IActionResult> Get(Guid id = default(Guid))
        {
            try
            {
                if (id == default(Guid))
                {
                    var results = await _schedulerService.GetResults();
                    return Ok(results);
                }

                var job = await _schedulerService.GetJobStatus(id);
                if (job == null) return NotFound(id);

                return Ok(job);

            }
            catch (Exception exception)
            {
                return StatusCode(500, exception);
            }
        }

        // GET api/v1/scraper/{id}/results
        [HttpGet]
        [Route("{id}/results")]
        public async Task<IActionResult> GetResults(Guid id)
        {
            if (id == default(Guid)) return BadRequest();
            try
            {
                var results = await _schedulerService.GetResults(id);
                if (results == null) return NoContent();
                return Ok(results);
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception);
            }
        }

        // POST api/v1/scraper
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]JobDetail jobDetail)
        {
            if (string.IsNullOrWhiteSpace(jobDetail.Name) || string.IsNullOrWhiteSpace(jobDetail.Url)) return BadRequest();
            try
            {
                if (jobDetail.Id == default(Guid)) jobDetail.Id = Guid.NewGuid();
                
                return Ok(await _schedulerService.ScheduleJob(jobDetail));
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception);
            }
        }
    }
}