using System.Windows;

namespace TimePunch.Wpf.Common.UI.Events
{
    public class MessageBoxEvent
    {
        public MessageBoxEvent(string messageText, string messageTitle, MessageBoxButton buttons, MessageBoxImage icon)
        {
            MessageText = messageText;
            MessageTitle = messageTitle;
            Buttons = buttons;
            Icon = icon;
        }

        public MessageBoxImage Icon { get; private set; }

        public MessageBoxButton Buttons { get; private set; }

        public string MessageTitle { get; private set; }

        public string MessageText { get; private set; }

        public MessageBoxResult MessageBoxResult { get; set; }
    }
}
