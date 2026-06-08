namespace Naples;

using System.Net;
using System.Net.Sockets;

public class Listener
{
    // The TcpListener
    private TcpListener _listener;

    /// <summary>
    /// Listener class constructor, initializes a new listener with a given port and Ip address.
    /// </summary>
    /// <param name="port">The port to access.</param>
    /// <param name="address">The Ip address of the connection.</param>
    /// <remarks>
    /// When not specified, defaults to <see cref="IPAddress.Any"/> (listens on all network interfaces).
    /// </remarks>
    public Listener(int port, IPAddress? address = null)
    {
        _listener = new(address ?? IPAddress.Any, port);
    }

    /// <summary>
    /// Starts the listener.
    /// </summary>
    public void Start()
    {
        _listener.Start();
    }

    /// <summary>
    /// Accepts a pending client connection asynchronously.
    /// </summary>
    /// <param name="token">Token used to cancel all running tasks.</param>
    /// <returns>A <see cref="Client"/> representing the accepted connection.</returns>
    public async Task<Client> AcceptClient(CancellationToken token = default)
    {
        // Wait for an incoming connection and wrap it in a Client object.
        TcpClient client = await _listener.AcceptTcpClientAsync(token);
        return new Client(client);
    }
}