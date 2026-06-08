namespace Naples.Tests;

using System.Diagnostics;
using Naples;
using Xunit;

public class TimerTest
{
    [Fact]
    public async Task DelayTest()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        await Timer.Delay(1000);
        stopwatch.Stop();
        Assert.True(stopwatch.ElapsedMilliseconds >= 1000);

    }
}