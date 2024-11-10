using Windows.Storage.Streams;

namespace TimePunchIntroPlayerWinUI3.Events.MainPage
{
    public class OpenVideoEvent
    {
        public OpenVideoEvent(IRandomAccessStream fileName, string type, double position)
        {
            Type = type;
            FileStream = fileName;
            Position = position;
        }

        public IRandomAccessStream FileStream { get; set; }

        public double Position { get; set; }

        public string Type { get; set; }

        public bool Result { get; set; }
    }
}
