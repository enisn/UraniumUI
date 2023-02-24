using Shouldly;
using UraniumUI.Material.Attachments;
using UraniumUI.Tests.Core;

namespace UraniumUI.Material.Tests.Attachments;
public class BackdropView_Test
{
    public BackdropView_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
    }

    [Fact]
    public void Title_BindingForInitialization_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new BackdropView());
        var viewModel = new { Title = "My Title" };
        control.BindingContext = viewModel;
        control.SetBinding(BackdropView.TitleProperty, new Binding(nameof(viewModel.Title)));

        // Assert
        control.Title.ShouldBe(viewModel.Title);
    }

    [Fact]
    public void Title_ShouldBeChanged_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new BackdropView());
        var viewModel = new BackdropTestViewModel { Title = "My Title" };
        control.BindingContext = viewModel;
        control.SetBinding(BackdropView.TitleProperty, new Binding(nameof(viewModel.Title)));

        // Act
        viewModel.Title = "Title (changed)";

        // Assert
        control.Title.ShouldBe(viewModel.Title);
    }

    internal class BackdropTestViewModel : UraniumBindableObject
    {
        private string title;

        public string Title { get => title; set => SetProperty(ref title, value); }
    }
}
