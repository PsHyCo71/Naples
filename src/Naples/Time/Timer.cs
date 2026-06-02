namespace Naples.Time;

public static class Timer
{
    public static Task Delay(int millisec, CancellationToken token = default)
    {
        return Task.Delay(millisec, token);
    }
}