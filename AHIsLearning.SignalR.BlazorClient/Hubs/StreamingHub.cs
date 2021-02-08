using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AHIsLearning.SignalR.BlazorClient.Hubs
{
    public class StreamingHub : Hub
    {
        public ChannelReader<SomeData> GetSomeDataWithChannelReader(
            int count,
            int delay,
            CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<SomeData>();
            _ = WriteItemsAsync(channel.Writer, count, delay, cancellationToken);
            return channel.Reader;
        }

        private async Task WriteItemsAsync(
            ChannelWriter<SomeData> writer,
            int count,
            int delay,
            CancellationToken cancellationToken)
        {
            try
            {
                for (var i = 0; i < count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await writer.WriteAsync(new SomeData() { Value = i });
                    await Task.Delay(delay, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                writer.TryComplete(ex);
            }

            writer.TryComplete();
        }
    }

    public class SomeData
    {
        public int Value { get; set; }

        public override string ToString() => Value.ToString();
    }
}