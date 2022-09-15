using Microsoft.Maui.Controls.Shapes;
using UraniumUI.Pages;
using UraniumUI.Resources;
using UraniumUI.Validations;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;
public partial class InputField
{
    public IList<IValidation> Validations { get; } = new List<IValidation>();

    protected Lazy<ContentView> iconValidation = new Lazy<ContentView>(() => new ContentView
    {
        Padding = new Thickness(10,0),
        VerticalOptions = LayoutOptions.Center,
        HorizontalOptions = LayoutOptions.End,
        Content = new Path
        {
            Fill = ColorResource.GetColor("Error", "ErrorDark", Colors.Red),
            Data = UraniumShapes.ExclamationCircle,
        }
    });

    protected Lazy<Label> labelValidation = new Lazy<Label>(() => new Label
    {
        HorizontalOptions = LayoutOptions.Start,
        TextColor = ColorResource.GetColor("Error", "ErrorDark", Colors.Red),
    });

    protected bool lastValidationState = true;
    protected virtual void InitializeValidation()
    {
        rootGrid.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
        rootGrid.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));

        this.AddRowDefinition(new RowDefinition(GridLength.Auto));
        this.AddRowDefinition(new RowDefinition(GridLength.Auto));
        // Keep for initialization.
    }

    protected virtual void CheckAndShowValidations()
    {
        var results = ValidationResults().ToArray();
        var isValidationPassed = results.All(a => a.result);

        var isStateChanged = isValidationPassed != lastValidationState;

        lastValidationState = isValidationPassed;

        if (isValidationPassed)
        {
            if (isStateChanged)
            {
                rootGrid.Remove(iconValidation.Value);
                this.Remove(labelValidation.Value);
            }
        }
        else
        {
            var message = string.Join(",\n", results.Select(s => s.message));
            labelValidation.Value.Text = message;

            if (isStateChanged)
            {
                rootGrid.Add(iconValidation.Value);
                this.Add(labelValidation.Value, row: 1);
            }
        }
    }

    protected IEnumerable<(bool result, string message)> ValidationResults()
    {
        foreach (var validation in Validations)
        {
            var validated = validation.Validate(GetValueForValidator());
            yield return new(validated, validation.Message);
        }
    }

    protected virtual object GetValueForValidator()
    {
        return new object();
    }
}