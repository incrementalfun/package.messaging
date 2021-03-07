using System;
using System.Linq;
using Incremental.Common.Queues.Client;
using Incremental.Common.Queues.Hosted.Client;
using Incremental.Common.Queues.Hosted.Hosted;
using Incremental.Common.Queues.Hosted.Messages;
using Incremental.Common.Queues.Hosted.Options;
using Incremental.Common.Queues.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Incremental.Common.Queues.Hosted
{
    /// <summary>
    ///     Registers queue management.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Registers a hosted service to handle incoming messages of a specific queue.
        /// </summary>
        /// <returns>
        ///     <see cref="IServiceCollection" />
        /// </returns>
        public static IServiceCollection AddQueuesHostedServices(this IServiceCollection services, Action<CommonQueuesOptions> hostedOptions)
        {
            var options = new CommonQueuesOptions();

            hostedOptions.Invoke(options);

            if (string.IsNullOrWhiteSpace(options.QueueEndpoint)) throw new ArgumentException("QueueEndpoint is a required argument.");

            if (options.SupportedMessageTypes.Any() && options.SupportedMessageTypes.Values.All(t => t.IsAssignableTo(typeof(Message))))
            {
                foreach (var registeredMessageType in options.SupportedMessageTypes.Values)
                    services.AddScoped(typeof(Message), registeredMessageType);
                
                services.AddScoped<IMessageBus, MessageBus>();
                
                services.AddScoped<IQueueSender, QueueClient>();
                services.AddScoped<IQueueReceiver, QueueClient>();
                
                services.AddHostedService<QueueHostedService>();
            }

            return services;
        }
    }
}