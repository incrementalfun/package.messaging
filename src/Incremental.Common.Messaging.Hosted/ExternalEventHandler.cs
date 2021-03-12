using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.EventBridge;
using Amazon.EventBridge.Model;
using Incremental.Common.Messaging.Hosted.Options;
using Incremental.Common.Sourcing.Events.Contract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Incremental.Common.Messaging.Hosted
{
    internal class ExternalEventHandler<TExternalEvent> : IEventHandler<TExternalEvent> where TExternalEvent : IExternalEvent
    {
        private readonly IAmazonEventBridge _eventBridge;
        private readonly ILogger _logger;
        private readonly MessagingOptions _options;

        public ExternalEventHandler(IAmazonEventBridge eventBridge, IOptions<MessagingOptions> options, ILogger logger)
        {
            _eventBridge = eventBridge;
            _logger = logger;
            _options = options.Value;
        }

        public async Task Handle(TExternalEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _eventBridge.PutEventsAsync(new PutEventsRequest
                {
                    Entries = new List<PutEventsRequestEntry>
                    {
                        new()
                        {
                            DetailType = notification.GetType().FullName,
                            EventBusName = _options.EventBus,
                            Source = Assembly.GetEntryAssembly()?.GetName().Name,
                            Detail = JsonSerializer.Serialize(notification)
                        }
                    }
                }, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unhandled exception sending {@Event} to bus", notification);
            }
        }
    }
}