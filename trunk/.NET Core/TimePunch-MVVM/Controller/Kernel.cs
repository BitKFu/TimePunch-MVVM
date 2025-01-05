// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Runtime.CompilerServices;
using TimePunch.MVVM.EventAggregation;

namespace TimePunch.MVVM.Controller
{
    public abstract class Kernel<TProjectKernel, TProjectController> : IKernel<TProjectController>
        where TProjectKernel : IKernel<TProjectController>, new()
        where TProjectController : IBaseController, new()
    {

        private static TProjectKernel? singleton;

        /// <summary>
        /// Gets or sets the Kernel Instance
        /// </summary>
        public static TProjectKernel Instance
        {
            get
            {
                if (singleton != null)
                    return singleton;

                singleton = new TProjectKernel();
                singleton.Controller = new();
                return singleton;

            }
            set => singleton = value;
        }

        /// <summary>
        /// Gets the event aggregator.
        /// </summary>
        /// <returns></returns>
        public IEventAggregator EventAggregator { get; protected set; } = new EventAggregator();

        /// <summary>
        /// Gets the current used controller
        /// </summary>
        /// <returns></returns>
        public TProjectController Controller { get; set; } = default!;
    }
}
