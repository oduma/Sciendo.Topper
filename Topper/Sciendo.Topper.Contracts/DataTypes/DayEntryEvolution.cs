namespace Sciendo.Topper.Contracts.DataTypes
{
    public class DayEntryEvolution: OverallEntryEvolution
    {
        public Position CurrentDayPosition { get; set; }

        public Position PreviousDayPosition { get; set; }

        public string Date { get; set; }
    }
}
