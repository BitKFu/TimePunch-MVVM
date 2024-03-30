#if NETSTANDARD || NET

using System.Windows.Input;

#if NET
using Microsoft.UI.Xaml;
#endif

namespace TimePunch.MVVM.Events
{
    public sealed class ExecutedRoutedEventArgs
#if NET
        : RoutedEventArgs
#endif
    {
        public ExecutedRoutedEventArgs(ICommand command, object parameter)
        {
            Command = command;
            Parameter = parameter;
        }

        public ICommand Command { get; }

        public object Parameter { get; }
    }
}

#endif