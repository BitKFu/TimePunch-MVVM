namespace TimePunchIntroPlayer
{
    public class ViewModelLocator
    {
        private static MainViewModel mainViewModel;
        private static AboutViewModel aboutWindowModel;

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

        public AboutViewModel AboutWindowModel
        {
            get
            {
                aboutWindowModel?.Dispose();
                aboutWindowModel = new AboutViewModel();
                aboutWindowModel.Initialize();
                return aboutWindowModel;
            }
        }
    }
}
