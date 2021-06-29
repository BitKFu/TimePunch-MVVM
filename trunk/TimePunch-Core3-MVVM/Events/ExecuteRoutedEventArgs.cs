﻿using System.Windows.Input;

namespace TimePunch.MVVM.Events
{
    public sealed class ExecutedRoutedEventArgs// : RoutedEventArgs
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
