namespace Naples.Core;

public class Runtime
{
    private List<Scheduler> _schedulers = new List<Scheduler>();

    private int _nextSheduler = 0;

    public Runtime(int threads = 1)
    {
        for (int i = 0; i < threads; i++)
            _schedulers.Add(new Scheduler());
    }

    public event Action<Exception>? OnTaskFailed
    {
        add => _schedulers.ForEach(s => s.OnTaskFailed += value);
        remove => _schedulers.ForEach(s => s.OnTaskFailed -= value);
    }

    public void Spawn(Func<CancellationToken, Task> task, int priority)
    {
        _schedulers[_nextSheduler].Enqueue(task, priority);
        _nextSheduler = (_nextSheduler + 1) % _schedulers.Count;
    }

    public void Run(CancellationToken token = default)
    {
        List<Thread> threads = new List<Thread>();
        foreach (var scheduler in _schedulers)
        {
            var thread = new Thread(() => scheduler.Run(_schedulers, token));
            threads.Add(thread);
        }
        foreach(var thread in threads)
        {
            thread.Start();
        }
        foreach(var thread in threads)
        {
            thread.Join();
        }
    }
}