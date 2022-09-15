namespace UraniumUI.Validations;
public interface IValidation
{
    string Message { get; }
    bool Validate(object value);
}
