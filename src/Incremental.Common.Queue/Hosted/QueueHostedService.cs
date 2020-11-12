using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Incremental.Common.Queue.Hosted.Options;
using Incremental.Common.Queue.Service.Contract;
using Incremental.Common.Sourcing.Events.Contract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Incremental.Common.Queue.Hosted
{
    /// <summary>
    /// Queue service that retrieves events from the queue and launches them as external events.
    /// </summary>
    public class QueueHostedService : BackgroundService
    {
        private readonly ILogger<QueueHostedService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly QueueOptions _queueOptions;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="scopeFactory"></param>
        /// <param name="queueOptions"></param>
        public QueueHostedService(ILogger<QueueHostedService> logger, IServiceScopeFactory scopeFactory, IOptions<QueueOptions> queueOptions)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _queueOptions = queueOptions.Value;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var outerServiceScope = _scopeFactory.CreateScope();

                var queueReceiver = outerServiceScope.ServiceProvider.GetRequiredService<IQueueReceiver>();

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(5000, stoppingToken);

                    _logger.LogInformation("Counting messages in queue.");
                
                    var messagesInQueue = await queueReceiver.Count(Queues.Services, stoppingToken);

                    while (messagesInQueue > 0)
                    {
                        _logger.LogInformation("Found {Count} messages in queue.", messagesInQueue);

                        var message = await queueReceiver.Receive(Queues.Services, 1, stoppingToken);

                        if (string.IsNullOrWhiteSpace(message.MessageId))
                        {
                            messagesInQueue = 0;
                            continue;
                        }
                    
                        if (_queueOptions.TypeDictionary.TryGetValue(message.MessageType ?? string.Empty, out var type))
                        {
                            using var innerServiceScope = _scopeFactory.CreateScope();
                        
                            var eventBus = innerServiceScope.ServiceProvider.GetRequiredService<IEventBus>();

                            var @event = JsonSerializer.Deserialize(message.Body, type);

                            try
                            {
                                await eventBus.Publish(@event as IExternalEvent);

                                await queueReceiver.MarkAsDelivered(Queues.Services, message.ReceiptHandle, stoppingToken);
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Unhandled exception handling external event from queue. (@event)", @event);
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