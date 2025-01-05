using Microsoft.UI.Xaml;
using TimePunchIntroPlayerWinUI3.Core;
using TimePunchIntroPlayerWinUI3.Pages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TimePunchIntroPlayerWinUI3
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public TimePunchIntroPlayerWinUI3Controller WinUI3Controller { get; set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();

            WinUI3Controller = TimePunchIntroPlayerWinUi3Kernel.Instance.Controller;
            WinUI3Controller.Init();
            WinUI3Controller.SetContentFrame(m_window.GetFrame());
            WinUI3Controller.NavigateToPage(typeof(MainPage));
        }

        private MainWindow m_window;

    }
}
