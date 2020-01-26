using Microsoft.Extensions.Logging;
using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain.Entities;
using Sciendo.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapImageInfoToNamedPicture : IMap<ImageInfo, NamedPicture>
    {
        private const string urlMarker = "http";
        private readonly ILogger<MapImageInfoToNamedPicture> logger;
        private readonly IPictureReader webPictureReader;
        private readonly IPictureReader dataPictureReader;

        public MapImageInfoToNamedPicture(ILogger<MapImageInfoToNamedPicture> logger, 
            IPictureReader webPictureReader, 
            IPictureReader dataPictureReader)
        {
            this.logger = logger;
            this.webPictureReader = webPictureReader;
            this.dataPictureReader = dataPictureReader;
        }
        public NamedPicture Map(ImageInfo fromItem)
        {
            if (fromItem == null || string.IsNullOrEmpty(fromItem.ArtistName) || string.IsNullOrEmpty(fromItem.ImageData))
                throw new ArgumentNullException(nameof(fromItem));
            if (IsUrlOfImage(fromItem.ImageData))
            {
                //get it from the web
                return webPictureReader.Read(fromItem.ArtistName, fromItem.ImageData);
            }
            //get it from the payload
            return dataPictureReader.Read(fromItem.ArtistName, fromItem.ImageData);
        }



        private bool IsUrlOfImage(string imageData)
        {
            return imageData.StartsWith(urlMarker, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
