using System.Net.Sockets;
using System.Text;

namespace Naples;

public class Client
{
    // The TcpClient.
    private TcpClient _client;

    /// <summary>
    /// Client class constructor, initializes the client.
    /// </summary>
    /// <param name="client">TcpClient of the user.</param>
    public Client(TcpClient client)
    {
        _client = client;
    }
    
    /// <summary>
    /// Gets the client's stream, reads the information and turns it into as string.
    /// </summary>
    /// <param name="token">Token used to cancel all running tasks.</param>
    /// <returns>A string of the read information.</returns>
    public async Task<string> ReadAsync(CancellationToken token = default)
    {
        var stream = _client.GetStream();
        byte[] buffer = new byte[1024];
        int intBytes = await stream.ReadAsync(buffer, token);
        return Encoding.UTF8.GetString(buffer, 0, intBytes);
    }

    /// <summary>
    /// Gets the client's stream, the bytes of the given data and writes them in the stream.
    /// </summary>
    /// <param name="data">A string of data.</param>
    /// <param name="token">Token used to cancel all running tasks.</param>
    public async Task WriteAsync(string data, CancellationToken token = default)
    {
        var stream = _client.GetStream();
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        await stream.WriteAsync(dataBytes, token);
    }

    /// <summary>
    /// Creates a new TCP connection to the specified host and port.
    /// </summary>
    /// <param name="host">Host name.</param>
    /// <param name="port">The port to access.</param>
    /// <param name="token">Token used to cancel all running tasks.</param>
    /// <returns>A <see cref="Client"/> connected to the specified host and port.</returns>
    public static async Task<Client> ConnectAsync(string host, int port, CancellationToken token = default)
    {
        TcpClient client = new TcpClient();
        await client.ConnectAsync(host, port, token);
        return new Client(client);
    }
}