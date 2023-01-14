using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using UraniumUI.Pages;

namespace UraniumUI.Material.Attachments;
public partial class BackdropView : ContentView, IPageAttachment
{
    public UraniumContentPage AttachedPage { get; protected set; }
    public AttachmentPosition AttachmentPosition => AttachmentPosition.Behind;

    protected ToolbarItem toolbarItem = new ToolbarItem();

    public BackdropView()
    {
        this.VerticalOptions = LayoutOptions.Fill;

        this.Padding = new Thickness(20, 0, 20, 30);
    }

    public virtual void OnAttached(UraniumContentPage attachedPage)
    {
        AttachedPage = attachedPage;
        if (Shell.Current?.BackgroundColor != null)
        {
            this.BackgroundColor = Shell.Current.BackgroundColor;
        }
        if (Shell.Current?.Background != null)
        {
            this.Background = Shell.Current.Background;
        }

        this.Content.VerticalOptions = LayoutOptions.Start;

        toolbarItem.SetBinding(ToolbarItem.IconImageSourceProperty, new Binding(nameof(IconImageSource), source: this));
        toolbarItem.SetBinding(ToolbarItem.TextProperty, new Binding(nameof(Title), source: this));
        toolbarItem.Clicked += (s, e) => IsPresented = !IsPresented;

        if (InsertAfterToolbarIcons)
        {
            AttachedPage.ToolbarItems.Add(toolbarItem);
        }
        else
        {
            AttachedPage.ToolbarItems.Insert(0, toolbarItem);
        }
    }

    protected virtual async void SlideToState(bool isPresented)
    {
        if (!isPresented)
        {
            await AttachedPage.ContentFrame.TranslateTo(0, isPresented ? this.Content.Height : 0);
        }

        foreach (BackdropView backdrop in AttachedPage.Attachments.Where(x => x is BackdropView))
        {
            backdrop.IsVisible = isPresented && backdrop == this;
        }

        if (isPresented)
        {
            await AttachedPage.ContentFrame.TranslateTo(0, isPresented ? this.Content.Height : 0);
        }
    }
}
