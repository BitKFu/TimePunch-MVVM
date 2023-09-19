using System.Windows.Input;
using Windows.Storage.Streams;
using Microsoft.UI.Dispatching;
using TimePunch.MVVM.EventAggregation;
using TimePunch.MVVM.Events;
using TimePunch.MVVM.ViewModels;
using TimePunchIntroPlayerWinUI3.Core;
using TimePunchIntroPlayerWinUI3.Events.MainPage;
using TimePunchIntroPlayerWinUI3.Model;

namespace TimePunchIntroPlayerWinUI3.ViewModels
{
    public class MainViewModel : ViewModelBase
        , IHandleMessage<VideoPositionChangedEvent>
        , IHandleMessage<MediaOpenedEvent>
    {

        #region Fields

        private IRandomAccessStream fileStream;
        private string fileType;

        private static MainPageModel mainPageModel;

        #endregion

        public MainViewModel() : base(TimePunchIntroPlayerWinUi3Kernel.Get().EventAggregator)
        {
        }

        #region Initialization of commands

        public override void Initialize()
        {
            OpenMp4Command = RegisterCommand(ExecuteOpenMp4Command, CanExecuteOpenMp4Command, true);
            PauseVideoCommand = RegisterCommand(ExecutePauseVideoCommand, CanExecutePauseVideoCommand, true);
            PlayVideoCommand = RegisterCommand(ExecutePlayVideoCommand, CanExecutePlayVideoCommand, true);
            OpenAboutPageCommand = RegisterCommand(ExecuteOpenAboutPageCommand, CanExecuteOpenAboutPageCommand, true);

            AddPropertyChangedNotification(() => IsVideoPlaying, PauseVideoCommand, PlayVideoCommand, OpenMp4Command);
            AddPropertyChangedNotification(() => IsVideoOpen, PauseVideoCommand, PlayVideoCommand);
        }

        #endregion

        #region Reload pagecontent

        public override void InitializePage(object extraData, DispatcherQueue dispatcher)
        {
            base.InitializePage(extraData, dispatcher);

            if (mainPageModel != null)
            {
                // Resend OpenVideoEvent

                fileStream = mainPageModel.FileStream;
                fileType = mainPageModel.FileType;

                EventAggregator.PublishMessage(new OpenVideoEvent(fileStream, fileType, mainPageModel.VideoPosition));
            }
        }

        #endregion

        //Properties
        #region VideoStatusProperties
        public bool IsVideoPlaying
        {
            get { return GetPropertyValue(() => IsVideoPlaying); }
            set { SetPropertyValue(() => IsVideoPlaying, value); }
        }

        public bool IsVideoOpen
        {
            get { return GetPropertyValue(() => IsVideoOpen); }
            set { SetPropertyValue(() => IsVideoOpen, value); }
        }
        #endregion

        #region VideoVolumeProperties
        public double VideoVolume
        {
            get { return GetPropertyValue(() => VideoVolume); }
            set
            {
                if (SetPropertyValue(() => VideoVolume, value))
                {
                    EventAggregator.PublishMessage(new ChangeVolumeEvent(value));
                }
            }
        }
        #endregion

        #region VideoPositionProperties

        public double VideoPosition
        {
            get { return GetPropertyValue(() => VideoPosition); }
            set
            {
                if (SetPropertyValue(() => VideoPosition, value))
                {
                    EventAggregator.PublishMessage(new SeekToPlayerPositionEvent(VideoPosition));
                }
            }
        }

        #endregion

        //Commands
        #region OpenMp4Command

        public ICommand OpenMp4Command
        {
            get { return GetPropertyValue(() => OpenMp4Command); }
            set { SetPropertyValue(() => OpenMp4Command, value); }
        }

        public void CanExecuteOpenMp4Command(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        public async void ExecuteOpenMp4Command(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            var openFile = await EventAggregator.PublishMessageAsync(new OpenStorageFileEvent($@"*.mp4", "Video File|*.mp4"));

            if (openFile.Result == true)
            {
                fileStream = openFile.FileStream;
                fileType = openFile.ContentType;

                EventAggregator.PublishMessage(new OpenVideoEvent(openFile.FileStream, openFile.ContentType, 0));
            }
        }

        #endregion

        #region PlayVideoCommand
        public ICommand PlayVideoCommand
        {
            get { return GetPropertyValue(() => PlayVideoCommand); }
            set { SetPropertyValue(() => PlayVideoCommand, value); }
        }

        public void CanExecutePlayVideoCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = !IsVideoPlaying && IsVideoOpen;
        }

        public void ExecutePlayVideoCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            IsVideoPlaying = true;
            EventAggregator.PublishMessage(new PlayVideoEvent());
        }
        #endregion

        #region PauseVideoCommand

        public ICommand PauseVideoCommand
        {
            get { return GetPropertyValue(() => PauseVideoCommand); }
            set { SetPropertyValue(() => PauseVideoCommand, value); }
        }

        public void CanExecutePauseVideoCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = IsVideoPlaying && IsVideoOpen;
        }

        public void ExecutePauseVideoCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {

            IsVideoPlaying = false;
            EventAggregator.PublishMessage(new PauseVideoEvent());
        }

        #endregion

        #region OpenAboutPageCommand

        public ICommand OpenAboutPageCommand
        {
            get => GetPropertyValue(() => OpenAboutPageCommand);
            set => SetPropertyValue(() => OpenAboutPageCommand, value);
        }

        public void CanExecuteOpenAboutPageCommand(object sende, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        public void ExecuteOpenAboutPageCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            mainPageModel = new MainPageModel(VideoPosition, VideoVolume, fileType, IsVideoPlaying, IsVideoOpen, fileStream);
            EventAggregator.PublishMessage(new OpenAboutWindowEvent());
        }

        #endregion

        //Events
        #region MediaEvents
        public void Handle(MediaOpenedEvent message)
        {
            IsVideoPlaying = true;
            IsVideoOpen = true;
            VideoVolume = 50;
        }
        #endregion

        //Methods
        #region VideoPositionMethod

        private void VideoPositionChanged(object state, object e)
        {
            TimePunchIntroPlayerWinUi3Kernel.Get().EventAggregator.PublishMessage(new SeekToPlayerPositionEvent(VideoPosition));
        }

        #endregion

        #region Userdefined Events

        public void Handle(VideoPositionChangedEvent message)
        {
            SetPropertyValue(() => VideoPosition, message.Position.TotalMilliseconds);
        }

        #endregion
    }
}
