namespace UraniumUI.Validations;
public class RequiredValidation : IValidation
{
    public string Message { get; set; } = "This field is required";

    public bool Validate(object value)
    {
        if (value is string text)
        {
            return !string.IsNullOrEmpty(text);
        }

        return value != null;
    }
}
