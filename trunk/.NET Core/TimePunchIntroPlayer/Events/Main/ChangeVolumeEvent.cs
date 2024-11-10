namespace TimePunchIntroPlayer.Events
{
    public class ChangeVolumeEvent
    {
        public ChangeVolumeEvent(double volume)
        {
            Volume = volume;
        }

        public double Volume { get; set; }
    }
}
