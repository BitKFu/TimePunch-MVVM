namespace TimePunchIntroPlayerWinUI3.Events.MainPage
{
    public class SaveFileDialogEvent
    {
        public SaveFileDialogEvent(string fileName, string defaultExt, string filter)
        {
            FileName = fileName;
            DefaultExt = defaultExt;
            Filter = filter;
        }

        public string FileName { get; set; }

        public string DefaultExt { get; private set; }

        public string Filter { get; private set; }

        public bool? Result { get; set; }
        
        public int FilterIndex { get; set; }
    }
}
