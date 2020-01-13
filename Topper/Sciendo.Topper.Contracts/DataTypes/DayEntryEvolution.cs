namespace Sciendo.Topper.Contracts.DataTypes
{
    public class DayEntryEvolution: OverallEntryEvolution
    {
        public DayEntryEvolution(Position currentDayPosition, Position previousDayPosition, string date, 
            Position previousDayOverallPosition, Position currentOverallPosition, string name, string pictureUrl)
            :base(previousDayOverallPosition, currentOverallPosition, name, pictureUrl)
        {
            CurrentDayPosition = currentDayPosition;
            PreviousDayPosition = previousDayPosition;
            Date = date;
        }
        public Position CurrentDayPosition { get; private set; }

        public Position PreviousDayPosition { get; private set; }

        public string Date { get; private set; }
    }
}
