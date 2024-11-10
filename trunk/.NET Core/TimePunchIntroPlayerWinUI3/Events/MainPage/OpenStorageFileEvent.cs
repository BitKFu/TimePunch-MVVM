using Windows.Storage.Streams;

namespace TimePunchIntroPlayerWinUI3.Events.MainPage
{
    public class OpenStorageFileEvent
    {
        public OpenStorageFileEvent(string fileExtension, string filter)
        {
            FileExtension = fileExtension;
            Filter = filter;
        }


        public string ContentType { get; set; }

        public IRandomAccessStream FileStream { get; set; }

        public string FileExtension { get; private set; }

        public string Filter { get; private set; }

        public bool? Result { get; set; }

        public int FilterIndex { get; set; }
    }
}
