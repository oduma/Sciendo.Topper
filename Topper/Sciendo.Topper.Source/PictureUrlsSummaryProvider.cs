using Microsoft.Extensions.Logging;
using Sciendo.MusicStory;
using Sciendo.Topper.Source.DataTypes.MusicStory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Source
{
    public class PictureUrlsSummaryProvider : IPictureUrlsSummaryProvider
    {
        private readonly ILogger<PictureUrlsSummaryProvider> logger;
        private readonly IContentProvider<PictureUrlsSummary> contentProvider;

        private const string Subject = "artist";

        private readonly ActionType ActionType = ActionType.GetById;

        private const string Attribute = "pictures";

        public PictureUrlsSummaryProvider(ILogger<PictureUrlsSummaryProvider> logger,IContentProvider<PictureUrlsSummary> contentProvider)
        {
            this.logger = logger;
            this.contentProvider = contentProvider;
        }
        public PictureUrlsSummary Get(long artistId)
        {
            logger.LogDebug("Getting pcitures for artist with id {artistId}", artistId);
            return contentProvider.GetContent(Subject, ActionType, "source=Music%20Story", artistId, Attribute);
        }
    }
}
