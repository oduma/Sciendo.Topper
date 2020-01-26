using Sciendo.Topper.Domain.Entities;
using Sciendo.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service
{
    public class WebPictureReader : IPictureReader
    {
        private readonly IWebGet<byte[]> webGetter;

        public WebPictureReader(IWebGet<byte[]> webGetter)
        {
            this.webGetter = webGetter;
        }
        public NamedPicture Read(string artistName, string imageData)
        {
            var imageDataParts = imageData.Split(new char[] { '.' });
            return new NamedPicture { Extension = imageDataParts[imageDataParts.Length - 1], Picture = webGetter.Get(new Uri(imageData)), Name = artistName };

        }
    }
}
