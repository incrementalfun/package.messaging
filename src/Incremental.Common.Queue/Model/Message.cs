using System.Text.Json;
using Incremental.Common.Sourcing.Events.Contract;

namespace Incremental.Common.Queue.Model
{
    /// <summary>
    /// Queue message.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Type of the event.
        /// </summary>
        public string EventType { get; set; }
        /// <summary>
        /// External event.
        /// </summary>
        public object EventData { get; set; }

        /// <summary>
        /// Creates a new message from an external event.
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public static Message FromExternalEvent(IExternalEvent @event)
        {
            return new Message
            {
                EventType = @event.GetType().AssemblyQualifiedName,
                EventData = @event
            };
        }

        /// <summary>
        /// Creates a new message from a serialized message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Message FromSerialized(string message)
        {
            return JsonSerializer.Deserialize<Message>(message);
        }
    }
}