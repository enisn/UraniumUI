namespace UraniumUI.Material.Attachments;
public partial class BackdropView
{
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty TitleProperty =
    BindableProperty.Create(nameof(Title), typeof(string), typeof(BackdropView),
    propertyChanged: (bo, ov, nv) => (bo as BackdropView).OnPropertyChanged(nameof(Title)));

    public ImageSource IconImageSource
    {
        get => (ImageSource)GetValue(IconImageSourceProperty);
        set => SetValue(IconImageSourceProperty, value);
    }

    public static readonly BindableProperty IconImageSourceProperty =
        BindableProperty.Create(nameof(IconImageSource), typeof(ImageSource), typeof(BackdropView),
            propertyChanged: (bo, ov, nv) => (bo as BackdropView).OnPropertyChanged(nameof(IconImageSource)));

    public bool IsPresented { get => (bool)GetValue(IsPresentedProperty); set => SetValue(IsPresentedProperty, value); }

    public static readonly BindableProperty IsPresentedProperty =
        BindableProperty.Create(nameof(IsPresented), typeof(bool), typeof(BackdropView), defaultValue: false,
            propertyChanged: (bo, ov, nv) => (bo as BackdropView).SlideToState((bool)nv));

    public bool InsertAfterToolbarIcons { get => (bool)GetValue(InsertAfterToolbarIconsProperty); set => SetValue(InsertAfterToolbarIconsProperty, value); }

    public static readonly BindableProperty InsertAfterToolbarIconsProperty =
        BindableProperty.Create(nameof(InsertAfterToolbarIcons), typeof(bool), typeof(BackdropView), defaultValue: true);
}