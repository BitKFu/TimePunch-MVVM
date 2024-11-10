using System.Windows;
using System.Windows.Navigation;

namespace TimePunchIntroPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IntroPlayerController IntroPlayerController { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IntroPlayerController = new IntroPlayerController();
        }
    }
}
