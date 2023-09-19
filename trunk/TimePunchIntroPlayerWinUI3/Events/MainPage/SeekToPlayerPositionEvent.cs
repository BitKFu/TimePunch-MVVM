namespace TimePunchIntroPlayerWinUI3.Events.MainPage
{
    public class SeekToPlayerPositionEvent
    {
        public SeekToPlayerPositionEvent(double position)
        {
            Position = position;
        }

        public double Position { get; set; }
    }
}
