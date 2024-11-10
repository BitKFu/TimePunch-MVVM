namespace TimePunchIntroPlayer.Events
{
    public class OpenVideoEvent
    {
        public OpenVideoEvent(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; }

        public bool Result { get; set; }
    }
}
