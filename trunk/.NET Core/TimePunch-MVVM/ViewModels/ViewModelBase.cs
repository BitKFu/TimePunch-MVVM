﻿// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TimePunch.MVVM.EventAggregation;

#if NET
using Microsoft.UI.Dispatching;
using TimePunch.MVVM.Commands;
using TimePunch.MVVM.Events;
using Windows.ApplicationModel;
using Windows.System.Threading;
using ThreadPool = Windows.System.Threading.ThreadPool;
#endif

#if NETFRAMEWORK
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using TimePunch.MVVM.Commands;
using TimePunch.MVVM.Controller;
#endif

namespace TimePunch.MVVM.ViewModels
{
    /// <summary>
    ///     The ViewModelBase is used as base for all view models
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        #region Fields

        /// <summary>
        ///     counter for the loading indicator
        /// </summary>
        private int isLoadingCounter;

        #endregion

        #region Constructor / destructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="ViewModelBase" /> class.
        /// </summary>
        /// <param name="eventAggregator">Used Event aggregator, or null if the default Kernel Event Aggregator shall be used</param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected ViewModelBase(IEventAggregator eventAggregator)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            if (eventAggregator == null) 
                throw new ArgumentNullException(nameof(eventAggregator));
            
            // initialize the maps
            PropertyValues = new ConcurrentDictionary<string, object?>();
            DependentNotifications = new Dictionary<string, IList<string>>();
            DependentCommandNotifications = new Dictionary<string, IList<ICommand>>();

            if (!IsDesignMode)
            {
                // subscribe to the event aggregator messages
                this.eventAggregator = eventAggregator;
                this.eventAggregator.Subscribe(this);
            }

            // default dependent notifications
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            AddPropertyChangedNotification(() => IsLoading, () => IsReady, () => ShowDefective);
            AddPropertyChangedNotification(() => IsDefective, () => ShowDefective);
            AddPropertyChangedNotification(() => Error, () => IsDefective);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        /// <summary>
        ///     Finalizes an instance of the ViewModelBase class
        /// </summary>
        ~ViewModelBase()
        {
            if (!IsDisposed)
                Dispose(false);
        }

