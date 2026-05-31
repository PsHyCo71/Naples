namespace Naples.Core;

public class Scheduler
{
    private Queue<Func<Task>> _queue = new();

    public void Enqueue(Func<Task> task)
    {
        _queue.Enqueue(task);
    }

    public void Run()
    {
        while(_queue.Count > 0)
        {
            var toRunTask = _queue.Dequeue();
            var task = toRunTask();
            if (!task.IsCompleted)
            {
                _queue.Enqueue(toRunTask);
            }
        }
    }
}
