using Microsoft.Maui.Controls;

namespace UraniumUI.Pages.Views;

[ContentProperty(nameof(Body))]
public class BottomSheetView : Frame
{
    public Page Page { get; }

    public View Body { get => body; set { body = value; Init(); } }

    private bool isPresented;
    public bool IsPresented { get => isPresented; set { isPresented = value; AlignBottomSheet(); } }

    PanGestureRecognizer panGestureRecognizer = new PanGestureRecognizer();
    private View body;

    public BottomSheetView(View content, Page page)
    {
        Page = page;
    }

    protected virtual void Init()
    {
        Padding = 0;
        this.CornerRadius = 20;
        this.VerticalOptions = LayoutOptions.End;
        this.HorizontalOptions = LayoutOptions.Fill;
        this.Content = new VerticalStackLayout()
        {
            Children =
            {
                GenerateAnchor(),
                Body
            }
        };

        panGestureRecognizer.PanUpdated += PanGestureRecognizer_PanUpdated;
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
                HeightRequest = 4,
                CornerRadius = 2,
                WidthRequest = 50,
                BackgroundColor = Colors.Gray,
                HorizontalOptions = LayoutOptions.Center,
                InputTransparent = true,
            }
        };

        anchor.GestureRecognizers.Add(panGestureRecognizer);

        Page.SizeChanged += (s, e) => { AlignBottomSheet(); };
        return anchor;
    }

    private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Running:
                //this.TranslateTo(this.X, this.TranslationY + e.TotalY, 50); // TODO: ax value
                this.TranslationY = this.TranslationY + e.TotalY;
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
        double y = this.Height - 25;
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
    }
}
