namespace Naples.Core;

public class Scheduler
{
    private readonly PriorityQueue<Func<CancellationToken, Task>, int> _queue = new();

    public event Action<Exception>? OnTaskFailed;

    public void Enqueue(Func<CancellationToken, Task> task, int priority)
    {
        _queue.Enqueue(task, priority);
    }

    public void Run( int priority, CancellationToken token = default)
    {
        while (_queue.Count > 0)
        {
            var toRunTask = _queue.Dequeue();
            try
            {
                var task = toRunTask(token);
                if (!task.IsCompleted)
                    _queue.Enqueue(toRunTask, priority);
            }
            catch (Exception ex)
            {
                OnTaskFailed?.Invoke(ex);
                continue;
            }
        }
    }
}