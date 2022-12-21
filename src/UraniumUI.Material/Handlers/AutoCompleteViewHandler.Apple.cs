#if IOS || MACCATALYST
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using UraniumUI.Material.Controls;

namespace UraniumUI.Material.Handlers;
public partial class AutoCompleteViewHandler : ViewHandler<AutoCompleteView, UIAutoCompleteTextField>
{

    protected override UIAutoCompleteTextField CreatePlatformView()
    {
        var view = new UIAutoCompleteTextField
        {
            AutoCompleteViewSource = new AutoCompleteDefaultDataSource(),
            SortingAlgorithm = (d, b) => b,
        };

        //view.AttributedPlaceholder = new NSAttributedString(VirtualView.Placeholder, null, VirtualView.PlaceholderColor.ToUIColor());
        view.Text = VirtualView.Text;
        //view.TextColor = VirtualView.TextColor.ToUIColor();

        view.AutoCompleteViewSource.Selected += AutoCompleteViewSourceOnSelected;
        return view;
    }

    public override void PlatformArrange(Rect rect)
    {
        base.PlatformArrange(rect);
        Draw(rect);
    }

    public static void MapText(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        handler.PlatformView.Text = view.Text;
    }

    public static void MapItemsSource(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        handler.SetItemsSource();
    }

    private void SetItemsSource()
    {
        if (VirtualView.ItemsSource != null)
        {
            var items = VirtualView.ItemsSource.ToList();
            PlatformView.UpdateItems(items);   
        }
    }

    public void Draw(CGRect rect)
    {
        var scrollView = GetParentScrollView(PlatformView);
        var ctrl = UIApplication.SharedApplication.GetTopViewController();

        var relativePosition = UIApplication.SharedApplication.KeyWindow;
        var relativeFrame = PlatformView.Superview.ConvertRectToView(PlatformView.Frame, relativePosition);

        PlatformView.Draw(ctrl, PlatformView.Layer, scrollView, relativeFrame.Y);
    }

    private void AutoCompleteViewSourceOnSelected(object sender, SelectedItemChangedEventArgs args)
    {
        //VirtualView.OnItemSelectedInternal(Element, args);
    }


    private static UIScrollView GetParentScrollView(UIView element)
    {
        if (element.Superview == null) return null;
        var scrollView = element.Superview as UIScrollView;
        return scrollView ?? GetParentScrollView(element.Superview);
    }

}

public class UIAutoCompleteTextField : UITextField, IUITextFieldDelegate
{
    private AutoCompleteViewSource _autoCompleteViewSource;
    private UIView _background;
    private CGRect _drawnFrame;
    private List<string> _items;
    private UIViewController _parentViewController;
    private UIScrollView _scrollView;

    public Func<string, ICollection<string>, ICollection<string>> SortingAlgorithm { get; set; } = (t, d) => d;

    public AutoCompleteViewSource AutoCompleteViewSource
    {
        get { return _autoCompleteViewSource; }
        set
        {
            _autoCompleteViewSource = value;
            _autoCompleteViewSource.AutoCompleteTextField = this;
            if (AutoCompleteTableView != null)
            {
                AutoCompleteTableView.Source = AutoCompleteViewSource;
            }
        }
    }

    public UITableView AutoCompleteTableView { get; private set; }

    public bool IsInitialized { get; private set; }

    public int Threshold { get; set; } = 2;

    public int AutocompleteTableViewHeight { get; set; } = 150;

#if NET6_0
    public void Draw(UIViewController viewController, CALayer layer, UIScrollView scrollView, NFloat y)
#else
    public void Draw(UIViewController viewController, CALayer layer, UIScrollView scrollView, nfloat y)
#endif
    {
        _scrollView = scrollView;
        _drawnFrame = layer.Frame;
        _parentViewController = viewController ?? throw new ArgumentNullException(nameof(viewController), "View cannot be null");

        //Make new tableview and do some settings
        AutoCompleteTableView = new AutoCompleteTableView(_scrollView)
        {
            DelaysContentTouches = true,
            ClipsToBounds = true,
            ScrollEnabled = true,
            AllowsSelection = true,
            Bounces = false,
            Hidden = true,
            ContentInset = UIEdgeInsets.Zero,
            AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
            Source = AutoCompleteViewSource,
            TableFooterView = new UIView()
        };

        //Some textfield settings
        AutocorrectionType = UITextAutocorrectionType.No;
        ClearButtonMode = UITextFieldViewMode.Never;

        var scrollViewIsNull = _scrollView == null;

        CGRect frame;
        UIView view;
        if (scrollViewIsNull)
        {
            view = _parentViewController.View;
            frame = new CGRect(_drawnFrame.X, y + _drawnFrame.Height, _drawnFrame.Width, AutocompleteTableViewHeight);
        }
        else
        {
            var e = (ScrollView)((ScrollViewRenderer)_scrollView).Element;
            var p = e.Padding;
            var m = e.Margin;
            frame = new CGRect(_drawnFrame.X + p.Left + m.Left,
                y + _drawnFrame.Height,
                _drawnFrame.Width,
                AutocompleteTableViewHeight);
            view = _scrollView;
        }

        AutoCompleteTableView.Layer.CornerRadius = 5;

        _background = new UIView(frame) { BackgroundColor = UIColor.White, Hidden = true };
        _background.Layer.CornerRadius = 5; //rounded corners
        _background.Layer.MasksToBounds = false;
        _background.Layer.ShadowColor = UIColor.Black.CGColor;
        _background.Layer.ShadowOffset = new CGSize(0.0f, 4.0f);
        _background.Layer.ShadowOpacity = 0.25f;
        _background.Layer.ShadowRadius = 8f;
        _background.Layer.BorderColor = UIColor.LightGray.CGColor;
        _background.Layer.BorderWidth = 0.1f;

        AutoCompleteTableView.Frame = frame;
        view.AddSubview(_background);
        view.AddSubview(AutoCompleteTableView);

        //listen to edit events
        EditingChanged += OnEditingChanged;
        EditingDidEnd += OnEditingDidEnd;

        UpdateTableViewData();
        IsInitialized = true;
    }

