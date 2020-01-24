using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapImageInfoToNamedPicture : IMap<ImageInfo, NamedPicture>
    {
        public NamedPicture Map(ImageInfo fromItem)
        {
            if (fromItem == null || string.IsNullOrEmpty(fromItem.ArtistName) || string.IsNullOrEmpty(fromItem.ImageData))
                throw new ArgumentNullException(nameof(fromItem));
            return new NamedPicture { 
                Name = fromItem.ArtistName, 
                Picture = GetPicture(fromItem.ImageData), 
                Extension = GetExtension(fromItem.ImageData) };
        }

        private string GetExtension(string imageData)
        {
            throw new NotImplementedException();
        }

        private byte[] GetPicture(string imageData)
        {
            if(IsUrlOfImage(imageData))
            {
                //get it from the web
                return null;
            }
            //get it from the payload
            return null;
        }

        private bool IsUrlOfImage(string imageData)
        {
            throw new NotImplementedException();
        }
    }
}
