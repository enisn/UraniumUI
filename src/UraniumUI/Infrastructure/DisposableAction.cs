namespace UraniumUI.Infrastructure;
public class DisposableAction : IDisposable
{
    private Action _action;

    public DisposableAction(Action action)
    {
        _action = action;
    }

    public void Dispose()
    {
        _action?.Invoke();
        _action = null;
    }
}
