using Android.Content;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Text;
using Android.Util;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Graphics.Drawable;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using static Android.Views.View;

namespace UraniumUI.Platforms.Android;
public class UraniumAndroidDatePicker : AppCompatEditText, IOnClickListener
{
    public UraniumAndroidDatePicker(Context context) : base(context)
    {
        Initialize();
    }

    public UraniumAndroidDatePicker(Context context, IAttributeSet attrs) : base(context, attrs)
    {
        Initialize();
    }

    public UraniumAndroidDatePicker(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
    {
        Initialize();
    }

    protected UraniumAndroidDatePicker(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
        Initialize();
    }
    public Action? ShowPicker { get; set; }
    public Action? HidePicker { get; set; }

    public void OnClick(global::Android.Views.View v)
    {
        ShowPicker?.Invoke();
    }

    void Initialize()
    {
        using (var gradientDrawable = new GradientDrawable())
        {
            gradientDrawable.SetColor(global::Android.Graphics.Color.Transparent);
            this.SetBackground(gradientDrawable);
            BackgroundTintList = global::Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToPlatform());
        }

        if (Background != null)
        {
            DrawableCompat.Wrap(Background);
        }

        Focusable = true;
        FocusableInTouchMode = false;
        Clickable = true;
        InputType = InputTypes.Null;

        SetOnClickListener(this);
    }
}
