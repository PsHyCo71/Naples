namespace Naples.Tests;

using Naples;
using Xunit;

public class SchedulerTests
{
    [Fact]
    public void RunTest()
    {
        bool ran = false;
        var scheduler = new Scheduler();
        scheduler.Enqueue((token) => { ran = true; return Task.CompletedTask; }, 0);
        scheduler.Run(new List<Scheduler>(), CancellationToken.None);
        Assert.True(ran);
    }

    [Fact]
    public void RequeueTests()
    {
        int count = 0;
        var scheduler = new Scheduler();

        scheduler.Enqueue((token) =>
        {
            count++;
            if (count < 3)
            {
                var tcs = new TaskCompletionSource();
                return tcs.Task; // task che non finisce mai
            }
            return Task.CompletedTask;
        }, 0);

        scheduler.Run(new List<Scheduler>(), CancellationToken.None);
        Assert.Equal(3, count);
    }

    [Fact]
    public void OnTaskFailedTest()
    {
        bool exceptionCaught = false;

        var scheduler = new Scheduler();

        scheduler.OnTaskFailed += ex => exceptionCaught = true;
        scheduler.Enqueue((token) => throw new Exception("Errore."), 0);
        scheduler.Run(new List<Scheduler>(), CancellationToken.None);
        Assert.True(exceptionCaught);
    }

    [Fact]
    public void CancellationTest()
    {
        var cts = new CancellationTokenSource();
        bool canceled = false;
        var scheduler = new Scheduler();
        scheduler.Enqueue((token) =>
        {
            if (token.IsCancellationRequested)
            {
                canceled = true;
            }
            return Task.CompletedTask;
        }, 0);
        cts.Cancel();
        scheduler.Run(new List<Scheduler>(), cts.Token);
        Assert.True(canceled);
    }

    [Fact]
    public void PriorityTest()
    {
        var scheduler = new Scheduler();
        List<string> order = new List<string>();

        scheduler.Enqueue(token => { order.Add("A"); return Task.CompletedTask; }, 3);
        scheduler.Enqueue(token => { order.Add("B"); return Task.CompletedTask; }, 1);
        scheduler.Enqueue(token => { order.Add("C"); return Task.CompletedTask; }, 2);
        scheduler.Run(new List<Scheduler>(), CancellationToken.None);
        
        Assert.Equal(new[] {"B", "C", "A"}, order);
    }
}