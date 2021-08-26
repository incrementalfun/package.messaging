using System.Threading.Tasks;
using MassTransit;

namespace Incremental.Common.Messaging
{
    /// <summary>
    ///     Request handler.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRequestHandler<in TRequest, in TResponse> : IConsumer<TRequest> where TRequest : Request<TResponse> where TResponse : class
    {
        /// <summary>
        ///     Respond to a request.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        async Task Respond(ConsumeContext<TRequest> context, TResponse response)
        {
            await context.RespondAsync(response);
        }
    }
}