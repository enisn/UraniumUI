# Validations
Validations are a way to validate the input of a field. They are defined in the `Validations` property of a control that implements `IValidatable`. The `validations` property is an array of validation objects which implements `IValidation` interface. They are some prebuilt validations that you can use or you can create your own validation. Unfortunetely, plain MAUI controls can't be validated at the moment.

> _You may visit [Validations](https://enisn-projects.io/docs/en/inputkit/latest/components/controls/FormView#validations) section of [FormView](https://enisn-projects.io/docs/en/inputkit/latest/components/controls/FormView) documentation of InputKit to see existing validations._

> _If you're looking for **DataAnnotations** validation, you should check out the [DataAnnotations](../../validations/DataAnnotations.md) documentation._

Validations are working compatible together FormView. So, you can use a FormView to create a form and validate the inputs.

| Light - Mobile | Dark - Desktop |
|--- | --- |
| ![MAUI Validation](../../images/validations-all-demo-light-android.gif) | ![MAUI Validation](../../images/validations-all-demo-dark-windows.gif) |


## Usage

* Prebuilt validations are defined in `InputKit.Shared.Validations` namespace. You should add an xml namespace to use them or define your own namespace that contains your validation rules.

```
xmlns:validation="clr-namespace:InputKit.Shared.Validations;assembly=InputKit.Maui"
```

* Each UraniumUI Material control accepts validations as content. So, you can define validations in the control's content.

```xml
<material:TextField Title="Fullname" Text="{Binding FullName}">
    <validation:RequiredValidation />
    <validation:LettersOnlyValidation AllowSpaces="True" />
</material:TextField>
```

* Using a [FormView](https://enisn-projects.io/docs/en/inputkit/latest/components/controls/FormView) is the easiest way to validate multiple inputs at the same time.

```xml
<input:FormView SubmitCommand="{Binding SubmitCommand}" Spacing="20">

    <material:TextField Title="Email" Text="{Binding Email}">
        <validation:RequiredValidation />
        <validation:RegexValidation Message="Please type a valid e-mail address." Pattern="{x:Static input:AdvancedEntry.REGEX_EMAIL}"/>
    </material:TextField>

    <material:CheckBox Text="I Accept Terms &amp; Conditions"
                        IsChecked="{Binding IsTermsAndConditionsAccepted}">
        <validation:RequiredValidation />
    </material:CheckBox>

    <Button Text="Submit"
            input:FormView.IsSubmitButton="True"
            StyleClass="FilledButton"/>
</input:FormView>
```

| Light | Dark |
| --- | --- |
| ![MAUI Validations](../../images/validations-demo-light-android.gif) | ![MAUI Validations](../../images/validations-demo-dark-windows.gif) |


## Prebuilt Validations
UraniumUI Material doesn't provide any prebuilt validations. You can use validations from [InputKit](https://enisn-projects.io/docs/en/inputkit/latest/components/controls/FormView#validations) or create your own.

There are some built-in validations that can be used in your application. They are:

  * `RequiredValidation` - Checks if the value is not null or empty.
  * `RegexValidation` - Checks if the value matches the given regex pattern.
  * `MinLengthValidation` - Checks if the string length is greater than or equal to the given length.
  * `MaxLengthValidation` - Checks if the string length is less than or equal to the given length.
  * `MaxValueValidation` - Checks if the value is less than or equal to the given value.
  * `MinValueValidation` - Checks if the value is greater than or equal to the given value.
  * `NumericValidation` - Checks if the value is a number.
  * `DigitsOnlyValidation` - Checks if the value contains only digits.
  * `LettersOnlyValidation` - Checks if the value contains only letters.

## Creating Custom Validation

You can create your own validation by implementing `IValidation` interface. It has a `Validate` method that takes a `object` as parameter and returns `bool` as result. The parameter is the value of the control that the validation is applied to. The result is the validation result. If the result is `false`, the validation message will be shown.

```csharp

public class MyEmailValidation : IValidation
{
    public string Message { get; set; } = "Please enter a valid email address.";

    public bool Validate(object value)
    {
        if (value is string text)
        {
            return text.Count(x => x == '@') == 1 && text.Split('@').Last().Length >= 2;
        }
        return false;
    }
}
```

## Validation Supported Controls

- **TextField** : Any validation according to the selected item can be applied.
- **PickerField** : Any validation according to the selected item can be applied.
- **DatePickerField** : `RequiredValidation`, `MinValueValidation` and `MaxValueValidation` are supported.
- **TimePickerField** : `RequiredValidation`, `MinValueValidation` and `MaxValueValidation` are supported.
- **CheckBox** : _Only `RequiredValidation` is supported._
- **RadioButton** _(RadioButtonGroupView from InputKit)_
- **AdvancedSlider** _(From InputKit)_
