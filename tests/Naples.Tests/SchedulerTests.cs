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
        scheduler.Enqueue(() => { ran = true; return Task.CompletedTask; });
        scheduler.Run();
        Assert.True(ran);
    }

    [Fact]
    public void RequeueTests()
    {
        int count = 0;
        var scheduler = new Scheduler();

        scheduler.Enqueue(() =>
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
}