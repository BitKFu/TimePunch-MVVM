using System.Threading.Tasks;

namespace TimePunch.MVVM.EventAggregation
{
    /// <summary>
    /// Marks the implementer as a message handler of a specific message type. Can 
    /// be implemented multiple times.
    /// </summary>
    /// <typeparam name="TMessage">the type of message to handle</typeparam>
    public interface IHandleMessageAsync<TMessage>
    {
        /// <summary>
        /// Handles a message of a specific type.
        /// </summary>
        /// <param name="message">the message to handle</param>
        Task<TMessage> Handle(TMessage message);
    }
}
    
