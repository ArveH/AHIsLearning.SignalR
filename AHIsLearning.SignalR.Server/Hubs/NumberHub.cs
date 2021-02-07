using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace AHIsLearning.SignalR.Server.Hubs
{
    public class NumberHub : Hub
    {
        public ChannelReader<int> GetNumber(int numberCount, CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<int>();

            // We don't want to await WriteItemsAsync, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.
            _ = WriteNumberAsync(channel.Writer, numberCount, cancellationToken);

            return channel.Reader;
        }

        private async Task WriteNumberAsync(ChannelWriter<int> channelWriter, int numberCount, CancellationToken cancellationToken)
        {
            Exception localException = null;
            try
            {
                for (var i = 0; i < numberCount; i++)
                {
                    await channelWriter.WriteAsync(i, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                localException = ex;
            }
            finally
            {
                channelWriter.Complete(localException);
            }
        }
    }
}