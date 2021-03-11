using System;
using System.Collections.Generic;

namespace Incremental.Common.Messaging.Hosted.Services
{
    internal class MessageDeserializer : IMessageDeserializer
    {
        private readonly Dictionary<string, Type> _supportedTypes;

        public MessageDeserializer(Dictionary<string, Type> supportedTypes)
        {
            _supportedTypes = supportedTypes;
        }

        public bool TryGetType(string typeName, out Type type)
        {
            return _supportedTypes.TryGetValue(typeName, out type);
        }
    }
}