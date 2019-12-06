using System;

namespace Sciendo.Topper.Contracts.DataTypes
{
    public class EntryTimeLine: EntryBase
    {
        public PostionAtDate[] PositionAtDates { get; set; }
    }
}