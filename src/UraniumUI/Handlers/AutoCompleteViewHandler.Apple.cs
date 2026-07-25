#if IOS || MACCATALYST
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UIKit;
using UraniumUI.Controls;
using UraniumUI.Extensions;

namespace UraniumUI.Handlers;
public partial class AutoCompleteViewHandler : ViewHandler<IAutoCompleteView, UIAutoCompleteTextField>
{
    protected override UIAutoCompleteTextField CreatePlatformView()
    {
        var view = new UIAutoCompleteTextField
        {
            AutoCompleteViewSource = new AutoCompleteDefaultDataSource(),
            SortingAlgorithm = (d, b) => b.OrderBy(x => x.StartsWith(d, StringComparison.InvariantCultureIgnoreCase) ? 0 : 1).ToArray(),
        };
        view.Text = VirtualView.Text;
        view.TextColor = VirtualView.TextColor.ToPlatform();
        view.ReturnKeyType = UIReturnKeyType.Done;
        if (OperatingSystem.IsIOSVersionAtLeast(15))
        {
            view.FocusEffect = null;
        }

        return view;
    }

    public override void PlatformArrange(Rect rect)
    {
        base.PlatformArrange(rect);
        
        if (PlatformView.IsInitialized)
        {
            UpdateDropdownPosition();
        }
        else
        {
            InitializeDropdown(rect);
        }
    }

    protected override void ConnectHandler(UIAutoCompleteTextField platformView)
    {
        platformView.EditingChanged += PlatformView_TextChanged;
        platformView.EditingDidBegin += PlatformView_EditingDidBegin;
        platformView.EditingDidEndOnExit += PlatformView_EditingDidEndOnExit;
        platformView.EditingDidEnd += PlatformView_EditingDidEnd;
        platformView.AutoCompleteViewSource.Selected += AutoCompleteViewSourceOnSelected;
    }

    protected override void DisconnectHandler(UIAutoCompleteTextField platformView)
    {
        platformView.EditingChanged -= PlatformView_TextChanged;
        platformView.EditingDidBegin -= PlatformView_EditingDidBegin;
        platformView.EditingDidEndOnExit -= PlatformView_EditingDidEndOnExit;
        platformView.EditingDidEnd -= PlatformView_EditingDidEnd;
        platformView.AutoCompleteViewSource.Selected -= AutoCompleteViewSourceOnSelected;
        platformView.CleanupScrollViewObserver();
    }

    private void PlatformView_TextChanged(object sender, EventArgs e)
    {
        if (VirtualView.Text != PlatformView.Text)
        {
            VirtualView.Text = PlatformView.Text;
        }
    }

    private void PlatformView_EditingDidBegin(object sender, EventArgs e)
    {
        VirtualView.IsFocused = true;
    }

    private void PlatformView_EditingDidEndOnExit(object sender, EventArgs e)
    {
        VirtualView.Completed();
    }

    private void PlatformView_EditingDidEnd(object sender, EventArgs e)
    {
        VirtualView.IsFocused = false;
    }

    public static void MapText(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        if (handler.PlatformView.Text != view.Text)
        {
            handler.PlatformView.Text = view.Text;
        }
    }

    public static void MapTextColor(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        if (view.TextColor is null)
        {
            return;
        }

        handler.PlatformView.TextColor = view.TextColor.ToPlatform();
    }

    public static void MapItemsSource(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        handler.SetItemsSource();
    }

    public static void MapThreshold(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        handler.PlatformView.Threshold = view.Threshold;
    }

    public static void MapKeyboard(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        handler.PlatformView.ApplyKeyboard(view.Keyboard);
    }

    private void SetItemsSource()
    {
        if (VirtualView.ItemsSource != null)
        {
            var items = VirtualView.ItemsSource;
            PlatformView.UpdateItems(items);
        }
    }

    private void InitializeDropdown(CGRect rect)
    {
        var ctrl = GetTopViewController();
        if (ctrl == null) return;

        PlatformView.Render(ctrl, VirtualView as AutoCompleteView);
    }

