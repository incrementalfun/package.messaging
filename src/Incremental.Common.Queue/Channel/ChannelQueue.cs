using System.Threading.Channels;
using Incremental.Common.Queue.Channel.Contract;

namespace Incremental.Common.Queue.Channel
{
    public class ChannelQueue<T> : IQueueReader<T>, IQueueWriter<T>
    {
        private readonly Channel<T> _queue;

        public ChannelReader<T> Reader => _queue.Reader;
        public ChannelWriter<T> Writer => _queue.Writer;
        
        public ChannelQueue()
        {
            _queue = System.Threading.Channels.Channel.CreateUnbounded<T>();
        }
    }
}