using System.Net.NetworkInformation;

namespace Naples.Core;

public class Runtime
{
    private Scheduler _scheduler = new();

    public event Action<Exception>? OnTaskFailed
    {
        add => _scheduler.OnTaskFailed += value;
        remove => _scheduler.OnTaskFailed -= value;
    }

    public void Spawn(Func<CancellationToken, Task> task, int priority)
    {
        _scheduler.Enqueue(task, priority);
    }

    public void Run(int priority, CancellationToken token = default)
    {
        _scheduler.Run(priority, token);
    }
}