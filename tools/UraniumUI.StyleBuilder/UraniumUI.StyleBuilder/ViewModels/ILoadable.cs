namespace UraniumUI.StyleBuilder.ViewModels;
public interface ILoadable
{
    Task LoadAsync(string path);
}
