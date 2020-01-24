using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sciendo.Topper.Contracts.DataTypes;

namespace Sciendo.Topper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private ILogger<ImageController> logger;

        public ImageController(ILogger<ImageController> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public IActionResult UploadImage([FromForm]string artistName, [FromForm]string imageData)
        {
            try
            {
                var imageInfo = new ImageInfo { ImageData = imageData, ArtistName = artistName };
                if (imageInfo==null)
                {
                    logger.LogError("ImageInfo object sent from client is null.");
                    return BadRequest("ImageInfo object is null");
                }

                if (!ModelState.IsValid)
                {
                    logger.LogError("Invalid ImageInfo object sent from client.");
                    return BadRequest("Invalid model object");
                }

                logger.LogInformation("Ok sent from the client: {imageInfo.ArtistName} ====> {imageInfo.ImageData}", imageInfo.ArtistName, imageInfo.ImageData);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside UploadImage action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}