using TimePunch.MVVM.EventAggregation;

namespace TimePunch.MVVM.Controller
{
    public interface IKernel
    {
        /// <summary>
        /// Gets the event aggregator.
        /// </summary>
        /// <returns></returns>
        IEventAggregator EventAggregator { get; }

        /// <summary>
        /// Gets the current used controller
        /// </summary>
        /// <returns></returns>
        IBaseController Controller { get; }
    }
}