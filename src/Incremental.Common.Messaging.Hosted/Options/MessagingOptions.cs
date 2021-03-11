using System;
using System.Collections.Generic;

namespace Incremental.Common.Messaging.Hosted.Options
{
    /// <summary>
    ///     Common options for hosted queue service.
    /// </summary>
    public class MessagingOptions
    {
        public static string Messaging = "Messaging";
        
        /// <summary>
        ///     Queue endpoint.
        /// </summary>
        public string QueueEndpoint { get; set; }
        
        /// <summary>
        ///     Event bus.
        /// </summary>
        public string EventBus { get; set; }
    }
}