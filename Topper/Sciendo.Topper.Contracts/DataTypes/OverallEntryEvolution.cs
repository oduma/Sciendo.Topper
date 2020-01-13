namespace Sciendo.Topper.Contracts.DataTypes
{
    public class OverallEntryEvolution: OverallEntry
    {
        public OverallEntryEvolution(Position previousDayOverallPosition, Position currentOverallPosition, string name, string pictureUrl)
            :base(currentOverallPosition,name,pictureUrl)
        {
            PreviousDayOverallPosition = previousDayOverallPosition;
        }
        public Position PreviousDayOverallPosition { get; set; }

    }
}
