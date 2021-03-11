using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Incremental.Common.Messaging.Handling;
using Incremental.Common.Messaging.Hosted.Hosted;
using Incremental.Common.Messaging.Hosted.Options;
using Incremental.Common.Messaging.Hosted.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
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
        /// <param name="configuration">The <see cref="IConfiguration"/> to source <see cref="MessagingOptions"/>.</param>
        /// <param name="assemblies">All assemblies with handlers.</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        /// <exception cref="ArgumentException">When the queue endpoint is null or empty.</exception>
        public static IServiceCollection AddMessagingHostedServices(this IServiceCollection services, IConfiguration configuration,
            params Assembly[] assemblies)
        {
            services.Configure<MessagingOptions>(configuration.GetSection(MessagingOptions.Messaging));

            services.AddMediatR(assemblies);

            var messageHandlers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.BaseType?.IsGenericType ?? false)
                .Where(type => type.BaseType.GetGenericTypeDefinition() == typeof(MessageHandler<>))
                .ToList();

            if (messageHandlers.Any())
            {
                var supportedMessages = new Dictionary<string, Type>();
                
                foreach (var handler in messageHandlers)
                {
                    var handledMessage = handler.BaseType?.GenericTypeArguments.FirstOrDefault();
                    
                    if (handledMessage?.BaseType is not null && handledMessage.BaseType == typeof(Message))
                    {
                        supportedMessages.TryAdd(handledMessage.BaseType.FullName, handledMessage.BaseType);
                    }
                }

                services.AddTransient<IMessageDeserializer>(_ => new MessageDeserializer(supportedMessages));

                services.AddHostedService<MessagingHostedService>();
            }

            return services;
        }
    }
}