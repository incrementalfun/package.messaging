using System;
using System.Linq;
using System.Reflection;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SQS;
using Incremental.Common.Queues.Hosted.Hosted;
using Incremental.Common.Queues.Hosted.Options;
using Incremental.Common.Queues.Messages;
using Incremental.Common.Queues.Messages.Contract;
using Incremental.Common.Queues.Service;
using Incremental.Common.Queues.Service.Contract;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Incremental.Common.Queues.Hosted
{
    /// <summary>
    ///     Registers queue management.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a hosted service to handle incoming messages of a specific queue.
        /// </summary>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddQueuesHostedServices(this IServiceCollection services, Action<CommonQueuesOptions> hostedOptions)
        {
            var options = new CommonQueuesOptions();
            
            hostedOptions.Invoke(options);

            if (string.IsNullOrWhiteSpace(options.QueueEndpoint))
            {
                throw new ArgumentException("QueueEndpoint is a required argument.");
            }

            if (options.RegisteredMessageTypes.Any() && options.RegisteredMessageTypes.Values.All(t => t.IsAssignableTo(typeof(Message))))
            {
                
                foreach (var registeredMessageType in options.RegisteredMessageTypes.Values)
                {
                    services.AddScoped(typeof(Message), registeredMessageType);
                }

                services.AddHostedService<QueueHostedService>();
            }
            
            return services;
        }
    }
}