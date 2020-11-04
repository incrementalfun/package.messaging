using System;
using System.Collections.Generic;

namespace Incremental.Common.Queue.Hosted.Options
{
    /// <summary>
    /// Queue related options.
    /// </summary>
    public class QueueOptions
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public QueueOptions()
        {
            TypeDictionary = new Dictionary<string, Type>();
        }

        /// <summary>
        /// Dictionary of types which the hosted service should react to.
        /// </summary>
        public IDictionary<string, Type> TypeDictionary { get; }
    }
}