        /// <summary>
        ///     Initializes the static members of the ViewModel Base
        /// </summary>
        static ViewModelBase()
        {
            try
            {
#if NETFRAMEWORK
                IsDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
#endif

#if NET
                IsDesignMode = DesignMode.DesignModeEnabled;
#endif
            }
            catch (Exception)
            {
                IsDesignMode = true;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public abstract void Initialize();

#if NETFRAMEWORK
        /// <summary>
        ///     Initializes the page.
        ///     This method will be called every time the user navigates to the page
        /// </summary>
        /// <param name="extraData">The extra Data, if there's any. Otherwise NULL</param>
        public abstract void InitializePage(object extraData);

        /// <summary>
        /// Async call for the page initialization
        /// </summary>
        public virtual Task<bool> InitializePageAsync(object extraData)
        {
            InitializePage(extraData);
            return Task.FromResult(true);
        }

#endif

#if NET
        /// <summary>
        ///     Initializes the page.
        ///     This method will be called every time the user navigates to the page
        /// </summary>
        /// <param name="extraData">The extra Data, if there's any. Otherwise NULL</param>
        /// <param name="dispatcher">Dispatcher of the page</param>
        public virtual void InitializePage(object extraData, DispatcherQueue dispatcher)
        {
            Dispatcher = dispatcher;
        }

        /// <summary>
        /// Async call for the page initialization
        /// </summary>
        public virtual Task<bool> InitializePageAsync(object extraData, DispatcherQueue dispatcher)
        {
            InitializePage(extraData, dispatcher);
            return Task.FromResult(true);
        }

#endif

        #endregion

        #region Command Bindings

#if NETFRAMEWORK
        /// <summary>
        /// Gets the command bindings.
        /// </summary>
        /// <value>The command bindings.</value>
        public CommandBindingCollection RegisteredCommands { get; } = new CommandBindingCollection();

        /// <summary>
        /// Creates a command which is automatically disabled when the IsLoading indicator is set to true and
        /// enabled or disabled automatically based on the required permissions
        /// </summary>
        public virtual void RegisterApplicationCommand(
            ICommand commandToRegister,
            Action<object?, ExecutedRoutedEventArgs>? execute,
            Action<object?, CanExecuteRoutedEventArgs>? canExecute,
            bool disableOnLoading)
        {
            var command = new CommandBinding(commandToRegister,
                (sender, parameter) => ExecuteRegisteredCommand(execute, sender, parameter),
                (sender, parameter) => CanExecuteRegisteredCommand(canExecute, sender, parameter, disableOnLoading));

            CommandManager.RegisterClassCommandBinding(GetType(), command);

            RegisteredCommands.Add(command);
        }
#endif

#if NETFRAMEWORK || NET

        /// <summary>
        ///     Creates a command which is automatically disabled when the <see cref="IsLoading" /> indicator is set to true and
        ///     enabled or disabled automatically based on the required permissions
        /// </summary>
        /// <param name="execute">the command action</param>
        /// <param name="canExecute">the predicate to check, if the command can be executed</param>
        /// <param name="disableOnLoading">
        ///     a value indicating whether the command should be disabled when the
        ///     <see cref="IsLoading" /> indicator is set to true
        /// </param>
        public virtual ICommand RegisterCommand(
            Action<object, ExecutedRoutedEventArgs>? execute,
            Action<object, CanExecuteRoutedEventArgs>? canExecute,
            bool disableOnLoading)
        {
            var commandToRegister = new RelayCommand(
                (sender, parameter) => ExecuteRegisteredCommand(execute, sender, parameter),
                (sender, parameter) => CanExecuteRegisteredCommand(canExecute, sender, parameter, disableOnLoading));

            if (disableOnLoading)
                AddPropertyChangedNotification(() => IsLoading, commandToRegister);

            return commandToRegister;
        }

        /// <summary>
        ///     Executes a registered command.
        /// </summary>
        private static void ExecuteRegisteredCommand(
            Action<object, ExecutedRoutedEventArgs>? execute, 
            object sender,
            ExecutedRoutedEventArgs parameter)
        {
            execute?.Invoke(sender, parameter);
        }

        /// <summary>
        ///     Returns a value indicating, whether a registered command can be executed, which is the case if the
        ///     <see cref="IsLoading" /> indicator is not set to true
        ///     and the user has the required permissions and the command's predicate is fulfilled
        /// </summary>
        private void CanExecuteRegisteredCommand(Action<object, CanExecuteRoutedEventArgs>? canExecute, object sender,
            CanExecuteRoutedEventArgs parameter, bool disableOnLoading)
        {
            canExecute?.Invoke(sender, parameter);
            parameter.CanExecute = (!disableOnLoading || !IsLoading) && parameter.CanExecute;
        }

#endif

        #endregion

        #region PropertyChanged event

        /// <summary>
        ///     Raised when a property on this object gets a new value.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        ///     Used Event Aggregator
        /// </summary>
        private readonly IEventAggregator eventAggregator;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the event aggregator of the application.
        /// </summary>
        protected virtual IEventAggregator EventAggregator => eventAggregator;

        /// <summary>
        ///     Gets the dictionary of the property values, which maps the name of a property to its current value.
        /// </summary>
        protected ConcurrentDictionary<string, object?> PropertyValues { get; }

        /// <summary>
        ///     Gets the dictionary, which maps the name of a base property to a list of dependent property names
        /// </summary>
        protected IDictionary<string, IList<string>> DependentNotifications { get; }

        /// <summary>
        ///     Gets the dictionary, which maps the name of a base property to a list of dependent property names
        /// </summary>
        protected IDictionary<string, IList<ICommand>> DependentCommandNotifications { get; }

        /// <summary>
        ///     Gets or sets the error message indicating what is wrong with this object.
        /// </summary>
        public string? Error
        {
            get { return GetPropertyValue(() => Error); }
            set { SetPropertyValue(() => Error, value); }
        }

        /// <summary>
        ///     Gets a value indicating whether the ViewModel is defect
        /// </summary>
        public bool IsDefective => !string.IsNullOrEmpty(Error);

        /// <summary>
        ///     Gets a value indicating whether the error message shall be shown
        /// </summary>
        public bool ShowDefective => IsDefective && !IsLoading;

        /// <summary>
        ///     Gets or sets a value indicating whether the view model is currently loading data and
        ///     therefore other actions should be prevented.
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return GetPropertyValue(() => IsLoading);
            }

            set
            {
                if (value)
                {
                    Interlocked.Increment(ref isLoadingCounter);
                }
                else
                {
                    if (Interlocked.Decrement(ref isLoadingCounter) < 0)
                        isLoadingCounter = 0;
                }

                var isLoading = isLoadingCounter > 0;
                SetPropertyValue(() => IsLoading, isLoading);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the view model is ready for user interaction
        /// </summary>
        public bool IsReady => !IsLoading;

        /// <summary>
        ///     Gets a value indicating whether the view model is in design mode
        /// </summary>
        public static bool IsDesignMode { get; }

        #endregion

        #region Property handling

        /// <summary>Gets the current value of a property.</summary>
        /// <typeparam name="T">the type of the property</typeparam>
        protected T? GetPropertyValue<T>(Expression<Func<T>> propertyAccessor)
        {
            return GetPropertyValue<T>(GetPropertyName(propertyAccessor));
        }

        /// <summary>Gets the current value of a property.</summary>
        /// <typeparam name="T">the type of the property</typeparam>
        protected T? GetPropertyValue<T>(string propertyName)
        {
            if (PropertyValues.TryGetValue(propertyName, out var result))
                return (T?) result;

            return default;
        }

        /// <summary>
        ///     Sets the value of a property. If the value is different the current value of the property,
        ///     <see cref="PropertyChanged" /> event is raised.
        /// </summary>
        /// <typeparam name="T">the type of the property</typeparam>
        /// <returns>True if the property value has been updated, otherwise false</returns>
        protected bool SetPropertyValue<T>(Expression<Func<T>> propertyAccessor, T? value)
        {
            var valueChanged = false;

            var propertyName = GetPropertyName(propertyAccessor);
            if (PropertyValues.ContainsKey(propertyName))
            {
                PropertyValues.AddOrUpdate(propertyName, value, (key, previousValue) =>
                {
                    if (typeof(T).IsValueType)
                        valueChanged = previousValue == null || !previousValue.Equals(value);
                    else
                        valueChanged = !previousValue?.Equals(value) ?? value != null;

                    return value;
                });

                if (valueChanged)
                    OnPropertyChanged(propertyName);
            }
            else
            {
                valueChanged = true;
                PropertyValues.AddOrUpdate(propertyName, value, (key, previousValue) => value);

                OnPropertyChanged(propertyName);
            }

            return valueChanged;
        }

        /// <summary>
        ///     Raises the <see cref="PropertyChanged" /> event for a property and all its dependent properties (that have been
        ///     registered by the AddPropertyChangedNotification method.
        /// </summary>
        /// <param name="propertyName">the name of the property</param>
        protected void OnPropertyChanged(string propertyName)
        {
            try
            {
                RaisePropertyChanged(propertyName, true, new Dictionary<string, object?>(), new Dictionary<ICommand, object?>());
            }
            catch (NullReferenceException)
            {
                // that's not nice, but we can ignore it
            }
        }

        /// <summary>
        ///     Raises the PropertyChanged event for a property and - if requested - all its dependent properties (that have been
        ///     registered by the AddPropertyChangedNotification method.
        /// </summary>
        /// <typeparam name="T">the type of the property</typeparam>
        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyAccessor, bool notifyDependentProperties = true)
        {
            try
            {
                var propertyName = GetPropertyName(propertyAccessor);
                RaisePropertyChanged(propertyName, notifyDependentProperties, new Dictionary<string, object?>(), new Dictionary<ICommand, object?>());
            }
            catch (NullReferenceException)
            {
                // that's not nice, but we can ignore it
            }
        }

        /// <summary>
        ///     Raises this object's PropertyChanged event.
        /// </summary>
        private void RaisePropertyChanged(string propertyName, bool notifyDependentProperties,
            IDictionary<string, object?> processedProperties,
            IDictionary<ICommand, object?> processedCommands)
        {
            if (processedProperties.ContainsKey(propertyName))
                return;

            var eventArgs = new PropertyChangedEventArgs(propertyName);

            if (PropertyChanged != null)
                Dispatch(() => PropertyChanged(this, eventArgs));

// ReSharper disable AssignNullToNotNullAttribute
            processedProperties.Add(propertyName, null);
// ReSharper restore AssignNullToNotNullAttribute

            if (!notifyDependentProperties)
                return;

            if (DependentNotifications.TryGetValue(propertyName, out var dependentPropertyNotifications))
                foreach (var dependentProperty in dependentPropertyNotifications)
                    RaisePropertyChanged(dependentProperty, true, processedProperties, processedCommands);

            if (DependentCommandNotifications.TryGetValue(propertyName, out var dependendCommandNotifications))
                foreach (var dependentCommand in dependendCommandNotifications)
                {
                    if (processedCommands.ContainsKey(dependentCommand))
                        continue;

                    if (dependentCommand is RelayCommand relayCommand)
                        relayCommand.RaiseCanExecuteChanged();

// ReSharper disable AssignNullToNotNullAttribute
                    processedCommands.Add(dependentCommand, null);
// ReSharper restore AssignNullToNotNullAttribute
                }
        }

        /// <summary>
        ///     Adds a new entry to the list of dependent property notifications.
        /// </summary>
        /// <typeparam name="TSource">the type of the source property</typeparam>
        /// <typeparam name="TDepend">the type of the dependent property</typeparam>
        public virtual void AddPropertyChangedNotification<TSource, TDepend>(
            Expression<Func<TSource>> sourcePropertyAccessor,
            params Expression<Func<TDepend>>[] dependentPropertyAccessors)
        {
            var basePropertyName = GetPropertyName(sourcePropertyAccessor);

            foreach (var dependentPropertyName in dependentPropertyAccessors
                .Select(GetPropertyName)
                .Where(dependentPropertyName => basePropertyName != dependentPropertyName))
            {
                if (!DependentNotifications.TryGetValue(basePropertyName, out var notifications))
                {
                    notifications = new List<string>();
                    DependentNotifications.Add(basePropertyName, notifications);
                }

                notifications.Add(dependentPropertyName);
            }
        }

        /// <summary>
        ///     Adds a new entry to the list of dependent command notifications.
        /// </summary>
        /// <typeparam name="TSource">the type of the source property</typeparam>
        public virtual void AddPropertyChangedNotification<TSource>(Expression<Func<TSource>> sourcePropertyAccessor,
            params ICommand[] dependentCommands)
        {
            // get the name of the source property
            var basePropertyName = GetPropertyName(sourcePropertyAccessor);
            AddPropertyChangedNotification(basePropertyName, dependentCommands);
        }

        /// <summary>
        ///     Adds the dependent notification.
        /// </summary>
        /// <param name="basePropertyName">Name of the base property.</param>
        /// <param name="dependentCommands">The dependent commands.</param>
        /// <exception cref="ArgumentNullException">Thrown when a dependent command is null.</exception>
        public virtual void AddPropertyChangedNotification(string basePropertyName, params ICommand[] dependentCommands)
        {
            IList<ICommand> notifications;

            if (DependentCommandNotifications.TryGetValue(basePropertyName, out var commandNotification))
            {
                notifications = commandNotification;
            }
            else
            {
                notifications = new List<ICommand>();
                DependentCommandNotifications.Add(basePropertyName, notifications);
            }

            foreach (var notification in dependentCommands.Except(notifications))
            {
                if (notification == null)
                    throw new ArgumentNullException(nameof(notification));

                notifications.Add(notification);
            }
        }

#endregion

        #region Threading

#if NETFRAMEWORK
        protected virtual Dispatcher? Dispatcher => Application.Current?.Dispatcher;
#endif

#if NET
        protected virtual DispatcherQueue? Dispatcher { get; private set; }

        protected bool IsUiThread => Dispatcher?.HasThreadAccess ?? false;
#endif

        /// <summary>
        ///     Performs a specific action on the UI thread.
        /// </summary>
        /// <param name="action">the action to execute</param>
        public virtual void Dispatch(Action action)
        {
            if (Dispatcher == null)
                action();
            else
            {
#if NET
                // check, if we have access to the UI thread
                if (IsUiThread)
                    action(); // execute directly
                else
                    Dispatcher.TryEnqueue(() => action()); // place the action on the Dispatcher of the UI thread
#endif

#if NETFRAMEWORK
                // check, if we have access to the UI thread
                if (Dispatcher.CheckAccess())
                {
                    action(); // execute directly
                }
                else
                {
                    try
                    {
                        Dispatcher.Invoke(
                            action,
                            DispatcherPriority.Normal,
                            CancellationToken.None,
                            BaseController.InvocationTimeout); // place the action on the Dispatcher of the UI thread
                    }
                    catch (TimeoutException)
                    {
                        Trace.TraceWarning("Can't dispatch action in time, so try to invoke async");
                        Dispatcher.InvokeAsync(action);
                    }
                }
#endif
            }
        }


        /// <summary>
        /// Performs an delayed action on the UI thread
        /// </summary>
        /// <param name="action">the action to execute</param>
#if NET
        [Obsolete("DispatchDelayed is obsolete when using WinUI because every action is executed async on UI Thread")]
#endif
        public virtual void DispatchDelayed(Action action)
        {
#if NET
            Dispatch(action);
#endif

#if NETFRAMEWORK
            if (Dispatcher == null)
                action();
            else
                // check, if we have access to the UI thread
            if (Dispatcher.CheckAccess())
                action(); // execute directly
            else
                Dispatcher.BeginInvoke(action); // place the action on the Dispatcher of the UI thread
#endif
        }


        /// <summary>
        ///     Executes an action asynchronously.
        /// </summary>
        /// <param name="action">the action to execute</param>
        /// <param name="delay">
        ///     a delay in milliseconds to wait before the action is executed (pass a value
        ///     greate or equal to zero to execute the action without any delay)
        /// </param>
        public virtual void ExecuteAsync(Action action, int delay = 0)
        {
            ExecuteAsync<object>(param => action(), null, delay);
        }

        /// <summary>
        ///     Executes an action, that takes one parameter, asynchronously.
        /// </summary>
        /// <typeparam name="T">the type of the parameter for the action</typeparam>
        /// <param name="action">the action to execute</param>
        /// <param name="parameter">the parameter to pass to the action</param>
        /// <param name="delay">
        ///     a delay in milliseconds to wait before the action is executed (pass a value
        ///     greater or equal to zero to execute the action without any delay)
        /// </param>
        public virtual void ExecuteAsync<T>(Action<T?> action, T? parameter, int delay = 0)
        {
#if HAS_UNO
            Task.Run(() =>
            {
                // check, if the action execution should be delayed
                if (delay > 0)
                    Task.Delay(delay).Wait();

                // execute the action
                action(parameter);
                return Task.CompletedTask;
            }).Start();
#elif NET
            // start new thread from the thread pool and pass it the action and the parameter
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            ThreadPool.RunAsync(
                async =>
                {
                    // check, if the action execution should be delayed
                    if (delay > 0)
                        Task.Delay(delay).Wait();

                    // execute the action
                    action(parameter);
                }, WorkItemPriority.Normal);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

#endif

#if NETFRAMEWORK
            // start new thread from the thread pool and pass it the action and the parameter
            ThreadPool.QueueUserWorkItem(
                threadParameter =>
                {
                    var threadParameters = (Object[])threadParameter;

                    // check, if the action execution should be delayed
                    if (delay > 0)
                        Thread.Sleep(delay);

                    // execute the action
                    ((Action<T>)threadParameters[0])((T)threadParameters[1]);
                },
                new Object?[] { action, parameter });
#endif
        }

        #endregion

        #region Disposing

        /// <summary>
        ///     Invoked when this object is being removed from the application
        ///     and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Child classes can override this method to perform
        ///     clean-up logic, such as removing event handlers.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        public virtual void Dispose(bool disposing)
        {
            if (IsDesignMode)
                return;

#if DEBUG
            if (IsDisposed)
                throw new ObjectDisposedException("ViewModel has already been disposed");
#endif

#if NETFRAMEWORK
            // UnSubscribe from the event aggregator
            DispatchDelayed(() => EventAggregator.Unsubscribe(this));
#endif

#if NET
            Dispatch(() => EventAggregator.Unsubscribe(this));
#endif
            IsDisposed = true;
        }

        /// <summary>
        ///     True, if the ViewModel has been disposed
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        ///     Gets or sets the faulted command that can be re-run
        /// </summary>
        public ICommand? FaultedCommand
        {
            get { return GetPropertyValue(() => FaultedCommand); }
            set { SetPropertyValue(() => FaultedCommand, value); }
        }

#endregion

#if NETFRAMEWORK

        #region Property DialogResult

        /// <summary>
        /// Gets or sets the DialogResult.
        /// </summary>
        /// <value>The DialogResult.</value>
        public bool? DialogResult
        {
            get { return GetPropertyValue(() => DialogResult); }
            set { SetPropertyValue(() => DialogResult, value); }
        }

        #endregion
#endif

        #region Property Helper

        /// <summary>
        ///     Gets the name of the property.
        /// </summary>
        public static string GetPropertyName<T>(Expression<Func<T>> propertyAccessor)
        {
            return ((MemberExpression)propertyAccessor.Body).Member.Name;
        }

        #endregion
    }
}