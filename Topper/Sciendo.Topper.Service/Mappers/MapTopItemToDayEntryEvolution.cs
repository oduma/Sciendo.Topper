using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using Sciendo.Topper.Domain.Entities;
using System;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemToDayEntryEvolution : IMapAggregateTwoEntries<TopItem, TopItem, DayEntryEvolution>
    {
        private readonly IMap<TopItem, Positions> mapToPositions;
        private readonly IEntryArtistImageProvider entryArtistImageProvider;

        public MapTopItemToDayEntryEvolution(IMap<TopItem, Positions> mapToPositions, IEntryArtistImageProvider entryArtistImageProvider) 
        {
            this.mapToPositions = mapToPositions;
            this.entryArtistImageProvider = entryArtistImageProvider;
        }

        public new DayEntryEvolution Map(TopItem currentDailyItem, TopItem previousDailyItem)
        {
            if (mapToPositions == null)
                throw new Exception("MapToPositions is mandatory");
            if (entryArtistImageProvider == null)
                throw new Exception("ImageProvider is mandatory");
            if (currentDailyItem == null || string.IsNullOrEmpty(currentDailyItem.Name))
                throw new ArgumentNullException(nameof(currentDailyItem));
            if (previousDailyItem != null 
                && (string.IsNullOrEmpty(previousDailyItem.Name) 
                || previousDailyItem.Name!=currentDailyItem.Name))
                throw new Exception("Cross Items Evolution not supported!");
            var currentDailyItemPositions = mapToPositions.Map(currentDailyItem);

            currentDailyItemPositions.DailyPosition.Rank = 
                (currentDailyItemPositions.DailyPosition.Rank == 0) ? 9999 : currentDailyItemPositions.DailyPosition.Rank;

            var previousDailyItemPositions = (previousDailyItem != null) ? mapToPositions.Map(previousDailyItem) : null;
            if(previousDailyItemPositions!=null)
                previousDailyItemPositions.DailyPosition.Rank=
                                    (previousDailyItemPositions.DailyPosition.Rank == 0) ? 9999 : previousDailyItemPositions.DailyPosition.Rank;

            return new DayEntryEvolution(currentDailyItemPositions.DailyPosition,
                (previousDailyItemPositions!=null)?previousDailyItemPositions.DailyPosition:null,
                currentDailyItem.Date.Date.ToString("yyyy-MM-dd"),
                (previousDailyItemPositions != null) ? previousDailyItemPositions.OverallPosition : null,
                currentDailyItemPositions.OverallPosition,
                currentDailyItem.Name,
                entryArtistImageProvider.GetPictureUrl(currentDailyItem.Name));
        }

    }
}
