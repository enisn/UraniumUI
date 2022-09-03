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
        if (Shell.Current?.BackgroundColor != null)
        {
            this.BackgroundColor = Shell.Current.BackgroundColor;
        }
        else
        {
            this.SetDynamicResource(BackgroundColorProperty, "Primary");
        }
    }

    public virtual void OnAttached(UraniumContentPage attachedPage)
    {
        AttachedPage = attachedPage;
        this.Content.VerticalOptions = LayoutOptions.Start;

        toolbarItem.SetBinding(ToolbarItem.IconImageSourceProperty, new Binding(nameof(IconImageSource), source: this));
        toolbarItem.SetBinding(ToolbarItem.TextProperty, new Binding(nameof(Title), source: this));
        toolbarItem.Clicked += (s,e)=> IsPresented = !IsPresented;

        AttachedPage.ToolbarItems.Add(toolbarItem);
    }

    protected virtual void SlideToState(bool isPresented)
    {
        AttachedPage.ContentFrame.TranslateTo(0, isPresented ? this.Content.Height : 0);
    }
}
