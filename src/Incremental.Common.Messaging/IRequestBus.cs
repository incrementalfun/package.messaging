using System.Threading;
using System.Threading.Tasks;

namespace Incremental.Common.Messaging
{
    /// <summary>
    ///     Request bus.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRequestBus<in TRequest, TResponse> where TRequest : Request<TResponse>
    {
        /// <summary>
        ///     Get a response from the request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TResponse> GetResponse(TRequest request, CancellationToken cancellationToken = default);
    }
}