using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain;
using System;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapTopItemToBaseEntry : IMap<TopItem, EntryBase>
    {
        public EntryBase Map(TopItem fromItem)
        {
            if (fromItem == null || string.IsNullOrEmpty(fromItem.Name))
                throw new ArgumentNullException(nameof(fromItem));
            return new EntryBase { Name = fromItem.Name };
        }
    }
}
