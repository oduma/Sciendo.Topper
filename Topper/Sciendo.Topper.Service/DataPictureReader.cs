using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service
{
    public class DataPictureReader : IPictureReader
    {
        private const char metadataSeparator = ',';
        private const char metadataPartsSeparator = ';';
        private const char metadataMimeTypeSeparator = ':';
        private const char mimeTypePartsSeparator = '/';
        public NamedPicture Read(string artistName, string imageData)
        {
            var metadata = imageData.Split(new char[] { metadataSeparator })[0];
            var image = imageData.Replace(metadata + metadataSeparator, "");
            return new NamedPicture
            {
                Extension = metadata
                .Split(new char[] { metadataPartsSeparator })[0]
                .Split(new char[] { metadataMimeTypeSeparator })[1]
                .Split(new char[] { mimeTypePartsSeparator })[1],
                Picture = Convert.FromBase64String(image),
                Name = artistName
            };
            
        }
    }
}
