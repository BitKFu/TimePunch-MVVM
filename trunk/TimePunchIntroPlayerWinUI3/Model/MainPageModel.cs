using Windows.Storage.Streams;

namespace TimePunchIntroPlayerWinUI3.Model
{
    public class MainPageModel
    {
        public MainPageModel(double videoPosition, double videoVolume, string videoType, bool isVideoPlaying, bool isVideoOpen, IRandomAccessStream fileStream)
        {
            VideoPosition = videoPosition;
            FileType = videoType;
            IsVideoPlaying = isVideoPlaying;
            IsVideoOpen = isVideoOpen;
            FileStream = fileStream;
        }

        #region VideoProperties

        public double VideoPosition { get; set; }

        public double VideoVolume { get; set; }

        public bool IsVideoPlaying { get; set; }

        public bool IsVideoOpen { get; set; }

        #endregion

        #region FileProperties

        public string FileType { get; set; }

        public IRandomAccessStream FileStream { get; set; }

        #endregion
    }
}
