using Sciendo.Topper.Domain.Entities;
using Sciendo.Topper.Source;
using Sciendo.Topper.Store;
using System.IO;

namespace Sciendo.Topper.Service
{
    public class EntryArtistImageProvider : IEntryArtistImageProvider
    {
        private readonly IFileRepository<NamedPicture> pictureRepository;
        private readonly FileStoreConfig fileStoreConfig;
        private readonly PathToUrlConverterConfig pathToUrlConverterConfig;

        public EntryArtistImageProvider(IFileRepository<NamedPicture> pictureRepository,
            FileStoreConfig fileStoreConfig,
            PathToUrlConverterConfig pathToUrlConverterConfig)
        {
            this.pictureRepository = pictureRepository;
            this.fileStoreConfig = fileStoreConfig;
            this.pathToUrlConverterConfig = pathToUrlConverterConfig;
        }
        public string GetPictureUrl(string name)
        {
            var picturePath = GetPicturePath(name);
            if(picturePath.StartsWith(pathToUrlConverterConfig.RootUrlFullPath))
            {
                return picturePath.Replace(pathToUrlConverterConfig.RootUrlFullPath, pathToUrlConverterConfig.RootUrl).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
            return null;
        }

        private string GetPicturePath(string name)
        {
            var fullPath = pictureRepository.GetItem(name);
            if (string.IsNullOrEmpty(fullPath))
            {
                return fileStoreConfig.DefaultPlaceholderPicture;
            }
            return fullPath;
        }
    }
}
