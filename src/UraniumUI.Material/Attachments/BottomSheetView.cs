using InputKit.Shared.Helpers;
using UraniumUI.Extensions;
using UraniumUI.Pages;

namespace UraniumUI.Material.Attachments;

[ContentProperty(nameof(Body))]
public partial class BottomSheetView : Border, IPageAttachment
{
    public UraniumContentPage AttachedPage { get; protected set; }
    public AttachmentPosition AttachmentPosition => AttachmentPosition.Front;

    public View Body { get; set; }

    public View Header { get; set; }

    private TapGestureRecognizer closeGestureRecognizer = new();

    public void OnAttached(UraniumContentPage page)
    {
        Init();

        AttachedPage = page;
        if (Body != null)
        {
            Body.SizeChanged += (s, e) => AlignBottomSheet(false);
        }
    }

    protected virtual void Init()
    {
        Header ??= GenerateAnchor();
        Padding = 0;
        this.StyleClass = new[] { "BottomSheet" };
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

        if (DeviceInfo.Idiom != DeviceIdiom.Desktop)
        {
            var panGestureRecognizer = new PanGestureRecognizer();
            panGestureRecognizer.PanUpdated += PanGestureRecognizer_PanUpdated;
            Header.GestureRecognizers.Add(panGestureRecognizer);
        }

        var tapGestureRecognizer = new TapGestureRecognizer();
        tapGestureRecognizer.Tapped += (s, e) => IsPresented = !IsPresented;
        Header.GestureRecognizers.Add(tapGestureRecognizer);
        Header.BackgroundColor = this.BackgroundColor;

        closeGestureRecognizer.Tapped += (s, e) => IsPresented = false;
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
            }
        };

        return anchor;
    }

    protected virtual void OnOpened()
    {
        if (CloseOnTapOutside)
        {
            AttachedPage?.ContentFrame?.GestureRecognizers.Add(closeGestureRecognizer);
        }
    }

    protected virtual void OnClosed()
    {
        if (CloseOnTapOutside)
        {
            AttachedPage?.ContentFrame?.GestureRecognizers.Remove(closeGestureRecognizer);
        }
    }

    private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Running:
                var isApple = DeviceInfo.Current.Platform == DevicePlatform.iOS || DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst;

                var y = TranslationY + (isApple ? e.TotalY * .05 : e.TotalY);

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
                AlignBottomSheet();
                break;
        }
    }

    private void AlignBottomSheet(bool animate = true)
    {
        double y = this.Height - Header.Height;
        if (IsPresented)
        {
            y = 0;
            OnOpened();
        }
        else
        {
            OnClosed();
        }

        if (animate)
        {
            this.TranslateToSafely(this.X, y, 50);

        }
        else
        {
            this.TranslationY = y;
        }

        UpdateDisabledStateOfPage();
    }

    protected void UpdateDisabledStateOfPage()
    {
        if (AttachedPage?.Body != null && DisablePageWhenOpened)
        {
            AttachedPage.Body.InputTransparent = IsPresented;

            AttachedPage.Body.FadeToSafely(IsPresented ? .5 : 1);
        }
    }
}
