using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> logger;
        private IAdminService _adminService;

        public AdminController(ILogger<AdminController> logger, IAdminService adminService)
        {
            this.logger = logger;
            _adminService = adminService;
        }
        [HttpGet]
        public IActionResult GetAllHistoryYears()
        {
            try
            {
                logger.LogInformation("Get All History Years!");
                var years = _adminService.GetHistoryYears();
                return Ok(years);
            }
            catch(Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}