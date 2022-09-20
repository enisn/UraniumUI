using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Pages;
using UraniumUI.Resources;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;
public class TreeViewNodeHolderView : VerticalStackLayout
{
    public View NodeView { get => nodeContainer.Content; set => nodeContainer.Content = value; }

    protected ContentView nodeContainer = new ContentView
    {
        HorizontalOptions = LayoutOptions.Fill,
    };

    protected ContentView iconArrow = new ContentView
    {
        VerticalOptions = LayoutOptions.Center,
        HorizontalOptions = LayoutOptions.Start,
        Padding = 15,
        Content = new Path
        {
            Data = UraniumShapes.ArrowLeft,
            Fill = ColorResource.GetColor("OnBackground", "OnBackgroundDark", Colors.DarkGray),
        }
    };

    protected VerticalStackLayout stackLayoutChildren = new VerticalStackLayout
    {
        Margin = new Thickness(10, 0, 0, 0),
        IsVisible = false
    };

    public DataTemplate DataTemplate { get; private set; }

    public TreeViewNodeHolderView(DataTemplate dataTemplate)
    {
        DataTemplate = dataTemplate;

        //TODO: Experimental / Remove Later
        nodeContainer.Content = DataTemplate.CreateContent() as View;
        // Experimental

        this.Add(new HorizontalStackLayout
        {
            Children =
            {
                iconArrow,
                nodeContainer
            }
        });
        this.Add(stackLayoutChildren);

        this.SetBinding(TreeView.IsExpandedProperty, new Binding("IsExtended", BindingMode.TwoWay));

        BindableLayout.SetItemTemplate(stackLayoutChildren, new DataTemplate(() =>
        {
            return new TreeViewNodeHolderView(DataTemplate);
        }));

        stackLayoutChildren.SetBinding(BindableLayout.ItemsSourceProperty, new Binding("Children"));

        InitializeArrowButton();
    }

    private void InitializeArrowButton()
    {
        var tapGestureRecognizer = new TapGestureRecognizer();
        iconArrow.GestureRecognizers.Add(tapGestureRecognizer);
        tapGestureRecognizer.Tapped += (s, e) =>
        {
            TreeView.SetIsExpanded(this, !TreeView.GetIsExpanded(this));
        };
    }

    protected internal virtual void OnIsExpandedChanged()
    {
        if (TreeView.GetIsExpanded(this))
        {
            iconArrow.RotateTo(90);
            stackLayoutChildren.IsVisible = true;
        }
        else
        {
            iconArrow.RotateTo(0);
            stackLayoutChildren.IsVisible = false;
        }

        Button btn = new Button();

        OnPropertyChanged("IsExpanded");
        OnPropertyChanged("IsExtended");


    }
}
