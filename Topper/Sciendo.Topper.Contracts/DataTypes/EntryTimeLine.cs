using System;

namespace Sciendo.Topper.Contracts.DataTypes
{
    public class EntryTimeLine: EntryBase
    {
        public EntryTimeLine(PositionAtDate[] positionAtDates, string name, string pictureUrl):base(name,pictureUrl)
        {
            PositionAtDates = positionAtDates;
        }
        public PositionAtDate[] PositionAtDates { get; private set; }
    }
}