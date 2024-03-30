
#if NETFRAMEWORK
using System.Windows.Controls;
#endif

#if NET
using Microsoft.UI.Xaml.Controls;
#endif

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

#if NETFRAMEWORK || NET

        /// <summary>
        /// Initializes the Navigation Controller
        /// </summary>
        void SetContentFrame(Frame contentFrame);

        /// <summary>
        /// Determines whether this instance can go back.
        /// </summary>
        bool CanGoBack { get; }

#endif
    }
}
