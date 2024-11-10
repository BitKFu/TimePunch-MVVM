namespace TimePunch.TpClientGui.Controls.Events
{
    public class OpenFileDialogEvent
    {
        public OpenFileDialogEvent(string fileName, string defaultExt, string filter)
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
