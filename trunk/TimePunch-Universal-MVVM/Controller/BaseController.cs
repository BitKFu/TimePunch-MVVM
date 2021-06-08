// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Reflection;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using TimePunch.MVVM.EventAggregation;
using TimePunch.MVVM.Events;
using TimePunch.MVVM.ViewModels;

namespace TimePunch.MVVM.Controller
{
    /// <summary>
    ///     Base class for module specific controllers
    /// </summary>
    public abstract class BaseController : TimePunch.MVVM.Controller.IBaseController,
        IHandleMessage<GoBackNavigationRequest>
    {
        #region Members

        private NavigationMode navigationMode;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the BaseController class.
        /// </summary>
        protected BaseController(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);
        }       
        
        /// <summary>
        ///     This method can do some initializations
        /// </summary>
        public virtual void Init()
        {
        }

        /// <summary>
        ///     Releases unmanaged resources and performs other cleanup operations before the
        ///     <see cref="BaseController" /> is reclaimed by garbage collection.
        /// </summary>
        ~BaseController()
        {
            EventAggregator.Unsubscribe(this);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///     Set an standard invocation timeout of 5 seconds
        /// </summary>
        public static TimeSpan InvocationTimeout { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        ///     Gets or sets the controller that used for monitor the navigational messages
        /// </summary>
        protected IEventAggregator EventAggregator { get; set; }

        /// <summary>
        /// Gets or sets the content frame
        /// </summary>
        protected virtual Frame ContentFrame { get; private set; }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>The current page.</value>
        public Page CurrentPage { get; private set; }

        #endregion Properties

        #region Navigation Methods

        /// <summary>
        /// Attaches the controller to the root frame
        /// </summary>
        /// <param name="contentFrame">The Content Frame</param>
        public virtual void SetContentFrame(Frame contentFrame)
        {
            if (contentFrame != null)
            {
                // wire events
                contentFrame.Navigated += OnNavigated;
                contentFrame.Navigating += OnNavigating;
            }

            ContentFrame = contentFrame;
        }

        /// <summary>
        /// Called when the NavigationService navigates to a new page inside the application
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Navigating Event Args</param>
        protected virtual void OnNavigated(object sender, NavigationEventArgs e)
        {
            var page = e.Content as Page;

            // keep track of the current page
            CurrentPage = page;
            if (page == null) return;

            // Set the navigation cache mode to enabled as default
            page.NavigationCacheMode = NavigationCacheMode.Enabled;

            // Send NavigationCompletedEvent, if it's a normal navigation request
            object context = page.DataContext;
            if (context != null)
            {
                // Send Navigated Request
                Type requestType = typeof(NavigationCompletedEvent<>).MakeGenericType(context.GetType());

                MethodInfo publishMessage = EventAggregator.GetType().GetMethod("PublishMessage");
                publishMessage = publishMessage.GetGenericMethodDefinition().MakeGenericMethod(requestType);
                publishMessage.Invoke(EventAggregator, new[] { Activator.CreateInstance(requestType, navigationMode) });
            }

            // Initializes the Page if the DataContext is derived from the ViewModelBase
            if (page.DataContext is ViewModelBase viewModelBase)
                viewModelBase.InitializePage(e.Parameter);

            EventAggregator.PublishMessage(new NavigatedEvent());
        }

        /// <summary>
        /// On Navigation Handler
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Navigating Cancel Event Args</param>
        protected virtual void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            navigationMode = e.NavigationMode;
            EventAggregator.PublishMessage(new NavigationEvent(e));
        }

        protected virtual CoreDispatcher Dispatcher => CoreApplication.GetCurrentView()?.CoreWindow?.Dispatcher
                                                       ?? Window.Current?.Dispatcher;

        /// <summary>
        /// Navigates to page.
        /// </summary>
        /// <param name="navigateToPage">The navigate to page.</param>
        public virtual void NavigateToPage(Type navigateToPage)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            if (CurrentPage == null)
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => ContentFrame.Navigate(navigateToPage));
            else
            {
                if (CurrentPage.Dispatcher.HasThreadAccess)
                {
                    ContentFrame.Navigate(navigateToPage);
                    //PasswordBoxAssistant.IsNavigating = true;
                    //try
                    //{
                    //    ContentFrame.Navigate(navigateToPage);
                    //}
                    //finally
                    //{
                    //    PasswordBoxAssistant.IsNavigating = false;
                    //}
                }
                else
                    CurrentPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => NavigateToPage(navigateToPage));
            }
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        /// <summary>
        /// Navigates to a page and add additional data
        /// </summary>
        /// <param name="navigateToPage">The navigate to page.</param>
        /// <param name="message">The message thas will be send to the page</param>
        public virtual void NavigateToPage(Type navigateToPage, object message)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            if (CurrentPage == null)
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => ContentFrame.Navigate(navigateToPage, message));
            else
            {
                if (CurrentPage.Dispatcher.HasThreadAccess)
                {
                    ContentFrame.Navigate(navigateToPage, message);
                    //PasswordBoxAssistant.IsNavigating = true;
                    //try
                    //{
                    //    ContentFrame.Navigate(navigateToPage, message);
                    //}
                    //finally
                    //{
                    //    PasswordBoxAssistant.IsNavigating = false;
                    //}
                }
                else
                    CurrentPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => NavigateToPage(navigateToPage, message));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }

        /// <summary>
        /// Determines whether this instance [can go back].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance [can go back]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanGoBack
        {
            get
            {
                // Maybe we can't access it directly
                if (CurrentPage != null && !CurrentPage.Dispatcher.HasThreadAccess)
                {
                    bool result = false;

                    var waitHandle = CurrentPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => result = CanGoBack);
                    while (waitHandle.Status == AsyncStatus.Started)
                        CoreWindow.GetForCurrentThread().Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessAllIfPresent);

                    return result;
                }

                return CurrentPage != null && ContentFrame != null && ContentFrame.CanGoBack;
            }
        }

        /// <summary>
        /// Handles the Navigation Request
        /// </summary>
        /// <param name="message">The Navigation goback request</param>
        public virtual void Handle(GoBackNavigationRequest message)
        {
            // Check, if we can go back
            if (!CanGoBack)
                return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            // Now Go Back
            if (CurrentPage.Dispatcher.HasThreadAccess)
            {
                // When going back, delete an potential error and clear the state
                if (CurrentPage.DataContext is ViewModelBase baseModel && baseModel.IsDefective)
                    baseModel.Error = string.Empty;

                if (ContentFrame.CanGoBack)
                    ContentFrame.GoBack();
            }
            else
                CurrentPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Handle(message));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            
        }
        #endregion
    }
}