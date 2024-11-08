// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Reflection;
using System.Linq;
using System.Windows.Input;

#if NET
using TimePunch.MVVM.Events;
using Microsoft.UI.Dispatching;
using TimePunch.MVVM.Controller;
#endif

#if NETFRAMEWORK
using System.Windows;
using System.Windows.Threading;
#endif

namespace TimePunch.MVVM.Commands
{
    public class RelayCommand : ICommand
    {
        private static readonly ConstructorInfo ExecutedRoutedEventArgsConstructor =
            typeof(ExecutedRoutedEventArgs).GetConstructors(
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).First();

        private static readonly ConstructorInfo CanExecuteRoutedEventArgsConstructor =
            typeof(CanExecuteRoutedEventArgs).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).First();

        /// <summary>
        ///     Initializes a new instance of the RelayCommand class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<object, ExecutedRoutedEventArgs>? execute,
            Action<object, CanExecuteRoutedEventArgs>? canExecute)
        {
            ExecuteAction = execute;
            CanExecuteAction = canExecute;

            if (execute != null)
                OwnsExecute = true;

#if NET
            Dispatcher = BaseController.ApplicationDispatcher;
#endif

#if NETFRAMEWORK
            // get hold of dispatcher (only necessary in .NET Framework)
            if (Application.Current != null)
                Dispatcher = Application.Current.Dispatcher;
#endif
        }

        /// <summary>
        ///     Gets or sets the execute action.
        /// </summary>
        /// <value>The execute action.</value>
        protected Action<object, ExecutedRoutedEventArgs>? ExecuteAction { get; }

        /// <summary>
        ///     Gets or sets the can execute predicate.
        /// </summary>
        /// <value>The can execute predicate.</value>
        protected Action<object, CanExecuteRoutedEventArgs>? CanExecuteAction { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether [owns execute].
        /// </summary>
        /// <value><c>true</c> if [owns execute]; otherwise, <c>false</c>.</value>
        protected bool OwnsExecute { get; }

#if NET
        /// <summary>
        /// Gets or sets the current dispatcher.
        /// </summary>
        /// <value>The current dispatcher.</value>
        protected DispatcherQueue Dispatcher { get; }
#endif

#if NETFRAMEWORK
        /// <summary>
        /// Gets or sets the current dispatcher.
        /// </summary>
        /// <value>The current dispatcher.</value>
        protected Dispatcher? Dispatcher { get; }
#endif

        /// <summary>
        ///     Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command.  If the command does not require data to be passed, this object can
        ///     be set to null.
        /// </param>
        public void Execute(object? parameter)
        {
            if (ExecuteAction == null)
                return;

#if NET
            var args = new ExecutedRoutedEventArgs(this, parameter);
#endif

#if NETFRAMEWORK
            var args = (ExecutedRoutedEventArgs)ExecutedRoutedEventArgsConstructor.Invoke([this, parameter]);
#endif
            ExecuteAction(parameter ?? new object(), args);
        }

        /// <summary>
        ///     Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        ///     true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">
        ///     Data used by the command.  If the command does not require data to be passed, this object can
        ///     be set to null.
        /// </param>
        public bool CanExecute(object? parameter)
        {
            if (CanExecuteAction == null)
                return false;

#if NET
            var args = new CanExecuteRoutedEventArgs(this, parameter);
#endif

#if NETFRAMEWORK
            var args = (CanExecuteRoutedEventArgs)CanExecuteRoutedEventArgsConstructor.Invoke([this, parameter]);
#endif

            CanExecuteAction(parameter ?? new object(), args);
            return args.CanExecute;
        }

        public event EventHandler? CanExecuteChanged;

        /// <summary>
        ///     Raises the can execute changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler == null)
                return;
#if NET
            if (DispatcherQueue.GetForCurrentThread() != null)
                handler(this, EventArgs.Empty); // already on the UI thread
            else
                Dispatcher.TryEnqueue(RaiseCanExecuteChanged);
#endif

#if NETFRAMEWORK
            if (Dispatcher != null && Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke((Action)RaiseCanExecuteChanged);
            }
            else
                handler(this, EventArgs.Empty);      // already on the UI thread
#endif
        }
    }
}
