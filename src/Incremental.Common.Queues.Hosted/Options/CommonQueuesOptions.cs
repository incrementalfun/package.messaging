using System;
using System.Collections.Generic;

namespace Incremental.Common.Queues.Hosted.Options
{
    public class CommonQueuesOptions
    {
        public string QueueEndpoint { get; set; }
        public Dictionary<string, Type> RegisteredMessageTypes { get; set; }
    }
}