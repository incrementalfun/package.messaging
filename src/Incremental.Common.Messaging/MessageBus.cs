using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Incremental.Common.Messaging
{
    internal class MessageBus : IMessageBus
    {
        private readonly ILogger<MessageBus> _logger;
        private readonly IBus _internalBus;
        
        public MessageBus(ILogger<MessageBus> logger, IBus internalBus)
        {
            _logger = logger;
            _internalBus = internalBus;
        }
        
        public async Task Send<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : Message
        {
            _logger.LogDebug("Sending {@Message}", message);
            
            await _internalBus.Publish(message, cancellationToken);
        }
    }
}