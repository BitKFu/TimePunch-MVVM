using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimePunchIntroPlayer.Events
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
