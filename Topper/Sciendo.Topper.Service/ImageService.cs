using Sciendo.Topper.Contracts;
using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain.Entities;
using Sciendo.Topper.Service.Mappers;
using Sciendo.Topper.Store;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service
{
    public class ImageService : IImageService
    {
        private readonly IMap<ImageInfo, NamedPicture> mapToNamedPicture;
        private readonly IFileRepository<NamedPicture> pictureRepository;

        public ImageService(IMap<ImageInfo,NamedPicture> mapToNamedPicture, 
            IFileRepository<NamedPicture> pictureRepository)
        {
            this.mapToNamedPicture = mapToNamedPicture;
            this.pictureRepository = pictureRepository;
        }
        public void SaveImage(ImageInfo imageInfo)
        {
            var namedPicture = mapToNamedPicture.Map(imageInfo);
            pictureRepository.CreateItem(namedPicture);
        }
    }
}
