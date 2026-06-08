namespace Naples;

public class Scheduler
{
    // Priority queue of tasks.
    private readonly PriorityQueue<(Func<CancellationToken, Task> task, int priority), int> _queue = new();

    /// <summary>
    /// Event triggered when a task throws an unhandled exception.
    /// </summary>
    public event Action<Exception>? OnTaskFailed;

    /// <summary>
    /// Number of tasks in a queue.
    /// </summary>
    public int Count => _queue.Count;

    /// <summary>
    /// Enqueues a task and the corresponding priority.
    /// </summary>
    /// <param name="task">The task to enqueue.</param>
    /// <param name="priority">The priority of the task — lower values run first.</param>
    public void Enqueue(Func<CancellationToken, Task> task, int priority)
    {
        _queue.Enqueue((task, priority), priority);
    }

    /// <summary>
    /// Runs all enqueued tasks in priority order. If the queue is empty,
    /// attempts to steal tasks from other schedulers. If a task throws
    /// an unhandled exception, <see cref="OnTaskFailed"/> is invoked and
    /// the task is skipped.
    /// </summary>
    /// <param name="schedulers">The list of all schedulers, used for work stealing.</param>
    /// <param name="token">Token used to cancel all running tasks.</param>
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
                {
                    Thread.Sleep(1);
                    _queue.Enqueue(toRunTask, toRunTask.priority);
                }
            }
            catch (Exception ex)
            {
                OnTaskFailed?.Invoke(ex);
                continue;
            }
        }
    }

    /// <summary>
    /// Tries to dequeue a task from this scheduler's queue, used by other schedulers for work stealing.
    /// </summary>
    /// <param name="item">Variables with all the information of the dequeued task.</param>
    /// <returns>True if a task was stolen, false if the queue was empty.</returns>
    public bool TrySteal(out (Func<CancellationToken, Task> task, int priority) item)
    {
        return _queue.TryDequeue(out item, out _);
    }

    /// <summary>
    /// Searches for the longest queue and uses TrySteal() on it.
    /// </summary>
    /// <param name="schedulers">The list of schedulers.</param>
    public void Steal(List<Scheduler> schedulers)
    {
        var longestQueue = schedulers.OrderByDescending(s => s.Count).First();
        if (longestQueue.TrySteal(out var stolen))
            _queue.Enqueue(stolen, stolen.priority);
    }
}