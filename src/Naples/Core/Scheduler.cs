namespace Naples.Core;

public class Scheduler
{
    private readonly PriorityQueue<(Func<CancellationToken, Task> task, int priority), int> _queue = new();

    public event Action<Exception>? OnTaskFailed;

    public int Count => _queue.Count;

    public void Enqueue(Func<CancellationToken, Task> task, int priority)
    {
        _queue.Enqueue((task, priority), priority);
    }

    public void Run(List<Scheduler> schedulers, CancellationToken token = default)
    {
        while (_queue.Count > 0 || schedulers.Any(s => s.Count > 0))
        {
            if (_queue.Count == 0)
                Steal(schedulers);

            if (_queue.Count == 0)
                continue;

            var toRunTask = _queue.Dequeue();
            try
            {
                var task = toRunTask.task(token);
                if (!task.IsCompleted)
                    _queue.Enqueue(toRunTask, toRunTask.priority);
            }
            catch (Exception ex)
            {
                OnTaskFailed?.Invoke(ex);
                continue;
            }
        }
    }

    public bool TrySteal(out (Func<CancellationToken, Task> task, int priority) item)
    {
        return _queue.TryDequeue(out item, out _);
    }

    public void Steal(List<Scheduler> schedulers)
    {
        var longestQueue = schedulers.OrderByDescending(s => s.Count).First();
        if (longestQueue.TrySteal(out var stolen))
            _queue.Enqueue(stolen, stolen.priority);
    }
}