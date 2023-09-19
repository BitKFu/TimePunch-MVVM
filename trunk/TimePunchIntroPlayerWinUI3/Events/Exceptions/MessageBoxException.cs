namespace TimePunchIntroPlayerWinUI3.Events.Exceptions
{
    public class MessageBoxException
    {
        public MessageBoxException(string exceptionMessage)
        {
            ExceptionMessage = exceptionMessage;
        }

        public string ExceptionMessage { get; set; }
    }
}
