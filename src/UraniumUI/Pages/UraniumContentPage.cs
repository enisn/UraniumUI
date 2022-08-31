using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace UraniumUI.Pages;

[ContentProperty(nameof(PageBody))]
public class UraniumContentPage : ContentPage
{
    protected Grid _contentGrid;

    public View PageBody { get => MainContent.Content; set => MainContent.Content = value; }

    public ObservableCollection<IPageAttachment> Attachments { get; set; } = new();

    protected ContentView MainContent { get; } = new ContentView();

    public UraniumContentPage()
    {
        Content = _contentGrid = new Grid
        {
            Children =
            {
                MainContent
            }
        };
        Attachments.CollectionChanged += Attachments_CollectionChanged;
    }

    private void Attachments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (IPageAttachment attachment in e.NewItems)
            {
                _contentGrid.Add(attachment, 0, 0);

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
