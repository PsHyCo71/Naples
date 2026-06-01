namespace Naples.Core;

public class Runtime
{
    private Scheduler _scheduler = new();

    public void Spawn(Func<Task> task)
    {
        _scheduler.Enqueue(task);
    }

    public void Run()
    {
        _scheduler.Run();
    }
}