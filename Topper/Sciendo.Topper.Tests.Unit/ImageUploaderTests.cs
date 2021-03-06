﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Service;
using Sciendo.Topper.Service.Mappers;
using Sciendo.Topper.Store;
using Sciendo.Web;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Tests.Unit
{
    [TestClass]
    public class ImageUploaderTests
    {
        string sampleImage = @"data:image/jpeg;base64,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";

        string sampelImageUrl = @"https://dewolff.nu/wp-content/status_images/pinkpop_.jpg";

        [TestMethod]
        public void ImageService_With_DataPicture_Ok()
        {
            var logger1 = new Mock<ILogger<MapImageInfoToNamedPicture>>();
            //logger1.Setup(m => m.LogInformation(It.IsAny<string>()));
            var logger2 = new Mock<ILogger<WebBytesGet>>();
            //logger2.Setup(m => m.LogInformation(It.IsAny<string>()));
            var logger3 = new Mock<ILogger<PictureRepository>>();
            //logger3.Setup(m => m.LogInformation(It.IsAny<string>()));
            ImageService imageService = new ImageService(
                new MapImageInfoToNamedPicture(logger1.Object,
                new WebPictureReader(new WebBytesGet(logger2.Object)),
                new DataPictureReader()),
                new PictureRepository(logger3.Object,new FileStoreConfig {Root="." }));
            imageService.SaveImage(new ImageInfo
            {
                ArtistName = "Test: Artist!",
                ImageData = sampleImage
            });
        }

        [TestMethod]
        public void ImageService_With_WebPicture_Ok()
        {
            var logger1 = new Mock<ILogger<MapImageInfoToNamedPicture>>();
            //logger1.Setup(m => m.LogInformation(It.IsAny<string>()));
            var logger2 = new Mock<ILogger<WebBytesGet>>();
            //logger2.Setup(m => m.LogInformation(It.IsAny<string>()));
            var logger3 = new Mock<ILogger<PictureRepository>>();
            //logger3.Setup(m => m.LogInformation(It.IsAny<string>()));
            ImageService imageService = new ImageService(
                new MapImageInfoToNamedPicture(logger1.Object,
                new WebPictureReader(new WebBytesGet(logger2.Object)),
                new DataPictureReader()),
                new PictureRepository(logger3.Object, new FileStoreConfig { Root = "." }));
            imageService.SaveImage(new ImageInfo
            {
                ArtistName = "Test: Artist2",
                ImageData = sampelImageUrl
            });
        }

    }
}
