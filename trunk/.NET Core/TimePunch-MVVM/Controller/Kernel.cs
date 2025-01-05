// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Runtime.CompilerServices;
using TimePunch.MVVM.EventAggregation;

namespace TimePunch.MVVM.Controller
{
    public interface IKernel<TProjectController> : IGenericKernel
        where TProjectController : IBaseController
    {
        /// <summary>
        /// Gets the current used controller
        /// </summary>
        /// <returns></returns>
        TProjectController Controller { get; set; }
    }

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

                // ReSharper disable once UseObjectOrCollectionInitializer
                singleton = new TProjectKernel();   // not used, because of StackOverflowException
                singleton.Controller = new TProjectController();

                GenericKernel.Instance = singleton;
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

        /// <summary>
        /// Gets the current used based controller
        /// </summary>
        IBaseController IGenericKernel.BaseController => Controller;
    }
}
