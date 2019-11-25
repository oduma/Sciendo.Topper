using System;

namespace Sciendo.Topper.Contracts.DataTypes
{
    public class DatedEntry: EntryBase
    {
        public DateTime Date { get; set; }

        public Position Position { get; set; }

    }
}