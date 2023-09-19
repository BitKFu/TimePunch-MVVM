using System;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Popups;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TimePunch.MVVM.EventAggregation;
using TimePunchIntroPlayerWinUI3.Core;
using TimePunchIntroPlayerWinUI3.Events.Exceptions;
using TimePunchIntroPlayerWinUI3.Events.MainPage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TimePunchIntroPlayerWinUI3.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
        , IHandleMessage<OpenVideoEvent>
        , IHandleMessage<PauseVideoEvent>
        , IHandleMessage<PlayVideoEvent>
        , IHandleMessage<ChangeVolumeEvent>
        , IHandleMessage<SeekToPlayerPositionEvent>
        , IHandleMessageAsync<MessageBoxException>
        , IDisposable
    {

        public MainPage()
        {
            InitializeComponent();
            TimePunchIntroPlayerWinUi3Kernel.Get().EventAggregator.Subscribe(this);

            PositionUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            PositionUpdateTimer.Tick += OnUpdatePosition;

            VideoPlayer.SetMediaPlayer(new MediaPlayer());
            VideoPlayer.Loaded += (v, e) =>
            {
                VideoPlayer.MediaPlayer.MediaEnded += VideoPlayer_MediaEnded;
                VideoPlayer.MediaPlayer.MediaFailed += VideoPlayer_MediaFailed;
                VideoPlayer.MediaPlayer.MediaOpened += VideoPlayer_MediaOpened;
            };
        }

        #region Fields

        private TimeSpan OldPosition;

        private double currentPosition;

        #endregion

        //Properties
        #region Controlling Videoposition Properties

        public DispatcherTimer PositionUpdateTimer { get; private set; }

        private void OnUpdatePosition(object sender, object e)
        {
            if (VideoPlayer != null)
            {
                if (OldPosition != VideoPlayer.MediaPlayer.Position)
                {
                    TimePunchIntroPlayerWinUi3Kernel.Get().EventAggregator.PublishMessage(new VideoPositionChangedEvent(VideoPlayer.MediaPlayer.Position));
                    OldPosition = VideoPlayer.MediaPlayer.Position;
                }
            }
        }

        #endregion

        #region PlayVideoProperties

        public void Handle(OpenVideoEvent message)
        {
            if (message.FileStream != null)
            {
                VideoPlayer.MediaPlayer.SetStreamSource(message.FileStream);

                currentPosition = message.Position;

                PositionUpdateTimer.Start();
            }
        }

        public void Handle(PauseVideoEvent message)
        {
            VideoPlayer.MediaPlayer.Pause();
            PositionUpdateTimer.Stop();
        }

        public void Handle(PlayVideoEvent message)
        {
            VideoPlayer.MediaPlayer.Play();
            PositionUpdateTimer.Start();
        }


        #endregion

        #region MediaEvents

        private void VideoPlayer_MediaEnded(MediaPlayer sender, object e)
        {
            VideoPlayer.MediaPlayer.Position = TimeSpan.Zero;
            VideoPlayer.MediaPlayer.Play();
        }

        private void VideoPlayer_MediaFailed(MediaPlayer sender, object e)
        {
            TimePunchIntroPlayerWinUi3Kernel.Get().EventAggregator.PublishMessage(new MediaFailedEvent());
        }

        private void VideoPlayer_MediaOpened(MediaPlayer sender, object e)
        {
            //if (!VideoPlayer.MediaPlayer.NaturalDuration != null)
            //{
            //    return;
            //}

            VideoPlayer.DispatcherQueue.TryEnqueue(() =>
            {

                if (VideoPlayer.MediaPlayer.CanSeek)
                {
                    var timeSpan = TimeSpan.FromMilliseconds(currentPosition);
                    VideoPlayer.MediaPlayer.Position = timeSpan;
                    VideoPlayer.MediaPlayer.Play();
                }

                TimePunchIntroPlayerWinUi3Kernel.Get().EventAggregator.PublishMessage(new MediaOpenedEvent());
                VideoPositionDefiner.Maximum = VideoPlayer.MediaPlayer.NaturalDuration.TotalMilliseconds;
            });
        }

        #endregion

        //Events that get called by the users action
        #region UserperformedEvents

        public void Handle(SeekToPlayerPositionEvent message)
        {
            try
            {
                var timeSpan = TimeSpan.FromMilliseconds(message.Position);
                VideoPlayer.MediaPlayer.Position = timeSpan;
            }
            catch (Exception e)
            {
                TimePunchIntroPlayerWinUi3Kernel.Get().EventAggregator.PublishMessage(new MessageBoxException(e.Message));
            }
        }

        async Task<MessageBoxException> IHandleMessageAsync<MessageBoxException>.Handle(MessageBoxException message)
        {
            var errorDialog = new MessageDialog(message.ExceptionMessage);
            await errorDialog.ShowAsync();

            return message;
        }

        public void Handle(ChangeVolumeEvent message)
        {
            VideoPlayer.MediaPlayer.Volume = message.Volume / 100.0;
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    TimePunchIntroPlayerWinUi3Kernel.Get().EventAggregator.Unsubscribe(this);
                    PositionUpdateTimer.Stop();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }


        #endregion
    }
}
