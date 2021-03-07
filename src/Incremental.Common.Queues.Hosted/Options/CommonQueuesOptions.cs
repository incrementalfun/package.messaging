using System;
using System.Collections.Generic;

namespace Incremental.Common.Queues.Hosted.Options
{
    /// <summary>
    /// Common options for hosted queue service.
    /// </summary>
    public class CommonQueuesOptions
    {
        /// <summary>
        /// Queue endpoint.
        /// </summary>
        public string QueueEndpoint { get; set; }
        
        /// <summary>
        /// Types of messages that this queue will handle.
        /// </summary>
        public Dictionary<string, Type> RegisteredMessageTypes { get; set; }
    }
}