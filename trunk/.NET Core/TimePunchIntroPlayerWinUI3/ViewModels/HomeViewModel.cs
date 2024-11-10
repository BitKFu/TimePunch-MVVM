using System.Windows.Input;
using Microsoft.UI.Dispatching;
using TimePunch.MVVM.Events;
using TimePunch.MVVM.ViewModels;
using TimePunchIntroPlayerWinUI3.Core;

namespace TimePunchIntroPlayerWinUI3.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public HomeViewModel() : base(TimePunchIntroPlayerWinUi3Kernel.Get().EventAggregator)
        {
        }

        public override void Initialize()
        {
            GoBackToMainCommand = RegisterCommand(ExecuteGoBackToMainCommand, CanExecuteGoBackToMainCommand, true);
        }


        public override void InitializePage(object extraData, DispatcherQueue dispatcher)
        {
            base.InitializePage(extraData, dispatcher);
        }

        //Properties
        #region GoBackToMainProperties

        public ICommand GoBackToMainCommand
        {
            get => GetPropertyValue(() => GoBackToMainCommand);
            set => SetPropertyValue(() => GoBackToMainCommand, value);
        }

        public void ExecuteGoBackToMainCommand(object sender, ExecutedRoutedEventArgs eventArgs) => EventAggregator.PublishMessage(new GoBackNavigationRequest());

        public void CanExecuteGoBackToMainCommand(object sender, CanExecuteRoutedEventArgs eventArgs) => eventArgs.CanExecute = true;

        #endregion
    }
}
