using Microsoft.Extensions.Logging;
using Sciendo.Last.Fm;
using Sciendo.Topper.Contracts;
using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Store;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service
{
    public class AdminService : IAdminService
    {
        private readonly ILogger<AdminService> logger;
        private readonly LastFmConfig lastFmConfig;
        private readonly FileStoreConfig fileStoreConfig;
        private readonly PathToUrlConverterConfig pathToUrlConverterConfig;

        public AdminService(
            ILogger<AdminService> logger,
            LastFmConfig lastFmConfig, 
            FileStoreConfig fileStoreConfig,
            PathToUrlConverterConfig pathToUrlConverterConfig)
        {
            this.logger = logger;
            this.lastFmConfig = lastFmConfig;
            this.fileStoreConfig = fileStoreConfig;
            this.pathToUrlConverterConfig = pathToUrlConverterConfig;
        }

        public int[] GetHistoryYears()
        {
            return new[]{ 2018, 2019};
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
