using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Incremental.Common.Messaging
{
    internal class MessageBus : IMessageBus
    {
        private readonly IBus _internalBus;
        private readonly ILogger<MessageBus> _logger;

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

        public async Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : ExternalEvent
        {
            _logger.LogDebug("Publishing {@Event}", @event);

            await _internalBus.Publish(@event, cancellationToken);
        }
    }
}