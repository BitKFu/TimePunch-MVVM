using Microsoft.Win32;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using TimePunch.MVVM.Controller;
using TimePunch.MVVM.EventAggregation;
using TimePunch.TpClientGui.Controls.Events;
using TimePunch.Wpf.Common.UI.Events;
using TimePunchIntroPlayer.Events;
using TimePunchIntroPlayer.Events.About;

namespace TimePunchIntroPlayer
{
    public class IntroPlayerKernel : Kernel<IntroPlayerKernel, IntroPlayerController>;

    public class IntroPlayerController() : BaseController(IntroPlayerKernel.Instance.EventAggregator)
        , IHandleMessage<OpenFileDialogEvent>
        , IHandleMessage<MessageBoxEvent>
        , IHandleMessage<OpenAboutWindowEvent>
        , IHandleMessage<VisitTwitterWebsiteEvent>
        , IHandleMessage<VisitHomepageSiteEvent>
    {
        //Events
        #region OpenFileDialog Events
        public void Handle(OpenFileDialogEvent message)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = message.FileName,
                DefaultExt = message.DefaultExt,
                Filter = message.Filter,
                CheckFileExists = true,
                CheckPathExists = true
            };

            message.Result = openFileDialog.ShowDialog();

            if (message.Result == true)
            {
                message.FileName = openFileDialog.FileName;
                message.FilterIndex = openFileDialog.FilterIndex;
            }
        }
        #endregion

        #region OpenMessageBox Events
        public void Handle(MessageBoxEvent message)
        {
            // Check, if we need to dispatch
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                var waitHandle = new AutoResetEvent(false);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Handle(message);
                    waitHandle.Set();
                });

                waitHandle.WaitOne();
                return;
            }

            var topWindow = Application.Current.Windows.Count - 1;

            message.MessageBoxResult = MessageBox.Show(
                Application.Current.Windows[topWindow],
                message.MessageText,
                message.MessageTitle,
                message.Buttons,
                message.Icon);
        }

        #endregion

        #region OpenNewWindow Events
        public void Handle(OpenAboutWindowEvent aboutWindowEvent)
        {
            var aboutWindow = new AboutWindow();
            if (aboutWindow.DataContext is AboutViewModel viewModel)
            {
                viewModel.InitializePage(aboutWindowEvent);
            }

            var result = aboutWindow.ShowDialog();
        }

        #endregion

        #region Open URLEvents

        public void Handle(VisitTwitterWebsiteEvent websiteEvent)
        {
            string twitterURL = ConfigurationManager.AppSettings["twitterUrl"];

            var openUrlProcess = new Process
            {
                StartInfo = new ProcessStartInfo(twitterURL)
            };
            openUrlProcess.Start();
        }

        public void Handle(VisitHomepageSiteEvent message)
        {
            string homepageURL = ConfigurationManager.AppSettings["homepageUrl"];

            var openUrlProcess = new Process
            {
                StartInfo = new ProcessStartInfo(homepageURL)
            };
            openUrlProcess.Start();
        }

        #endregion
    }
}