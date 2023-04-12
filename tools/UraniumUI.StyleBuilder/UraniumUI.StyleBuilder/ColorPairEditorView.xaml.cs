using System.Windows.Input;
using UraniumUI.StyleBuilder.Converters;

namespace UraniumUI.StyleBuilder;

public partial class ColorPairEditorView : ContentView
{
    public ColorPairEditorView()
    {
        InitializeComponent();
    }

    public string EditingColorBinding { get => (string)GetValue(EditingColorBindingProperty); set => SetValue(EditingColorBindingProperty, value); }

    public static readonly BindableProperty EditingColorBindingProperty = BindableProperty.Create(
        nameof(EditingColorBinding),
        typeof(string),
        typeof(ColorPairEditorView),
        defaultValue: string.Empty,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is ColorPairEditorView editor && newValue is string bindingPath)
            {
                var propertyName = bindingPath.Split('.').Last();
                editor.primarySquare.SetBinding(ContentView.BackgroundColorProperty, bindingPath);
                editor.primarySquare.CommandParameter = bindingPath;
                editor.primaryLabel.SetBinding(Label.TextColorProperty, bindingPath, converter: new ToSurfaceColorConverter());
                editor.primaryLabel.Text = propertyName;

                var onPropertyName = "On" + propertyName.Split('.').Last();
                var onBindingPath = bindingPath.Replace(propertyName, onPropertyName);

                editor.onSquare.SetBinding(ContentView.BackgroundColorProperty, bindingPath);
                editor.onSquare.CommandParameter = onBindingPath;
                editor.onLabel.SetBinding(Label.TextColorProperty, onBindingPath);
                editor.onLabel.Text = onPropertyName;
            }
        });

    public ICommand EditCommand { get => (ICommand)GetValue(EditCommandProperty); set => SetValue(EditCommandProperty, value); }
    public static readonly BindableProperty EditCommandProperty = BindableProperty.Create(
        nameof(EditCommand),
        typeof(ICommand),
        typeof(ColorPairEditorView),
        defaultValue: null,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is ColorPairEditorView editor && newValue is ICommand command)
            {
                editor.primarySquare.TappedCommand = command;
                editor.onSquare.TappedCommand = command;
            }
        });
}