    private void UpdateDropdownPosition()
    {
        PlatformView.UpdatePosition();
    }

    private UIViewController GetTopViewController()
    {
        // Use modern API instead of deprecated KeyWindow
        var windowScene = UIApplication.SharedApplication.ConnectedScenes
            .OfType<UIWindowScene>()
            .FirstOrDefault(scene => scene.ActivationState == UISceneActivationState.ForegroundActive);

        var window = windowScene?.Windows.FirstOrDefault(w => w.IsKeyWindow) 
                     ?? UIApplication.SharedApplication.KeyWindow; // Fallback for older iOS

        if (window?.RootViewController == null) return null;

        var viewController = window.RootViewController;
        while (viewController.PresentedViewController != null)
            viewController = viewController.PresentedViewController;

        return viewController;
    }

    private void AutoCompleteViewSourceOnSelected(object sender, SelectedItemChangedEventArgs args)
    {
        var selectedItemText = args.SelectedItem?.ToString();
        
        if (VirtualView.SelectedText != selectedItemText)
        {
            VirtualView.SelectedText = selectedItemText;
        }
    }
}

public class UIAutoCompleteTextField : MauiTextField, IUITextFieldDelegate
{
    private AutoCompleteViewSource _autoCompleteViewSource;
    private UIView _background;
    private IList _items = new List<string>();
    private UIViewController _parentViewController;
    private UIScrollView _parentScrollView;
    private IDisposable _scrollObserver;

    public Func<string, IEnumerable<string>, IList<string>> SortingAlgorithm { get; set; } = (t, d) => d.OrderBy(x => x.Contains(t, StringComparison.InvariantCultureIgnoreCase) ? 0 : 1).ToArray();

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

    public void Render(UIViewController viewController, AutoCompleteView virtualView)
    {
        if (IsInitialized)
            return;

        _parentViewController = viewController ?? throw new ArgumentNullException(nameof(viewController), "View cannot be null");
        _parentScrollView = GetParentScrollView(this);

        // Make new tableview
        AutoCompleteTableView = new AutoCompleteTableView(_parentScrollView)
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

        AutoCompleteTableView.Layer.CornerRadius = 5;

        _background = new UIView { BackgroundColor = UIColor.White, Hidden = true };
        _background.Layer.CornerRadius = 5;
        _background.Layer.MasksToBounds = false;
        _background.Layer.ShadowColor = UIColor.Black.CGColor;
        _background.Layer.ShadowOffset = new CGSize(0.0f, 4.0f);
        _background.Layer.ShadowOpacity = 0.25f;
        _background.Layer.ShadowRadius = 8f;
        _background.Layer.BorderColor = UIColor.LightGray.CGColor;
        _background.Layer.BorderWidth = 0.1f;

        // Add to view hierarchy
        var parentView = _parentScrollView ?? _parentViewController.View;
        parentView.AddSubview(_background);
        parentView.AddSubview(AutoCompleteTableView);

        // Textfield settings
        AutocorrectionType = UITextAutocorrectionType.No;
        ClearButtonMode = UITextFieldViewMode.Never;

        // Subscribe to events only once
        EditingChanged += OnEditingChanged;
        EditingDidEnd += OnEditingDidEnd;
        EditingDidBegin += UIAutoCompleteTextField_EditingDidBegin;

        // Observe scroll events if in scrollview
        if (_parentScrollView != null)
        {
            _scrollObserver = _parentScrollView.AddObserver("contentOffset", Foundation.NSKeyValueObservingOptions.New, _ =>
            {
                if (!AutoCompleteTableView.Hidden)
                {
                    UpdatePosition();
                }
            });
        }

        UpdateTableViewData();
        IsInitialized = true;
    }

    public void UpdatePosition()
    {
        if (!IsInitialized || AutoCompleteTableView == null)
            return;

        var frame = CalculateDropdownFrame();
        AutoCompleteTableView.Frame = frame;
        _background.Frame = frame;
    }

