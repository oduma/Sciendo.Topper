using Microsoft.Extensions.Logging;
using Sciendo.Last.Fm;
using Sciendo.Topper.Contracts;
using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sciendo.Topper.Service
{
    public class AdminService : IAdminService
    {
        private readonly ILogger<AdminService> logger;
        private readonly LastFmConfig lastFmConfig;
        private readonly FileStoreConfig fileStoreConfig;
        private readonly PathToUrlConverterConfig pathToUrlConverterConfig;
        private readonly IRepository<TopItem> repository;

        public AdminService(
            ILogger<AdminService> logger,
            LastFmConfig lastFmConfig, 
            FileStoreConfig fileStoreConfig,
            PathToUrlConverterConfig pathToUrlConverterConfig,
            IRepository<TopItem> repository)
        {
            this.logger = logger;
            this.lastFmConfig = lastFmConfig;
            this.fileStoreConfig = fileStoreConfig;
            this.pathToUrlConverterConfig = pathToUrlConverterConfig;
            this.repository = repository;
        }

        public string[] GetHistoryYears()
        {
            return repository
                .GetAllItemsAsync()
                .Result
                .Select(i => i.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToArray();

        }

        public ServerConfig GetServerConfig()
        {
            return new ServerConfig
            {
                LastFmRootUrl = "https://www.last.fm/music/",
                LastFmAppKey = lastFmConfig.ApiKey,
                RelativeDefaultImagePath = fileStoreConfig
                    .DefaultPlaceholderPicture
                    .Replace(pathToUrlConverterConfig.RootUrlFullPath, "")
                    .Replace("\\", "/")
            };
        }
    }
}
