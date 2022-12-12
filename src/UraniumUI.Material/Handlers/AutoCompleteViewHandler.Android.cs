#if ANDROID

using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.TextField;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Material.Controls;

namespace UraniumUI.Material.Handlers;
public partial class AutoCompleteViewHandler : ViewHandler<AutoCompleteView, TextInputLayout>
{
    public AutoCompleteView TypedView => VirtualView as AutoCompleteView;
    
    private AppCompatAutoCompleteTextView NativeControl =>  PlatformView?.EditText as AppCompatAutoCompleteTextView;

    protected override TextInputLayout CreatePlatformView()
    {
        var textInputLayout = new TextInputLayout(Context)
        {
            MinWidth = 1000
        };
        var autoComplete = new AppCompatAutoCompleteTextView(Context)
        {
            BackgroundTintList = ColorStateList.ValueOf(VirtualView.PlaceholderColor.ToAndroid()),
            Text = TypedView?.Text,
            Hint = TypedView?.Placeholder,
            
        };
        
        
        GradientDrawable gd = new GradientDrawable();
        gd.SetColor(global::Android.Graphics.Color.Transparent);
        autoComplete.SetBackground(gd);
        if (TypedView != null)
        {
            autoComplete.SetHintTextColor(TypedView.PlaceholderColor.ToAndroid());
            autoComplete.SetTextColor(TypedView.TextColor.ToAndroid());
        }
        textInputLayout.AddView(autoComplete);
        return textInputLayout;
    }

    private void SetItemsSource()
    {
        if (TypedView.ItemsSource == null) return;

        ResetAdapter();
    }

    private void ResetAdapter()
    {
        var adapter = new BoxArrayAdapter(Context,
            Android.Resource.Layout.SimpleDropDownItem1Line,
            TypedView.ItemsSource.ToList());
        
        NativeControl.Adapter = adapter;

        adapter.NotifyDataSetChanged();
    }

    public static void MapText(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        handler.NativeControl.Text = view.Text;
    }

    public static void MapItemsSource(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        handler.SetItemsSource();
    }
}

internal class BoxArrayAdapter : ArrayAdapter
{
    private readonly IList<string> _objects;

    public BoxArrayAdapter(
        Context context,
        int textViewResourceId,
        List<string> objects) : base(context, textViewResourceId, objects)
    {
        _objects = objects;
    }
}

#endif