    private CGRect CalculateDropdownFrame()
    {
        CGRect frame;
        
        if (_parentScrollView == null)
        {
            // Not in a scrollview - position relative to view controller's view
            var windowFrame = this.ConvertRectToView(this.Bounds, _parentViewController.View);
            frame = new CGRect(
                windowFrame.X,
                windowFrame.Y + windowFrame.Height,
                windowFrame.Width,
                AutocompleteTableViewHeight
            );
        }
        else
        {
            // Inside a scrollview - position relative to scrollview coordinates
            var scrollFrame = this.ConvertRectToView(this.Bounds, _parentScrollView);
            
            frame = new CGRect(
                scrollFrame.X,
                scrollFrame.Y + scrollFrame.Height,
                scrollFrame.Width,
                AutocompleteTableViewHeight
            );
        }

        return frame;
    }

    private void UIAutoCompleteTextField_EditingDidBegin(object sender, EventArgs e)
    {
        HandleTableState();
    }

    private void OnEditingDidEnd(object sender, EventArgs eventArgs)
    {
        HideAutoCompleteView();
    }

    private void OnEditingChanged(object sender, EventArgs eventArgs)
    {
        HandleTableState();
    }

    private void HandleTableState()
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
        UpdatePosition(); // Update position before showing
        _background.Hidden = false;
        AutoCompleteTableView.Hidden = false;
        
        if (_parentScrollView != null)
        {
            _parentScrollView.ScrollRectToVisible(AutoCompleteTableView.Frame, true);
        }
    }

    private void HideAutoCompleteView()
    {
        _background.Hidden = true;
        AutoCompleteTableView.Hidden = true;
    }

    public void UpdateTableViewData()
    {
        if (_items is IEnumerable<object> _itemAsObject)
        {
            var sorted = SortingAlgorithm(Text, _itemAsObject.Select(x => x.ToString()));
            if (!sorted.Any())
            {
                HideAutoCompleteView();
                return;
            }
            AutoCompleteViewSource.Suggestions = (IList) sorted;
        }
        else
        {
            AutoCompleteViewSource.Suggestions = _items;
        }

        AutoCompleteTableView.ReloadData();

        // Adjust height based on content
        var height = Math.Min(AutocompleteTableViewHeight, (int)AutoCompleteTableView.ContentSize.Height);
        var currentFrame = AutoCompleteTableView.Frame;
        var newFrame = new CGRect(currentFrame.X, currentFrame.Y, currentFrame.Width, height);
        AutoCompleteTableView.Frame = newFrame;
        _background.Frame = newFrame;
    }

    public void UpdateItems(IList items)
    {
        _items = items;
        AutoCompleteViewSource.UpdateSuggestions(items);
    }

    public void CleanupScrollViewObserver()
    {
        _scrollObserver?.Dispose();
        _scrollObserver = null;
    }

    private static UIScrollView GetParentScrollView(UIView element)
    {
        if (element.Superview == null) return null;
        var scrollView = element.Superview as UIScrollView;
        return scrollView ?? GetParentScrollView(element.Superview);
    }
}

public abstract class AutoCompleteViewSource : UITableViewSource
{
    public IList Suggestions { get; set; } = new List<string>();

    public UIAutoCompleteTextField AutoCompleteTextField { get; set; }

    public abstract void UpdateSuggestions(IList suggestions);

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
            AutoCompleteTextField.Text = Suggestions[indexPath.Row]?.ToString();
        AutoCompleteTextField.ResignFirstResponder();
        var item = Suggestions[(int)indexPath.Item];
        Selected?.Invoke(tableView, new SelectedItemChangedEventArgs(item, -1));
    }
}

public class AutoCompleteDefaultDataSource : AutoCompleteViewSource
{
    private const string _cellIdentifier = "DefaultIdentifier";

    public override void UpdateSuggestions(IList suggestions)
    {
        Suggestions = suggestions;
    }


    public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {
        var cell = tableView.DequeueReusableCell(_cellIdentifier);
        var item = Suggestions[indexPath.Row];

        if (cell == null)
            cell = new UITableViewCell(UITableViewCellStyle.Default, _cellIdentifier);

        cell.BackgroundColor = UIColor.Clear;
        cell.TextLabel.Text = item.ToString();

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
