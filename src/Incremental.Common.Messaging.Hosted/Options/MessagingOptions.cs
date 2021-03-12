using System;
using System.Collections.Generic;

namespace Incremental.Common.Messaging.Hosted.Options
{
    /// <summary>
    ///     Common options for hosted queue service.
    /// </summary>
    public class MessagingOptions
    {
        /// <summary>
        ///     Key for the configuration provider.
        /// </summary>
        public static readonly string Messaging = "Messaging";
        
        /// <summary>
        ///     Queue endpoint.
        /// </summary>
        public string Queue { get; set; }
        
        /// <summary>
        ///     Event bus.
        /// </summary>
        public string EventBus { get; set; }
    }
}