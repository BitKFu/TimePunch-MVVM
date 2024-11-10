using TimePunch.MVVM.EventAggregation;

namespace TimePunchIntroPlayer
{
    public class IntroPlayerKernel
    {
        private static IntroPlayerKernel Singleton;

        private IntroPlayerKernel()
        {
            EventAggregator = new EventAggregator();
        }

        public static IntroPlayerKernel Get()
        {
            if (Singleton == null)
            {
                Singleton = new IntroPlayerKernel();
                return Singleton;
            }
            return Singleton;
        }

        public IEventAggregator EventAggregator { get; }
    }
}
