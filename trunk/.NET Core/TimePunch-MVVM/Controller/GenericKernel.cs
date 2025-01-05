using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimePunch.MVVM.EventAggregation;

namespace TimePunch.MVVM.Controller
{
    public interface IGenericKernel
    {
        /// <summary>
        /// Gets the event aggregator.
        /// </summary>
        /// <returns></returns>
        IEventAggregator EventAggregator { get; }

        /// <summary>
        /// Gets the current used based controller
        /// </summary>
        IBaseController BaseController { get; }
    }

    public class GenericKernel : IGenericKernel
    {
        /// <summary>
        /// Gets or sets the Kernel Instance
        /// </summary>
        public static IGenericKernel? Instance { get; internal set; }

        /// <summary>
        /// Gets the event aggregator.
        /// </summary>
        /// <returns></returns>
        public IEventAggregator EventAggregator => Instance?.EventAggregator ?? default!;

        /// <summary>
        /// Gets the base controller
        /// </summary>
        public IBaseController BaseController => Instance?.BaseController ?? default!;
    }
}
