﻿using System;
using System.Collections.Generic;
using Incremental.Common.Queues.Messages;

namespace Incremental.Common.Queues.Hosted.Options
{
    /// <summary>
    ///     Common options for hosted queue service.
    /// </summary>
    public class CommonQueuesOptions
    {
        internal CommonQueuesOptions()
        {
            SupportedMessageTypes = new Dictionary<string, Type>();
        }

        /// <summary>
        ///     Queue endpoint.
        /// </summary>
        public string QueueEndpoint { get; set; }

        /// <summary>
        ///     Types of messages that this queue will handle.
        /// </summary>
        /// <remarks>
        ///     To add a message type to this collection please use ConfigureSupportFor.
        /// </remarks>
        public readonly Dictionary<string, Type> SupportedMessageTypes;

        /// <summary>
        /// Adds a message type to the collection of supported types.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        public void ConfigureSupportFor<TMessage>() where TMessage : Message
        {
            SupportedMessageTypes.TryAdd(typeof(TMessage).FullName, typeof(TMessage));
        }
        
        
    }
}