using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Incremental.Common.Queue.Message.Contract;
using Incremental.Common.Queue.Service.Contract;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Incremental.Common.Queue.Hosted
{
    /// <summary>
    /// Queue service that retrieves events from the queue and launches them as external events.
    /// </summary>
    public class QueueHostedService : BackgroundService
    {
        private readonly ILogger<QueueHostedService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Dictionary<string, Type> _messageTypes;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="scopeFactory"></param>
        /// <param name="messageTypes"></param>
        public QueueHostedService(ILogger<QueueHostedService> logger, IServiceScopeFactory scopeFactory, IEnumerable<IMessage> messageTypes)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _messageTypes = messageTypes.ToDictionary(e => e.GetType().FullName, e => e.GetType());
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var outerServiceScope = _scopeFactory.CreateScope();

            try
            {
                var queueReceiver = outerServiceScope.ServiceProvider.GetRequiredService<IQueueReceiver>();

                var visibility = await queueReceiver.GetVisibilityTimeSpan(Queues.Services, stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(5000, stoppingToken);

                    var messagesInQueue = await queueReceiver.Count(Queues.Services, stoppingToken);

                    while (messagesInQueue > 0)
                    {
                        _logger.LogDebug("Found {Count} messages in queue.", messagesInQueue);

                        var message = await queueReceiver.Receive(Queues.Services, 1, stoppingToken);

                        var cancellationTokenSource = new CancellationTokenSource(visibility.Subtract(TimeSpan.FromSeconds(5)));

                        if (string.IsNullOrWhiteSpace(message.receipt.id))
                        {
                            messagesInQueue = 0;
                            continue;
                        }

                        if (_messageTypes.TryGetValue(message.type ?? string.Empty, out var type))
                        {
                            using var innerServiceScope = _scopeFactory.CreateScope();

                            var sender = innerServiceScope.ServiceProvider.GetRequiredService<ISender>();

                            if (JsonSerializer.Deserialize(message.body, type) is IMessage request)
                            {
                                request.Receipt = message.receipt;

                                try
                                {
                                    await sender.Send(request, cancellationTokenSource.Token);
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError(e, "Unhandled exception handling message from queue. ({@message})", message);
                                }
                            }
                        }

                        messagesInQueue--;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Unhandled critical exception receiving external events from queue.");
            }
        }
    }
}