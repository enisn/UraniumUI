namespace UraniumUI.Material.Attachments;
public partial class BottomSheetView
{
    public bool IsPresented { get => (bool)GetValue(IsPresentedProperty); set => SetValue(IsPresentedProperty, value); }

    public static readonly BindableProperty IsPresentedProperty =
        BindableProperty.Create(nameof(IsPresented), typeof(bool), typeof(BottomSheetView), defaultValue: false, defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bo, ov, nv) => (bo as BottomSheetView).AlignBottomSheet());

    public bool DisablePageWhenOpened { get => (bool)GetValue(DisablePageWhenOpeneProperty); set => SetValue(DisablePageWhenOpeneProperty, value); }

    public static readonly BindableProperty DisablePageWhenOpeneProperty =
        BindableProperty.Create(
            nameof(DisablePageWhenOpened),
            typeof(bool), typeof(BottomSheetView), defaultValue: true);

    public bool CloseOnTapOutside { get => (bool)GetValue(CloseOnTapOutsideProperty); set => SetValue(CloseOnTapOutsideProperty, value); }

    public static readonly BindableProperty CloseOnTapOutsideProperty =
        BindableProperty.Create(
            nameof(CloseOnTapOutside),
            typeof(bool), typeof(BottomSheetView), defaultValue: true);
}