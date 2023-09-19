using TimePunch.MVVM.EventAggregation;

namespace TimePunchIntroPlayerWinUI3.Core
{
    public class TimePunchIntroPlayerWinUi3Kernel
    {
        private static TimePunchIntroPlayerWinUi3Kernel IntroPlayerUWP;

        private TimePunchIntroPlayerWinUi3Kernel()
        {
            EventAggregator = new EventAggregator();
        }

        public static TimePunchIntroPlayerWinUi3Kernel Get()
        {
            if (IntroPlayerUWP == null)
            {
                IntroPlayerUWP = new TimePunchIntroPlayerWinUi3Kernel();
                return IntroPlayerUWP;
            }
            return IntroPlayerUWP;
        }

        public IEventAggregator EventAggregator;
    }
}
