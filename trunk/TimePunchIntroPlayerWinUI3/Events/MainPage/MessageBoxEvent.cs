using Microsoft.UI.Xaml.Controls;

namespace TimePunchIntroPlayerWinUI3.Events.MainPage
{
    public class ContentDialogEvent
    {
        public ContentDialogEvent(string messageText, string messageTitle, string buttonText)
        {
            MessageText = messageText;
            MessageTitle = messageTitle;
            ButtonText = buttonText;
        }

        public string ButtonText { get; set; }

        public string MessageTitle { get; private set; }

        public string MessageText { get; private set; }

        public ContentDialogResult MessageBoxResult { get; set; }
    }
}
