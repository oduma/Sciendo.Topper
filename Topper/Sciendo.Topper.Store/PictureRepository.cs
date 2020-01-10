using Microsoft.Extensions.Logging;
using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.Topper.Store
{
    public class PictureRepository : IFileRepository<NamedPicture>
    {
        private readonly ILogger<PictureRepository> logger;
        private readonly FileStoreConfig fileStoreConfig;

        public PictureRepository(ILogger<PictureRepository> logger, FileStoreConfig fileStoreConfig)
        {
            this.logger = logger;
            this.fileStoreConfig = fileStoreConfig;
        }

        public string CreateItem(NamedPicture item)
        {
            var fileName = SanitizeNameForPath(item.Name);
            string fullPath = $"{fileStoreConfig.Root}{Path.DirectorySeparatorChar}{fileName[0]}{Path.DirectorySeparatorChar}{fileName}.{item.Extension}";
            using(var fs = File.Create(fullPath))
            {
                fs.Write(item.Picture, 0, item.Picture.Length);
                fs.Flush();
                fs.Close();
            }
            return fullPath;
        }

        private static string SanitizeNameForPath(string inputString)
        {
            var invalidPathCharacters = Path.GetInvalidPathChars();
            StringBuilder result = new StringBuilder();
            foreach (char pathCharacter in inputString.ToLower())
            {
                if (invalidPathCharacters.Any((c) => c == pathCharacter))
                    result.Append("_");
                else
                    result.Append(pathCharacter);
            }
            return result.ToString();
        }

        public string GetItem(string artistName)
        {
            var fileName = SanitizeNameForPath(artistName);
            string path = $"{fileStoreConfig.Root}{Path.DirectorySeparatorChar}{fileName[0]}";
            var fullPath=Directory.GetFiles(path, $"{fileName}.*").FirstOrDefault();
            if (fullPath == null)
                return null;
            return fullPath;
        }
    }
}
