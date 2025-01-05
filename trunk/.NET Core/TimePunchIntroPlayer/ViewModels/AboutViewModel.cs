using System.Windows.Input;
using TimePunch.MVVM.ViewModels;
using TimePunchIntroPlayer.Events.About;

namespace TimePunchIntroPlayer
{
    public class AboutViewModel : ViewModelBase
    {
        public AboutViewModel() : base(IntroPlayerKernel.Instance.EventAggregator)
        {
        }


        public override void Initialize()
        {
            GoBackToMainWindowCommand = RegisterCommand(ExecuteGoBackToMainWindowCommand, CanExecuteGoBackToMainWindowCommand, true);
            VisitTwitterWebsiteCommand = RegisterCommand(ExecuteVisitTwitterWebsiteCommand, CanExecuteVisistTwitterWebsiteCommand, true);
            VisitHomepageSiteCommand = RegisterCommand(ExecuteVisitHomepageSiteCommand, CanExecuteVisitHomepageSiteCommand, true);
        }

        public override void InitializePage(object extraData)
        {
        }

        //Properties
        #region DialogResult
        public new bool DialogResult
        {
            get { return GetPropertyValue(() => DialogResult); }
            set { SetPropertyValue(() => DialogResult, value); }
        }
        #endregion

        //Methods

        //Commands
        #region GoBackToMainWindowCommand

        public ICommand GoBackToMainWindowCommand
        {
            get { return GetPropertyValue(() => GoBackToMainWindowCommand); }
            set { SetPropertyValue(() => GoBackToMainWindowCommand, value); }
        }

        public void CanExecuteGoBackToMainWindowCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        public void ExecuteGoBackToMainWindowCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            DialogResult = true;
        }

        #endregion

        #region VisitTwitterSiteCommand

        public ICommand VisitTwitterWebsiteCommand
        {
            get => GetPropertyValue(() => VisitTwitterWebsiteCommand);
            set => SetPropertyValue(() => VisitTwitterWebsiteCommand, value);
        }

        public void ExecuteVisitTwitterWebsiteCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new VisitTwitterWebsiteEvent());
        }

        public void CanExecuteVisistTwitterWebsiteCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        #endregion

        #region VisitHomepageSiteCommand

        public ICommand VisitHomepageSiteCommand
        {
            get => GetPropertyValue(() => VisitHomepageSiteCommand);
            set => SetPropertyValue(() => VisitHomepageSiteCommand, value);
        }

        public void ExecuteVisitHomepageSiteCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new VisitHomepageSiteEvent());
        }

        public void CanExecuteVisitHomepageSiteCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        #endregion
    }
}
