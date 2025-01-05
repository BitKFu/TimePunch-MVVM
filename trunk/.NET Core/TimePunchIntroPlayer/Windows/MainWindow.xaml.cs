using System;
using System.Threading;
using System.Windows;
using TimePunch.MVVM.EventAggregation;
using TimePunchIntroPlayer.Events;

namespace TimePunchIntroPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
        , IHandleMessage<OpenVideoEvent>
        , IHandleMessage<PauseVideoEvent>
        , IHandleMessage<PlayVideoEvent>
        , IHandleMessage<SeekToPlayerPositionEvent>
        , IHandleMessage<ChangeVolumeEvent>
    {

        public MainWindow()
        {
            InitializeComponent();
            IntroPlayerKernel.Instance.EventAggregator.Subscribe(this);
            PositionUpdateTimer = new Timer(OnUpdatePosition, null, 0, 1000);
        }

        ~MainWindow()
        {
            IntroPlayerKernel.Instance.EventAggregator.Unsubscribe(this);
        }

        private static TimeSpan oldPosition;


        private void OnUpdatePosition(object state)
        {
            Dispatcher.Invoke(() =>
            {
                if (oldPosition != VideoPlayer.Position)
                {
                    IntroPlayerKernel.Instance.EventAggregator.PublishMessage(new VideoPositionChangedEvent(VideoPlayer.Position));
                    oldPosition = VideoPlayer.Position;
                }
            });
        }

        private Timer PositionUpdateTimer { get; set; }

        //Properties
        #region PlayVideoProperties

        public void Handle(OpenVideoEvent openVideoEvent)
        {
            VideoPlayer.Source = new Uri(openVideoEvent.FileName);
            VideoPlayer.Play();
        }

        public void Handle(PauseVideoEvent pauseVideoEvent)
        {
            VideoPlayer.Pause();
        }

        public void Handle(PlayVideoEvent playVideoEvent)
        {
            VideoPlayer.Play();
        }

        #endregion

        //Events
        #region Mediaevents

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (!VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                return;
            }

            IntroPlayerKernel.Instance.EventAggregator.PublishMessage(new MediaOpenedEvent());
            VideoPositonDefiner.Maximum = VideoPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Position = TimeSpan.Zero;
            VideoPlayer.Play();
        }

        private void VideoPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            IntroPlayerKernel.Instance.EventAggregator.PublishMessage(new MediaFailedEvent());
        }

        #endregion

        #region UserperformedEvents

        public void Handle(SeekToPlayerPositionEvent playerPositionEvent)
        {

            var timeSpan = TimeSpan.FromMilliseconds(playerPositionEvent.Position);
            VideoPlayer.Position = timeSpan;
        }

        public void Handle(ChangeVolumeEvent videoVolumeEvent)
        {
            VideoPlayer.Volume = videoVolumeEvent.Volume / 100.0;
        }

        #endregion

    }
}
