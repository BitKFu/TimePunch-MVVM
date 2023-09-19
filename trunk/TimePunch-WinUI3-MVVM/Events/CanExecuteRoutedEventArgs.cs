using Microsoft.UI.Xaml;
using System.Windows.Input;

namespace TimePunch.MVVM.Events
{
    public sealed class CanExecuteRoutedEventArgs : RoutedEventArgs
    {
        public CanExecuteRoutedEventArgs(ICommand command, object parameter)
        {
            Command = command;
            Parameter = parameter;
        }

        public ICommand Command { get; }
        public object Parameter { get; }
        public bool CanExecute { get; set; }
        public bool ContinueRouting { get; set; }
    }
}
