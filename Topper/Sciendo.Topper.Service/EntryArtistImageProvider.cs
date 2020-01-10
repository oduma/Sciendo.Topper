using Sciendo.Topper.Domain.Entities;
using Sciendo.Topper.Source;
using Sciendo.Topper.Store;
using System.IO;

namespace Sciendo.Topper.Service
{
    public class EntryArtistImageProvider : IEntryArtistImageProvider
    {
        private readonly IFileRepository<NamedPicture> pictureRepository;
        private readonly IArtistImageProvider artistImageProvider;
        private readonly FileStoreConfig fileStoreConfig;
        private readonly PathToUrlConverterConfig pathToUrlConverterConfig;

        public EntryArtistImageProvider(IFileRepository<NamedPicture> pictureRepository,
            IArtistImageProvider artistImageProvider, 
            FileStoreConfig fileStoreConfig,
            PathToUrlConverterConfig pathToUrlConverterConfig)
        {
            this.pictureRepository = pictureRepository;
            this.artistImageProvider = artistImageProvider;
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
                var namedPicture = artistImageProvider.GetImage(name);
                if (namedPicture == null)
                {
                    return fileStoreConfig.DefaultPlaceholderPicture;
                }
                return pictureRepository.CreateItem(namedPicture);
            }
            return fullPath;
        }
    }
}
