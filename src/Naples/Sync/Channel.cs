namespace Naples.Sync;
public class Channel<T>
{
    private Queue<T> _queue = new();

    public void Send(T value)
    {
        _queue.Enqueue(value);
    }

    public bool TryReceive(out T value)
    {
        if (_queue.Count == 0)
        {
            value = default!;
            return false;
        }

        value = _queue.Dequeue();
        return true;
    }
}