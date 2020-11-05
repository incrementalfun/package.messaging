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
            using var serviceScope = _scopeFactory.CreateScope();

            var queueReceiver = serviceScope.ServiceProvider.GetRequiredService<IQueueReceiver>();
            var eventBus = serviceScope.ServiceProvider.GetRequiredService<IEventBus>();

            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await queueReceiver.Receive(Queues.Services, 1, stoppingToken);
                
                if (_queueOptions.TypeDictionary.TryGetValue(message.MessageType, out var type))
                {
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
            }
        }
    }
}