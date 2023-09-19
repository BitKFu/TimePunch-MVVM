using System.Windows.Input;
using Microsoft.UI.Dispatching;
using TimePunch.MVVM.Events;
using TimePunch.MVVM.ViewModels;
using TimePunchIntroPlayerWinUI3.Core;
using TimePunchIntroPlayerWinUI3.Events.AboutPage;

namespace TimePunchIntroPlayerWinUI3.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public AboutViewModel() : base(TimePunchIntroPlayerWinUi3Kernel.Get().EventAggregator)
        {
        }

        public override void Initialize()
        {
            GoBackToMainPageCommand = RegisterCommand(ExecuteGoBackToMainPageCommand, CanExecuteGoBackToMainPageCommand, true);
            VisitTwitterSiteCommand = RegisterCommand(ExecuteVisitTwitterSiteCommand, CanExecuteVisitTwitterSiteCommand, true);
            VisitHomepageSiteCommand = RegisterCommand(ExecuteVisitHomepageSiteCommand, CanExecuteVisitHomepageSiteCommand, true);
        }

        public override void InitializePage(object extraData, DispatcherQueue dispatcher)
        {
            base.InitializePage(extraData, dispatcher);
        }

        //Properties
        #region DialogResult
        public bool DialogResult
        {
            get => GetPropertyValue(() => DialogResult);
            set => SetPropertyValue(() => DialogResult, value);
        }
        #endregion

        //Commands
        #region GoBackToMainPageCommand

        public ICommand GoBackToMainPageCommand
        {
            get => GetPropertyValue(() => GoBackToMainPageCommand);
            set => SetPropertyValue(() => GoBackToMainPageCommand, value);
        }

        public void ExecuteGoBackToMainPageCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }

        public void CanExecuteGoBackToMainPageCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        #endregion

        #region VisitTwitterSiteCommand

        public ICommand VisitTwitterSiteCommand
        {
            get => GetPropertyValue(() => VisitTwitterSiteCommand);
            set => SetPropertyValue(() => VisitTwitterSiteCommand, value);
        }

        public void ExecuteVisitTwitterSiteCommand(object sender, ExecutedRoutedEventArgs eventArgs) => EventAggregator.PublishMessage(new VisitTwitterWebsiteEvent());

        public void CanExecuteVisitTwitterSiteCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
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

        public void ExecuteVisitHomepageSiteCommand(object sender, ExecutedRoutedEventArgs eventArgs) => EventAggregator.PublishMessage(new VisitHomepageSiteEvent());

        public void CanExecuteVisitHomepageSiteCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        #endregion
    }
}
