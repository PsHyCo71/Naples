namespace Naples;

public class Waker
{
    // A callback action.
    private Action _callback;
    
    // Determines if the callback is called or not.
    private bool _called;
    
    /// <summary>
    /// Waker class constructor, makes a received action the callback.
    /// </summary>
    /// <param name="action">User given action.</param>
    public Waker(Action action)
    {
        _callback = action;
    }

    /// <summary>
    /// Calls the callback.
    /// </summary>
    /// <remarks>
    /// Calls the callback only if it has not already been called.
    /// </remarks>
    public void Wake()
    {
        if (!_called)
        {
            _called = true;
            _callback();
        }
    }
}