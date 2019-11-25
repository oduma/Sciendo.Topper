using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Contracts.DataTypes
{
    public class DayEntryEvolution: OverallEntryEvolution
    {
        public Position CurrentDayPosition { get; set; }

        public Position PreviousDayPosition { get; set; }
    }
}