    private void OnEditingDidEnd(object sender, EventArgs eventArgs)
    {
        HideAutoCompleteView();
    }

    private void OnEditingChanged(object sender, EventArgs eventArgs)
    {
        if (Text.Length >= Threshold)
        {
            ShowAutoCompleteView();
            UpdateTableViewData();
        }
        else
        {
            HideAutoCompleteView();
        }
    }

    private void ShowAutoCompleteView()
    {
        _background.Hidden = false;
        AutoCompleteTableView.Hidden = false;
        if (_scrollView != null)
        {
            _scrollView.ScrollRectToVisible(AutoCompleteTableView.Frame, true);
        }
    }

    private void HideAutoCompleteView()
    {
        _background.Hidden = true;
        AutoCompleteTableView.Hidden = true;
    }

    public void UpdateTableViewData()
    {
        var sorted = SortingAlgorithm(Text, _items);
        if (!sorted.Any())
        {
            HideAutoCompleteView();
            return;
        }
        AutoCompleteViewSource.Suggestions = sorted;
        AutoCompleteTableView.ReloadData();

        var f = AutoCompleteTableView.Frame;
        var height = Math.Min(AutocompleteTableViewHeight, (int)AutoCompleteTableView.ContentSize.Height);
        var frame = new CGRect(f.X, f.Y, f.Width, height);
        AutoCompleteTableView.Frame = frame;
        _background.Frame = frame;
    }

    public void UpdateItems(List<string> items)
    {
        _items = items;
        AutoCompleteViewSource.UpdateSuggestions(items);
    }
}

public abstract class AutoCompleteViewSource : UITableViewSource
{
    public ICollection<string> Suggestions { get; set; } = new List<string>();

    public UIAutoCompleteTextField AutoCompleteTextField { get; set; }

    public abstract void UpdateSuggestions(ICollection<string> suggestions);

    public abstract override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath);

    public override nint RowsInSection(UITableView tableview, nint section)
    {
        return Suggestions.Count;
    }

    public event EventHandler<SelectedItemChangedEventArgs> Selected;

    public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
    {
        AutoCompleteTextField.AutoCompleteTableView.Hidden = true;
        if (indexPath.Row < Suggestions.Count)
            AutoCompleteTextField.Text = Suggestions.ElementAt(indexPath.Row);
        AutoCompleteTextField.ResignFirstResponder();
        var item = Suggestions.ToList()[(int)indexPath.Item];
        Selected?.Invoke(tableView, new SelectedItemChangedEventArgs(item, -1));
        // don't call base.RowSelected
    }
}

public class AutoCompleteDefaultDataSource : AutoCompleteViewSource
{
    private const string _cellIdentifier = "DefaultIdentifier";

    public override void UpdateSuggestions(ICollection<string> suggestions)
    {
        Suggestions = suggestions;
    }

    public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {
        var cell = tableView.DequeueReusableCell(_cellIdentifier);
        var item = Suggestions.ElementAt(indexPath.Row);

        if (cell == null)
            cell = new UITableViewCell(UITableViewCellStyle.Default, _cellIdentifier);

        cell.BackgroundColor = UIColor.Clear;
        cell.TextLabel.Text = item;

        return cell;
    }
}

internal class AutoCompleteTableView : UITableView
{
    private readonly UIScrollView _parentScrollView;

    public AutoCompleteTableView(UIScrollView parentScrollView)
    {
        _parentScrollView = parentScrollView;
    }

    public override bool Hidden
    {
        get { return base.Hidden; }
        set
        {
            base.Hidden = value;
            if (_parentScrollView == null) return;
            _parentScrollView.DelaysContentTouches = !value;
        }
    }
}
#endif