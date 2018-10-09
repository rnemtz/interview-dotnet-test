using System;
using System.Threading.Tasks;
using EazeCrawler.Common.Interfaces;
using EazeCrawler.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace EazeCrawler.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Scraper Controller
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("api/v1/scraper")]
    public class ScraperController : ControllerBase
    {
        private readonly IScheduler _schedulerService;

        /// <inheritdoc />
        public ScraperController(IScheduler schedulerService)
        {
            _schedulerService = schedulerService;
        }

        /// <summary>
        /// Post a new scrap job
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/scraper
        ///     {
        ///        "name": "Google",
        ///        "Url": "https://www.google.com"
        ///     }
        ///
        /// </remarks>
        /// <param name="jobDetail">Job Detail</param>
        /// <returns>Job Detail received</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
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

        /// <summary>
        /// Get Jobs Results
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/scraper/
        ///
        ///  </remarks>
        /// <returns>List of Scraped urls</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetResults()
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

        /// <summary>
        /// Delete Current Results
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/v1/scraper/
        ///
        ///  </remarks>
        /// <returns>Delete Result</returns>
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
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

        /// <summary>
        /// Get Job Information with results if available
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/scraper/{id}
        ///
        ///  </remarks>
        /// <param name="id">Job Id</param>
        /// <returns>Job Information</returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
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

        /// <summary>
        /// Get job results
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/scraper/{id}
        ///
        ///  </remarks>
        /// <param name="id">Job Id</param>
        /// <returns>List of Scraped urls for the job</returns>
        [HttpGet]
        [Route("{id}/results")]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
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