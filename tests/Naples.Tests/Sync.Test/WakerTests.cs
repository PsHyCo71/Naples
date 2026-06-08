namespace Naples.Tests;

using Naples;
using Xunit;

public class WakerTests
{
    [Fact]
    public void CallbackTest()
    {
        bool called = false;
        var waker = new Waker(() => called = true);
        waker.Wake();
        Assert.True(called);
    }

    [Fact]
    public void SecondRequestTest()
    {
        int count = 0;
        var waker = new Waker(() => count++);
        waker.Wake();
        waker.Wake();
        Assert.Equal(1, count);
    }
}
