using System;
using System.Linq;
using System.Reflection;
using Incremental.Common.Messaging.Client;
using Incremental.Common.Messaging.Handling;
using Incremental.Common.Messaging.Hosted.Hosted;
using Incremental.Common.Messaging.Hosted.Options;
using Incremental.Common.Messaging.Messages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Incremental.Common.Messaging.Hosted
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
        public static IServiceCollection AddQueuesHostedServices(this IServiceCollection services, Action<MessagingOptions> hostedOptions)
        {
            var options = new MessagingOptions();

            hostedOptions.Invoke(options);

            if (string.IsNullOrWhiteSpace(options.QueueEndpoint)) throw new ArgumentException("QueueEndpoint is a required argument.");

            if (options.SupportedMessageTypes.Any() && options.SupportedMessageTypes.Values.All(t => t.IsAssignableTo(typeof(Message))))
            {
                foreach (var registeredMessageType in options.SupportedMessageTypes.Values)
                    services.AddScoped(typeof(Message), registeredMessageType);
                
                services.AddHostedService<QueueHostedService>();
            }

            return services;
        }
    }
}