using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace UraniumUI.Pages;

[ContentProperty(nameof(Body))]
public class UraniumContentPage : ContentPage
{
    protected Grid _contentGrid;

    public View Body { get => ContentFrame.Content; set => ContentFrame.Content = value; }

    public ObservableCollection<IPageAttachment> Attachments { get; set; } = new();

    public Frame ContentFrame { get; } = new Frame()
    { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, HasShadow = false };

    public UraniumContentPage()
    {
        ContentFrame.BackgroundColor = this.BackgroundColor;
        Content = _contentGrid = new Grid
        {
            Children =
            {
                ContentFrame
            }
        };
        Attachments.CollectionChanged += Attachments_CollectionChanged;

        ContentFrame.SetBinding(View.BackgroundColorProperty, new Binding(nameof(BackgroundColor), source: this));
    }

    private void Attachments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (IPageAttachment attachment in e.NewItems)
            {
                if (attachment.AttachmentPosition == AttachmentPosition.Front)
                {
                    _contentGrid.Add(attachment, 0, 0);
                }
                else
                {
                    _contentGrid.Insert(0, attachment);
                }

                attachment.OnAttached(this);
            }
        }

        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (IPageAttachment attachment in e.OldItems)
            {
                _contentGrid.Remove(attachment);
            }
        }
    }
}
