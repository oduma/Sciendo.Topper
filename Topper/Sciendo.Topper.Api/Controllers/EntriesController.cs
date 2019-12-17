using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sciendo.Topper.Contracts;
using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Store;

namespace Sciendo.Topper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly ILogger<EntriesController> logger;
        private readonly IEntriesService service;
        private readonly CosmosDbConfig cosmosDbConfig;

        public EntriesController(ILogger<EntriesController> logger, IEntriesService service, CosmosDbConfig cosmosDbConfig)
        {
            this.logger = logger;
            this.service = service;
            this.cosmosDbConfig = cosmosDbConfig;
            logger.LogInformation("Configuration: ", cosmosDbConfig);
        }

        [HttpGet("[action]/{date}")]
        public ActionResult GetByDate(string date)
        {
            DateTime dateTime;
            #region Sanitation
            try
            {
                string[] dateParts = date.Split('-');
                dateTime = new DateTime(Convert.ToInt32(dateParts[0]), Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[2]));
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Wrong date format! Should be yyyy-mm-dd.");
                return StatusCode(500, "Wrong date format!  Should be yyyy-mm-dd.");
            }
            #endregion
            try
            {
                var results = this.service.GetEntriesByDate(dateTime);
                return Ok(results);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "");
                return StatusCode(500, "Service Exception.");
            }
        }

        [HttpGet("[action]/{year}")]
        public ActionResult GetByYear(string year)
        {
            int forYear;
            #region Sanitation
            try
            {
                
                forYear = Convert.ToInt32(year);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Wrong year format! Should be yyyy.");
                return StatusCode(500, "Wrong date format! Should be yyyy.");
            }
            #endregion

            try
            {
                var results = this.service.GetEntriesWithEvolutionByYear(forYear);
                return Ok(results);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "");
                return StatusCode(500, "Service Exception.");
            }
        }

        [HttpGet("[action]/{year}")]
        public ActionResult GetByYearWithoutEvolution(string year)
        {
            int forYear;
            #region Sanitation
            try
            {

                forYear = Convert.ToInt32(year);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Wrong year format! Should be yyyy.");
                return StatusCode(500, "Wrong date format! Should be yyyy.");
            }
            #endregion

            try
            {
                var results = this.service.GetEntriesByYear(forYear);
                return Ok(results);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "");
                return StatusCode(500, "Service Exception.");
            }
        }

        [HttpGet("[action]")]
        public ActionResult GetTimeLines([FromQuery(Name ="name")]string[] names)
        {
            if(names==null || names.Length==0)
            {
                logger.LogError("No selected names.");
                return StatusCode(500, "No selected names.");
            }
            try
            {
                var results = this.service.GetEntriesTimeLines(names);

                return Ok(results);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "");
                return StatusCode(500, "Service Exception.");
            }
        }
    }
}