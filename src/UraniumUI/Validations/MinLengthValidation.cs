namespace UraniumUI.Validations;
public class MinLengthValidation : IValidation
{
    private string message;

    public string Message { get => message ?? $"The field should contain at least {MinLength} character."; set => message = value; }
    public int MinLength { get; set; }

    public bool Validate(object value)
    {
        if (value is string text)
        {
            return text.Length >= MinLength;
        }

        return false;
    }
}
