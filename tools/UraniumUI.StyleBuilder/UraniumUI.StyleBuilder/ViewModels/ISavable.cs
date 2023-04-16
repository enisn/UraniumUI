namespace UraniumUI.StyleBuilder.ViewModels;
public interface ISavable : IDisposable
{
    Task SaveAsync();

    Task SaveAsAsync();

    string Title { get; }

    string Path { get; }
}
