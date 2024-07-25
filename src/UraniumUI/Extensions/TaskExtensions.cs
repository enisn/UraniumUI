namespace UraniumUI.Extensions;
public static class TaskExtensions
{
    public static async void FireAndForget(this Task task)
    {
        await task.ConfigureAwait(false);
    }
}
