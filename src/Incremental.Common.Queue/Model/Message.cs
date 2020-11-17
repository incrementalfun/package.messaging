using System.Collections.Generic;
using System.Text.Json;

namespace Incremental.Common.Queue.Model
{
    /// <summary>
    /// Queue message.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Attributes.
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; }
        
        /// <summary>
        /// Message id.
        /// </summary>
        public string MessageId { get; set; }
        
        /// <summary>
        /// Receipt handle id.
        /// </summary>
        public string ReceiptHandle { get; set; }

        /// <summary>
        /// Body of the message.
        /// </summary>
        public string Body { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string MessageType { get; set; }
        
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Message()
        {
            Attributes = new Dictionary<string, string>();
        }
        
        /// <summary>
        /// Creates a new message from an external event.
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public static Message FromExternalEvent(object @event)
        {
            var message = new Message
            {
                MessageType = @event.GetType().AssemblyQualifiedName,
                Body = JsonSerializer.Serialize(@event)
            };

            return message;
        }
    }
}