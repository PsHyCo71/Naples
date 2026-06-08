namespace Naples;

public static class Timer
{
    /// <summary>
    /// Delays a task.
    /// </summary>
    /// <param name="millisec">The delay duration in milliseconds.</param>
    /// <param name="token">Token used to cancel all running tasks.</param>
    /// <returns>Returns the delayed task.</returns>
    public static Task Delay(int millisec, CancellationToken token = default)
    {
        return Task.Delay(millisec, token);
    }
}