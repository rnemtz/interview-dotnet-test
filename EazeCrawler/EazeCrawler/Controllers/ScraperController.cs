using System;
using System.Threading.Tasks;
using EazeCrawler.Common.Interfaces;
using EazeCrawler.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace EazeCrawler.Controllers
{
    [ApiController]
    [Route("api/v1/scraper")]
    public class ScraperController : ControllerBase
    {
        private readonly IScheduler _schedulerService;

        public ScraperController(IScheduler schedulerService)
        {
            _schedulerService = schedulerService;
        }

        // POST api/v1/scraper/
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

        // GET api/v1/scraper/
        [HttpGet]
        public async Task<IActionResult> GetResults(Guid id)
        {
            try
            {
                return Ok(await _schedulerService.GetResults());
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception);
            }
        }

        // DELETE api/v1/scraper/
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            try
            {
                return Ok(await _schedulerService.DeleteResults());
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception);
            }
        }

        //GET api/v1/scraper/{id

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetJobInformation(Guid id)
        {
            if (id == default(Guid)) return BadRequest();
            try
            {
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
        public async Task<IActionResult> GetJobResults(Guid id)
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
    }
}