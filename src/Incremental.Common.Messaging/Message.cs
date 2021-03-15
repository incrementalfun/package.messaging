using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MediatR;

namespace Incremental.Common.Messaging
{
    /// <summary>
    ///     Message.
    /// </summary>
    public record Message : IRequest
    {
        private readonly IList<(string Type, string Message)> _innerQueue;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public Message()
        {
            _innerQueue = new List<(string, string)>();
        }

        /// <summary>
        ///     True if there are any messages to be launched as a follow up to this message.
        /// </summary>
        public bool HasFollowingSteps() => _innerQueue.Any();

        /// <summary>
        ///     Receipt of the message in the queue.
        /// </summary>
        public (string Queue, string Id) Receipt { get; init; }
        
        /// <summary>
        ///     
        /// </summary>
        public IEnumerable<(string Type, string Message)> FollowingSteps => _innerQueue;


        /// <summary>
        ///     Sets up a message to fire if the initial message is a success.
        /// </summary>
        /// <param name="message"></param>
        public void FollowUpWith<TMessage>(TMessage message) where TMessage : Message
        {
            _innerQueue.Add((typeof(TMessage).FullName, JsonSerializer.Serialize(message)));
        }
    }
}