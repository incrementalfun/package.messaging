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
        /// <param name="services">The <see cref="IServiceCollection"/> to attach to.</param>
        /// <param name="hostedOptions">Hosted options.</param>
        /// <param name="assemblies">All assemblies with handlers.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        /// <exception cref="ArgumentException">When the queue endpoint is null or empty.</exception>
        public static IServiceCollection AddQueuesHostedServices(this IServiceCollection services, Action<MessagingOptions> hostedOptions, params Assembly[] assemblies)
        {
            var options = new MessagingOptions();

            hostedOptions.Invoke(options);

            if (string.IsNullOrWhiteSpace(options.QueueEndpoint)) throw new ArgumentException("QueueEndpoint is a required argument.");

            if (options.SupportedMessageTypes.Any() && options.SupportedMessageTypes.Values.All(t => t.IsAssignableTo(typeof(Message))))
            {
                foreach (var registeredMessageType in options.SupportedMessageTypes.Values)
                    services.AddScoped(typeof(Message), registeredMessageType);

                services.AddMediatR(assemblies);
                
                services.AddHostedService<QueueHostedService>();
            }

            return services;
        }
    }
}