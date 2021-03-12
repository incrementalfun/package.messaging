using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Messaging.Client;
using Incremental.Common.Messaging.Hosted.Options;
using Incremental.Common.Messaging.Hosted.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Incremental.Common.Messaging.Hosted.Hosted
{
    internal class MessagingHostedService : BackgroundService
    {
        private readonly ILogger<MessagingHostedService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMessageDeserializer _messageDeserializer;
        private readonly MessagingOptions _options;

        public MessagingHostedService(ILogger<MessagingHostedService> logger, IServiceScopeFactory scopeFactory,
            IMessageDeserializer messageDeserializer, IOptions<MessagingOptions> options
        )
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _messageDeserializer = messageDeserializer;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var outerServiceScope = _scopeFactory.CreateScope();

            try
            {
                var queueReceiver = await outerServiceScope.ServiceProvider.GetRequiredService<IMessagingClientFactory>()
                    .GetReceiver(_options.QueueEndpoint, stoppingToken);

                var visibility = await queueReceiver.GetVisibilityTimeSpan(stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(5000, stoppingToken);

                    var messagesInQueue = await queueReceiver.Count(stoppingToken);

                    while (messagesInQueue > 0)
                    {
                        _logger.LogDebug("{MessageCount} messages in queue", messagesInQueue);

                        var message = await queueReceiver.Receive(1, stoppingToken);

                        var cancellationTokenSource = new CancellationTokenSource(visibility.Subtract(TimeSpan.FromSeconds(5)));

                        if (string.IsNullOrWhiteSpace(message.receipt.id))
                        {
                            messagesInQueue = 0;
                            continue;
                        }

                        try
                        {
                            await TryHandleMessage(message, cancellationTokenSource);

                            messagesInQueue--;
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Unhandled exception handling {@Message}", message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Unhandled critical exception handling message queue");
            }
        }

        private async Task TryHandleMessage((string Body, string Type, (string Queue, string Id) receipt) message,
            CancellationTokenSource cancellationTokenSource)
        {
            if (_messageDeserializer.TryGetType(message.Type, out var type))
            {
                using var innerServiceScope = _scopeFactory.CreateScope();

                var sender = innerServiceScope.ServiceProvider.GetRequiredService<ISender>();

                if (JsonSerializer.Deserialize(message.Body, type) is Message request)
                {
                    request = request with {Receipt = message.receipt};

                    try
                    {
                        await sender.Send(request, cancellationTokenSource.Token);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unhandled exception handling message from queue. ({@Message})", message);
                    }
                }
            }
        }
    }
}