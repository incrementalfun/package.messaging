using System.Reflection;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.SQS;
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
        /// TODO:
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection AddQueuesHostedServices(this IServiceCollection services, )
    }
}