namespace Naples.Tests;

using Naples.Core;
using Xunit;

public class SchedulerTests
{
    [Fact]
    public void RunTest()
    {
        bool ran = false;
        var scheduler = new Scheduler();
        scheduler.Enqueue((token) => { ran = true; return Task.CompletedTask; });
        scheduler.Run();
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
        });

        scheduler.Run();
        Assert.Equal(3, count);
    }

    [Fact]
    public void OnTaskFailedTest()
    {
        bool exceptionCaught = false;

        var scheduler = new Scheduler();

        scheduler.OnTaskFailed += ex => exceptionCaught = true;
        scheduler.Enqueue((token) => throw new Exception("Errore."));
        scheduler.Run();
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
        });
        cts.Cancel();
        scheduler.Run(cts.Token);
        Assert.True(canceled);
    }
}