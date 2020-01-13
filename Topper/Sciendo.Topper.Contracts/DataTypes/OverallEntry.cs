using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Contracts.DataTypes
{
    public class OverallEntry : EntryBase
    {
        public OverallEntry(Position currentOverallPosition, string name, string pictureUrl) : base(name, pictureUrl)
        {
            CurrentOverallPosition = currentOverallPosition;
        }
        public Position CurrentOverallPosition { get; private set; }
    }
}
