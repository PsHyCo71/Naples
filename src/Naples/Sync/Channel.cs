namespace Naples;
using System.Collections.Concurrent;


public class Channel<T>
{
    // The ConcurrentQueue.
    private ConcurrentQueue<T> _queue = new();

    /// <summary>
    /// Enqueues a T value in the queue.
    /// </summary>
    /// <param name="value">The value to send through the channel.</param>
    public void Send(T value)
    {
        _queue.Enqueue(value);
    }

    /// <summary>
    /// Tries to dequeue a t value from the queue.
    /// </summary>
    /// <param name="value">The received value, or default if the channel is empty.</param>
    /// <returns>True if the value is dequeued, otherwise false.</returns>
    public bool TryReceive(out T? value)
    {
        return _queue.TryDequeue(out value);
    }
}