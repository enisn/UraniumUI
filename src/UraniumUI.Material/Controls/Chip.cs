using Microsoft.Maui.Controls.Shapes;
using System.Windows.Input;
using UraniumUI.Extensions;
using UraniumUI.Pages;
using UraniumUI.Resources;
using UraniumUI.Views;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls
{
    public class Chip : Border
    {
        public event EventHandler DestroyClicked;
        protected Label label = new Label
        {
            VerticalOptions = LayoutOptions.Center,
        };
        protected StatefulContentView closeButton = new StatefulContentView
        {
            Content = new Path
            {
                Data = UraniumShapes.XCircle,
            },
            VerticalOptions = LayoutOptions.Center,
        };

        public Chip()
        {
            this.HorizontalOptions = LayoutOptions.Start;
            this.Padding = 5;
            this.StrokeShape = new RoundRectangle
            {
                CornerRadius = 20,
            };
            var defaultAccent = InputKit.Shared.InputKitOptions.GetAccentColor();
            this.SetAppThemeColor(
                BackgroundColorProperty,
                ColorResource.GetColor("Primary", defaultAccent),
                ColorResource.GetColor("PrimaryDark", defaultAccent));

            (closeButton.Content as Path).SetAppTheme(
                Path.FillProperty,
                Colors.White.WithAlpha(.5f).ToSolidColorBrush(),
                Colors.Black.WithAlpha(.5f).ToSolidColorBrush()
                );

            label.SetAppThemeColor(
                Label.TextColorProperty,
                ColorResource.GetColor("OnPrimary", Colors.White),
                ColorResource.GetColor("OnPrimaryDark", Colors.DarkGray));

            Content = new HorizontalStackLayout
            {
                Spacing = 5,
                Children =
                {
                    label,
                    closeButton
                }
            };

            closeButton.TappedCommand = new Command(() =>
            {
                DestroyCommand?.Execute(this);
                DestroyClicked?.Invoke(this, new EventArgs());
                if (SelfDestruct)
                {
                    if(this.Parent is Layout layout)
                    {
                        layout.Remove(this);
                    }
                    if (this.Parent is ContentView cv)
                    {
                        cv.Content = null;
                    }
                }
            });
        }

        public ICommand DestroyCommand { get => (ICommand)GetValue(DestroyCommandProperty); set => SetValue(DestroyCommandProperty, value); }

        public static readonly BindableProperty DestroyCommandProperty = BindableProperty.Create(
                nameof(DestroyCommand),
                typeof(ICommand),
                typeof(Chip));

        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(Chip),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is Chip chip)
                {
                    chip.label.Text = (string)newValue;
                }
            });

        public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
                nameof(TextColor),
                typeof(Color),
                typeof(Chip),
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    if (bindable is Chip chip)
                    {
                        chip.label.TextColor = (Color)newValue;
                    }
                });

        public bool SelfDestruct { get => (bool)GetValue(SelfDestructProperty); set => SetValue(SelfDestructProperty, value); }

        public static readonly BindableProperty SelfDestructProperty = BindableProperty.Create(
                nameof(SelfDestruct),
                typeof(bool),
                typeof(Chip),
                defaultValue: true);

        public bool IsDestroyVisible { get => (bool)GetValue(IsDestroyVisibleProperty); set => SetValue(IsDestroyVisibleProperty, value); }

        public static readonly BindableProperty IsDestroyVisibleProperty = BindableProperty.Create(
                nameof(IsDestroyVisible),
                typeof(bool),
                typeof(Chip),
                defaultValue: true,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    if (bindable is Chip chip)
                    {
                        chip.closeButton.IsVisible = (bool)newValue;
                    }
                });
    }
}
