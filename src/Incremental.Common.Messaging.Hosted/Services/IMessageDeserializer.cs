using System;

namespace Incremental.Common.Messaging.Hosted.Services
{
    internal interface IMessageDeserializer
    {
        public bool TryGetType(string typeName, out Type type);
    }
}