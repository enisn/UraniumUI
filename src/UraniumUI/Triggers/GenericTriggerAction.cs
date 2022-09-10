namespace UraniumUI.Triggers;
public class GenericTriggerAction<T> : TriggerAction<T>
    where T : BindableObject
{
    private readonly Action<T> action;

    public GenericTriggerAction(Action<T> action)
    {
        this.action = action ?? throw new InvalidOperationException("An action must be defined to run the GenericTriggerAction");
    }
    
    protected override void Invoke(T sender)
    {
        action(sender);
    }
}
