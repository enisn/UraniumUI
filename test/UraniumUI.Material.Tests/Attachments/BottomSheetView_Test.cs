using Shouldly;
using UraniumUI.Material.Attachments;
using UraniumUI.Tests.Core;

namespace UraniumUI.Material.Tests.Attachments;
public class BottomSheetView_Test
{
    public BottomSheetView_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
    }

    [Fact]
    public void DisablePageWhenOpened_BindingForInitialization_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new BottomSheetView());
        var viewModel = new { DisablePageWhenOpened = false };
        control.BindingContext = viewModel;
        control.SetBinding(BottomSheetView.DisablePageWhenOpenedProperty, new Binding(nameof(viewModel.DisablePageWhenOpened)));

        // Assert
        control.DisablePageWhenOpened.ShouldBe(viewModel.DisablePageWhenOpened);
    }

    [Fact]
    public void DisablePageWhenOpened_ShouldBeChanged_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new BottomSheetView());
        var viewModel = new BottomSheetTestViewModel { DisablePageWhenOpened = false };
        control.BindingContext = viewModel;
        control.SetBinding(BottomSheetView.DisablePageWhenOpenedProperty, new Binding(nameof(viewModel.DisablePageWhenOpened)));

        // Act
        viewModel.DisablePageWhenOpened = true;

        // Assert
        control.DisablePageWhenOpened.ShouldBe(viewModel.DisablePageWhenOpened);
    }

    internal class BottomSheetTestViewModel : UraniumBindableObject
    {
        private bool disablePageWhenOpened;
        public bool DisablePageWhenOpened { get => disablePageWhenOpened; set => SetProperty(ref disablePageWhenOpened, value); }
    }
}
