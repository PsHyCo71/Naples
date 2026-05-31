namespace Naples.Sync;

public class Waker
{
    private Action _callback;
    private bool _called;
    
    public Waker(Action action)
    {
        _callback = action;
    }

    public void Wake()
    {
        if (!_called)
        {
            _called = true;
            _callback();
        }
    }
}