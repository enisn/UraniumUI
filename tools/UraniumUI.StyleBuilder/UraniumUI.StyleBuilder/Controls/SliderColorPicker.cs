using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.StyleBuilder.Controls
{
    public class SliderColorPicker : VerticalStackLayout
    {
        private Slider sliderRed = new Slider
        {
            ThumbColor = Colors.Red,
            MinimumTrackColor = Colors.Red,
        };
        private Slider sliderGreen = new Slider
        {
            ThumbColor = Colors.Green,
            MinimumTrackColor = Colors.Green,
        };
        private Slider sliderBlue = new Slider
        {
            ThumbColor = Colors.Blue,
            MinimumTrackColor = Colors.Blue,
        };

        private bool initialized;
        public SliderColorPicker()
        {
            Children.Add(sliderRed);
            Children.Add(sliderGreen);
            Children.Add(sliderBlue);

            sliderGreen.ValueChanged += Slider_ValueChanged;
            sliderRed.ValueChanged += Slider_ValueChanged;
            sliderBlue.ValueChanged += Slider_ValueChanged;
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            UpdateFromSliders();
        }

        protected void UpdateFromColor()
        {
            initialized = false;

            if ((float)sliderRed.Value != Color.Red)
                sliderRed.Value = Color.Red;
            if ((float)sliderGreen.Value != Color.Green)
                sliderGreen.Value = Color.Green;
            if ((float)sliderBlue.Value != Color.Blue)
                sliderBlue.Value = Color.Blue;

            initialized = true;
        }

        protected void UpdateFromSliders()
        {
            if (!initialized)
            {
                return;
            }

            var newColor = new Color((float)sliderRed.Value, (float)sliderGreen.Value, (float)sliderBlue.Value);
            if (newColor.Red != Color.Red || newColor.Blue != Color.Blue || newColor.Green != Color.Green)
            {
                Color = newColor;
            }
        }

        public Color Color { get => (Color)GetValue(ColorProperty); set => SetValue(ColorProperty, value); }

        public static readonly BindableProperty ColorProperty = BindableProperty.Create(
            nameof(Color),
            typeof(Color),
            typeof(SliderColorPicker),
            defaultValue: Colors.Transparent,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is SliderColorPicker colorEditor && newValue is Color newColor)
                {
                    colorEditor.UpdateFromColor();
                }
            });
    }
}
