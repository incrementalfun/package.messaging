using System.Threading.Channels;

namespace Incremental.Common.Queue.Channel.Contract
{
    /// <summary>
    /// Writes to a queue.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueueWriter<T>
    {
        /// <summary>
        /// Queue writer.
        /// </summary>
        ChannelWriter<T> Writer { get; }

    }
}