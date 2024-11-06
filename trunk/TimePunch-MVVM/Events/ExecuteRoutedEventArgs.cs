#if NET

using System.Windows.Input;
using Microsoft.UI.Xaml;

namespace TimePunch.MVVM.Events
{
    public sealed class ExecutedRoutedEventArgs
        : RoutedEventArgs
    {
        public ExecutedRoutedEventArgs(ICommand command, object? parameter)
        {
            Command = command;
            Parameter = parameter;
        }

        public ICommand Command { get; }

        public object? Parameter { get; }
    }
}

#endif