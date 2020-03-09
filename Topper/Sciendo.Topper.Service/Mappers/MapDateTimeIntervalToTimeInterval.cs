using Sciendo.Topper.Contracts.DataTypes;
using Sciendo.Topper.Domain.Entities;

namespace Sciendo.Topper.Service.Mappers
{
    public class MapDateTimeIntervalToTimeInterval : IMap<DateTimeInterval, TimeInterval>
    {
        public TimeInterval Map(DateTimeInterval fromItem)
        {
            return new TimeInterval
            {
                FromDate = fromItem.FromDate.ToString("yyyy-MM-dd"),
                ToDate = fromItem.ToDate.ToString("yyyy-MM-dd")
            };
        }
    }
}
