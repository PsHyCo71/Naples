namespace Naples.Tests;

using Naples.Sync;
using Xunit;

public class ChannelTests
{
    [Fact]
    public void SendReceiveTest()
    {
        var channel = new Channel<int>();
        channel.Send(2);
        channel.TryReceive(out var value);
        Assert.Equal(2, value);
    }

    [Fact]
    public void EmptyQueueTests()
    {
        var channel = new Channel<int>();
        bool received = channel.TryReceive(out var value);
        Assert.False(received);
    }
}