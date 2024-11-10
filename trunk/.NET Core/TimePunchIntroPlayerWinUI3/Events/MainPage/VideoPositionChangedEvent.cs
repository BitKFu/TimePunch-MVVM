using System;

namespace TimePunchIntroPlayerWinUI3.Events.MainPage
{
    public class VideoPositionChangedEvent
    {
        public VideoPositionChangedEvent(TimeSpan position)
        {
            Position = position;
        }

        public TimeSpan Position { get; set; }
    }
}
