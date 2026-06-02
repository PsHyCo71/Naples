using System.Collections.Concurrent;

namespace Naples.Sync;
public class Channel<T>
{
    private ConcurrentQueue<T> _queue = new();

    public void Send(T value)
    {
        _queue.Enqueue(value);
    }

    public bool TryReceive(out T? value)
    {
        return _queue.TryDequeue(out value);
    }
}