using System.Threading;
using System.Threading.Tasks;

namespace Incremental.Common.Messaging
{
    public interface IMessageBus
    {
        Task Send<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : Message;
    }
}