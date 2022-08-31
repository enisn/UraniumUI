namespace UraniumUI.Material.Views;
public partial class BottomSheetView
{
    public bool DisablePageWhenOpened { get => (bool)GetValue(DisablePageWhenOpeneProperty); set => SetValue(DisablePageWhenOpeneProperty, value); }

    public static readonly BindableProperty DisablePageWhenOpeneProperty =
        BindableProperty.Create(
            nameof(DisablePageWhenOpened),
            typeof(bool), typeof(BottomSheetView), defaultValue: true);
}