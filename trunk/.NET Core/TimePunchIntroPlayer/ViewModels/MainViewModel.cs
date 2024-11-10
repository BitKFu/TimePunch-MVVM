using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using TimePunch.MVVM.EventAggregation;
using TimePunch.MVVM.ViewModels;
using TimePunch.TpClientGui.Controls.Events;
using TimePunch.Wpf.Common.UI.Events;
using TimePunchIntroPlayer.Events;
using static System.Environment;

namespace TimePunchIntroPlayer
{
    public class MainViewModel : ViewModelBase
        , IHandleMessage<MediaOpenedEvent>
        , IHandleMessage<MediaFailedEvent>
        , IHandleMessage<VideoPositionChangedEvent>
    {
        public MainViewModel() : base(IntroPlayerKernel.Get().EventAggregator)
        {
        }

        /// <summary>
        /// Registeres every command and tells them what commands they are registered to
        /// </summary>
        public override void Initialize()
        {
            PlayVideoCommand = RegisterCommand(ExecutePlayVideoCommand, CanExecutePlayVideoCommand, true);
            PauseVideoCommand = RegisterCommand(ExecutePauseVideoCommand, CanExecutePauseVideoCommand, true);
            OpenMp4Command = RegisterCommand(ExecuteOpenMp4Command, CanExecuteOpenMp4Command, true);
            OpenAboutWindowCommand = RegisterCommand(ExecuteOpenAboutWindowCommand, CanExecuteOpenAboutWindowCommand, true);

            AddPropertyChangedNotification(() => IsVideoPlaying, PauseVideoCommand, PlayVideoCommand, OpenMp4Command);
            AddPropertyChangedNotification(() => IsVideoOpen, PauseVideoCommand, PlayVideoCommand);
        }

        /// <summary>
        /// Is called if a new window gets created
        /// </summary>
        /// <param name="extraData"></param>
        public override void InitializePage(object extraData)
        {

        }

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

        #region VideoPositionProperties

        public double VideoPosition
        {
            get { return GetPropertyValue(() => VideoPosition); }
            set
            {
                if (SetPropertyValue(() => VideoPosition, value))
                {
                    new Timer(VideoPositionChanged, value, 300, 0);
                }
            }
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

        //Methods
        #region VideoPositionMethod

        private void VideoPositionChanged(object state)
        {
            if (Math.Abs((double)state - VideoPosition) < 1000)
            {
                Dispatcher.Invoke(() =>
                {
                    IntroPlayerKernel.Get().EventAggregator.PublishMessage(new SeekToPlayerPositionEvent(VideoPosition));
                });
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

        public void ExecuteOpenMp4Command(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            var openFile = EventAggregator.PublishMessage(new OpenFileDialogEvent(GetFolderPath(SpecialFolder.MyVideos), "*.mp4", "Video File|*.mp4"));
            if (openFile.Result == true)
            {
                EventAggregator.PublishMessage(new OpenVideoEvent(openFile.FileName));
            }
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

        #region OpenAboutWindowCommand

        public ICommand OpenAboutWindowCommand
        {
            get { return GetPropertyValue(() => OpenAboutWindowCommand); }
            set { SetPropertyValue(() => OpenAboutWindowCommand, value); }
        }

        public void ExecuteOpenAboutWindowCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            EventAggregator.PublishMessage(new OpenAboutWindowEvent());
        }

        public void CanExecuteOpenAboutWindowCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        #endregion

        //Events
        #region Mediaevents
        public void Handle(MediaOpenedEvent message)
        {
            IsVideoPlaying = true;
            IsVideoOpen = true;
            VideoVolume = 50;
        }

        public void Handle(MediaFailedEvent message)
        {
            EventAggregator.PublishMessage(new MessageBoxEvent("Media failed", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation));
        }

        #endregion

        #region Userdefined Events

        public void Handle(VideoPositionChangedEvent positionChangedEvent)
        {
            SetPropertyValue(() => VideoPosition, positionChangedEvent.Position.TotalMilliseconds);
        }

        #endregion
    }
}
