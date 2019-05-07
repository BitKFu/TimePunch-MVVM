// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Reflection;
using TimePunch.MVVM.EventAggregation;
using TimePunch.MVVM.Events;
using TimePunch.MVVM.ViewModels;

namespace TimePunch.MVVM.Controller
{
    /// <summary>
    ///     Base class for module specific controllers
    /// </summary>
    public abstract class BaseController
    {
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

        #endregion Properties
    }
}