// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using TimePunch.MVVM.EventAggregation;

namespace TimePunch.MVVM.Controller
{
    public abstract class Kernel : IKernel
    {
        /// <summary>
        /// Gets or sets the Kernel Instance
        /// </summary>
        public static IKernel? Instance { get; set; }

        /// <summary>
        /// Gets the event aggregator.
        /// </summary>
        /// <returns></returns>
        public abstract IEventAggregator EventAggregator { get; }

        /// <summary>
        /// Gets the current used controller
        /// </summary>
        /// <returns></returns>
        public abstract IBaseController Controller { get; }
    }
}
