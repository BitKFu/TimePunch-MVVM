using Windows.UI.Xaml.Controls;

namespace TimePunch.MVVM.Controller
{
    /// <summary>
    /// Interface for the Base Controller
    /// </summary>
    public interface IBaseController
    {
        /// <summary>
        /// Used to initialize the Navigation Controller
        /// </summary>
        void Init();

        /// <summary>
        /// Initializes the Navigation Controller
        /// </summary>
        void SetContentFrame(Frame contentFrame);

        /// <summary>
        /// Determines whether this instance can go back.
        /// </summary>
        bool CanGoBack { get; }
    }
}
