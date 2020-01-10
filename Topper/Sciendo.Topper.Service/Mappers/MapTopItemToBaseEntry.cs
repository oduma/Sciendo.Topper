using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using System;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemToBaseEntry : IMap<TopItem, EntryBase>
    {
        private readonly IEntryArtistImageProvider entryArtistImageProvider;

        public MapTopItemToBaseEntry(IEntryArtistImageProvider entryArtistImageProvider)
        {
            this.entryArtistImageProvider = entryArtistImageProvider;
        }
        public EntryBase Map(TopItem fromItem)
        {
            if (fromItem == null || string.IsNullOrEmpty(fromItem.Name))
                throw new ArgumentNullException(nameof(fromItem));
            if (entryArtistImageProvider == null)
                throw new Exception("EntryArtistImageProvider is null.");
            EntryBase result = new EntryBase { Name = fromItem.Name };
            result.PictureUrl = entryArtistImageProvider.GetPictureUrl(fromItem.Name);
            return result;
        }

    }
}
