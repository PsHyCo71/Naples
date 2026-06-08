namespace Naples.Test;

using Naples;
using Xunit;

public class TcpTests
{
    public Runtime runtime = new Runtime(2);
    public Listener listener = new Listener(12345);

    // Note: TCP tasks are tested directly without the Naples scheduler
    // because the current scheduler does not support async I/O natively
    [Fact]
    public async Task ListenerTest()
    {
        string? message = null;

        var server = Task.Run(async () =>
        {
            listener.Start();
            var client = await listener.AcceptClient();
            message = await client.ReadAsync();
        });

        var clientTask = Task.Run(async () =>
        {
            var client = await Client.ConnectAsync("localhost", 12345);
            await client.WriteAsync("Hy from Naples!");
        });

        await Task.WhenAll(server, clientTask);
        Assert.Equal("Hy from Naples!", message);
    }
}