#if NETSTANDARD || NET

using System.Windows.Input;

#if NET
using Microsoft.UI.Xaml;
#endif

namespace TimePunch.MVVM.Events
{
    public sealed class CanExecuteRoutedEventArgs
#if NET
        : RoutedEventArgs
#endif
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

#endif
