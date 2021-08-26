using System.Threading;
using System.Threading.Tasks;
using MassTransit;

namespace Incremental.Common.Messaging
{
    internal class RequestBus<TRequest, TResponse> : IRequestBus<TRequest, TResponse>
        where TRequest : Request<TResponse> where TResponse : class
    {
        private readonly IRequestClient<TRequest> _requestClient;

        public RequestBus(IRequestClient<TRequest> requestClient)
        {
            _requestClient = requestClient;
        }

        /// <inheritdoc />
        public async Task<TResponse> GetResponse(TRequest request, CancellationToken cancellationToken = default)
        {
            return (await _requestClient.GetResponse<TResponse>(request, cancellationToken)).Message;
        }
    }
}