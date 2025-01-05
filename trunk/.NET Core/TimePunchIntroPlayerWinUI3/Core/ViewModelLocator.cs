using TimePunchIntroPlayerWinUI3.ViewModels;

namespace TimePunchIntroPlayerWinUI3.Core
{
    public class ViewModelLocator
    {
        #region Fields

        private static MainViewModel mainViewModel;

        private static AboutViewModel aboutViewModel;

        private static TwitterViewModel twitterViewModel;


        private static HomeViewModel homeViewModel;
        #endregion

        #region Properties

        public MainViewModel MainViewModel
        {
            get
            {
                mainViewModel?.Dispose();
                mainViewModel = new MainViewModel();
                mainViewModel.Initialize();
                return mainViewModel;
            }
        }

        public AboutViewModel AboutViewModel
        {
            get
            {
                aboutViewModel?.Dispose();
                aboutViewModel = new AboutViewModel();
                aboutViewModel.Initialize();
                return aboutViewModel;
            }
        }

        public TwitterViewModel TwitterViewModel
        {
            get
            {
                twitterViewModel?.Dispose();
                twitterViewModel = new TwitterViewModel();
                twitterViewModel.Initialize();
                return twitterViewModel;
            }
        }

        public HomeViewModel HomeViewModel
        {
            get
            {
                homeViewModel?.Dispose();
                homeViewModel = new HomeViewModel();
                homeViewModel.Initialize();
                return homeViewModel;
            }
        }

        #endregion

    }
}
