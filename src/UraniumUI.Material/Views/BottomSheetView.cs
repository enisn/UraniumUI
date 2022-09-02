using InputKit.Shared.Helpers;
using UraniumUI.Extensions;
using UraniumUI.Pages;

namespace UraniumUI.Material.Views;

[ContentProperty(nameof(Body))]
public partial class BottomSheetView : Frame, IPageAttachment
{
    public UraniumContentPage AttachedPage { get; protected set; }
    public View Body { get; set; }

    public View Header { get; set; }

    public void OnAttached(UraniumContentPage page)
    {
        Init();

        AttachedPage = page;
        page.SizeChanged += (s, e) => { AlignBottomSheet(false); };
    }

    protected virtual void Init()
    {
        Header ??= GenerateAnchor();
        Padding = 0;
        this.CornerRadius = 10;
        this.VerticalOptions = LayoutOptions.End;
        this.HorizontalOptions = LayoutOptions.Fill;
        this.Content = new VerticalStackLayout()
        {
            Children =
            {
                Header,
                Body
            }
        };

        var panGestureRecognizer = new PanGestureRecognizer();
        panGestureRecognizer.PanUpdated += PanGestureRecognizer_PanUpdated;
        Header.GestureRecognizers.Add(panGestureRecognizer);

        AlignBottomSheet(false);
    }

    protected virtual View GenerateAnchor()
    {
        var anchor = new ContentView
        {
            HorizontalOptions = LayoutOptions.Fill,
            Padding = 10,
            Content = new BoxView
            {
                HeightRequest = 2,
                CornerRadius = 2,
                WidthRequest = 50,
                Color = this.BackgroundColor?.ToSurfaceColor() ?? Colors.Gray,
                HorizontalOptions = LayoutOptions.Center,
                InputTransparent = true,
            }
        };

        return anchor;
    }

    private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Running:
                var isApple = DeviceInfo.Current.Platform == DevicePlatform.iOS || DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst;

                var y = TranslationY + (isApple ? e.TotalY *.05 : e.TotalY);

                this.TranslationY = y.Clamp(-50, this.Height);
                
                break;
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                if (this.TranslationY < this.Height * .5)
                {
                    IsPresented = true;
                }
                else
                {
                    IsPresented = false;
                }
                break;
        }
    }

    private void AlignBottomSheet(bool animate = true)
    {
        double y = this.Height - Header.Height;
        if (IsPresented)
        {
            y = 0;
        }

        if (animate)
        {
            this.TranslateTo(this.X, y, 50);

        }
        else
        {
            this.TranslationY = y;
        }

        UpdateDisabledStateOfPage();
    }

    protected void UpdateDisabledStateOfPage()
    {
        if (AttachedPage?.PageBody != null && DisablePageWhenOpened)
        {
            AttachedPage.PageBody.InputTransparent = IsPresented;

            AttachedPage.PageBody.FadeTo(IsPresented ? .5 : 1);
        }
    }
}
