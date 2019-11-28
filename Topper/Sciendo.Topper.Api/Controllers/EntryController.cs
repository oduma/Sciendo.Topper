using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntryController : ControllerBase
    {
        private readonly IEntriesService service;

        public EntryController(IEntriesService service)
        {
            this.service = service;
        }


    }
}