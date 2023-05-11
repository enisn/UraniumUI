using InputKit.Shared.Abstraction;
using InputKit.Shared.Validations;
using UraniumUI.Pages;
using UraniumUI.Resources;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace UraniumUI.Material.Controls;

public partial class InputField : IValidatable
{
    public List<IValidation> Validations { get; } = new();
    public bool IsValid { get => ValidationResults().All(x => x.isValid); }

    protected Lazy<ContentView> iconValidation = new Lazy<ContentView>(() => new ContentView
    {
        VerticalOptions = LayoutOptions.Center,
        HorizontalOptions = LayoutOptions.End,
        Padding = new Thickness(5, 0),
        Margin = new Thickness(0, 0, 5, 0),
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
        this.AddRowDefinition(new RowDefinition(GridLength.Auto));
        this.AddRowDefinition(new RowDefinition(GridLength.Auto));
    }

    protected virtual void CheckAndShowValidations()
    {
        var results = ValidationResults().ToArray();
        var isValidationPassed = results.All(a => a.isValid);

        var isStateChanged = isValidationPassed != lastValidationState;

        lastValidationState = isValidationPassed;

        if (isValidationPassed)
        {
            if (isStateChanged)
            {
                endIconsContainer.Remove(iconValidation.Value);
                this.Remove(labelValidation.Value);
            }
        }
        else
        {
            var message = string.Join('\n', results.Where(x => !x.isValid).Select(s => s.message));
            labelValidation.Value.Text = message;

            if (isStateChanged)
            {
                endIconsContainer.Add(iconValidation.Value);
                this.Add(labelValidation.Value, row: 1);
            }
        }
    }

    protected IEnumerable<(bool isValid, string message)> ValidationResults()
    {
        foreach (var validation in Validations)
        {
            yield return ValidateOne(validation);
        }
    }

    protected (bool isValid, string message) ValidateOne(IValidation validation)
    {
        try
        {
            var value = GetValueForValidator();
            var validated = validation.Validate(value);
            return new(validated, validation.Message);
        }
        catch (Exception ex)
        {
            return new(false, ex.Message);
        }
    }

    protected virtual object GetValueForValidator()
    {
        return new object();
    }

    public void DisplayValidation()
    {
        CheckAndShowValidations();
    }
}