using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Queues.Client;
using Incremental.Common.Queues.Hosted.Options;
using Incremental.Common.Queues.Messages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Incremental.Common.Queues.Hosted.Hosted
{
    internal class QueueHostedService : BackgroundService
    {
        private readonly ILogger<QueueHostedService> _logger;
        private readonly CommonQueuesOptions _options;
        private readonly IServiceScopeFactory _scopeFactory;

        public QueueHostedService(ILogger<QueueHostedService> logger, IServiceScopeFactory scopeFactory, IOptions<CommonQueuesOptions> options)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var outerServiceScope = _scopeFactory.CreateScope();

            try
            {
                var queueReceiver = outerServiceScope.ServiceProvider.GetRequiredService<IQueueReceiver>();

                var visibility = await queueReceiver.GetVisibilityTimeSpan(_options.QueueEndpoint, stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(5000, stoppingToken);

                    var messagesInQueue = await queueReceiver.Count(_options.QueueEndpoint, stoppingToken);

                    while (messagesInQueue > 0)
                    {
                        _logger.LogDebug("{MessageCount} messages in queue", messagesInQueue);

                        var message = await queueReceiver.Receive(_options.QueueEndpoint, 1, stoppingToken);

                        var cancellationTokenSource = new CancellationTokenSource(visibility.Subtract(TimeSpan.FromSeconds(5)));

                        if (string.IsNullOrWhiteSpace(message.receipt.id))
                        {
                            messagesInQueue = 0;
                            continue;
                        }

                        await TryHandleMessage(message, cancellationTokenSource);

                        messagesInQueue--;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Unhandled critical exception receiving external events from queue");
            }
        }

        private async Task TryHandleMessage((string body, string type, (string queue, string id) receipt) message,
            CancellationTokenSource cancellationTokenSource)
        {
            if (MessageTypeIsRegistered(message, out var type))
            {
                using var innerServiceScope = _scopeFactory.CreateScope();

                var sender = innerServiceScope.ServiceProvider.GetRequiredService<ISender>();

                if (JsonSerializer.Deserialize(message.body, type) is Message request)
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

        private bool MessageTypeIsRegistered((string body, string type, (string queue, string id) receipt) message, out Type type)
        {
            return _options.SupportedMessageTypes.TryGetValue(message.type ?? string.Empty, out type);
        }
    }
}