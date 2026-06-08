namespace Naples;

public class Runtime
{
    // List of schedulers, one per thread
    private List<Scheduler> _schedulers = new List<Scheduler>();

    // Round-robin index for load balancing
    private int _nextSheduler = 0;

    /// <summary>
    /// Initializes the runtime with the specified number of threads.
    /// </summary>
    /// <param name="threads">Number of threads to run tasks on.</param>
    /// <remarks>
    /// Defaults to 1 thread (single-threaded mode).
    /// </remarks>
    public Runtime(int threads = 1)
    {
        for (int i = 0; i < threads; i++)
            _schedulers.Add(new Scheduler());
    }

    /// <summary>
    /// Event triggered when a task throws an unhandled exception.
    /// </summary>
    public event Action<Exception>? OnTaskFailed
    {
        add => _schedulers.ForEach(s => s.OnTaskFailed += value);
        remove => _schedulers.ForEach(s => s.OnTaskFailed -= value);
    }

    /// <summary>
    /// Enqueues a task on the next available scheduler using round-robin load balancing.
    /// </summary>
    /// <param name="task">The task to enqueue.</param>
    /// <param name="priority">The priority of the task — lower values run first.</param>
    public void Spawn(Func<CancellationToken, Task> task, int priority)
    {
        _schedulers[_nextSheduler].Enqueue(task, priority);
        _nextSheduler = (_nextSheduler + 1) % _schedulers.Count;
    }

    /// <summary>
    /// Starts all scheduler threads and blocks until they all complete.
    /// </summary>
    /// <param name="token">Token used to cancel all running tasks.</param>
    public void Run(CancellationToken token = default)
    {
        List<Thread> threads = new List<Thread>();
        foreach (var scheduler in _schedulers)
        {
            var thread = new Thread(() => scheduler.Run(_schedulers, token));
            threads.Add(thread);
        }
        foreach (var thread in threads)
            thread.Start();
        foreach (var thread in threads)
            thread.Join();
    }
}