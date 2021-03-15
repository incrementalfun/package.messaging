﻿using System.Collections.Generic;
using System.Linq;
using MediatR;

namespace Incremental.Common.Messaging
{
    /// <summary>
    ///     Message.
    /// </summary>
    public record Message : IRequest
    {
        private readonly IList<Message> _innerQueue;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public Message()
        {
            _innerQueue = new List<Message>();
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
        public IEnumerable<Message> FollowingSteps => _innerQueue;


        /// <summary>
        ///     Sets up a message to fire if the initial message is a success.
        /// </summary>
        /// <param name="message"></param>
        public void FollowUpWith(Message message)
        {
            _innerQueue.Add(message);
        }
    }
}