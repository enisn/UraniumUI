using InputKit.Shared.Validations;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace UraniumUI.Validations;
public class DataAnnotationValidation : IValidation
{
    public string Message { get; protected set; }

    public ValidationAttribute Attribute { get; }

    public string PropertyName { get; }

    public DataAnnotationValidation(ValidationAttribute attribute, string propertyName)
    {
        Attribute = attribute;
        PropertyName = propertyName;
        Message = Attribute.ErrorMessage;
    }

    public bool Validate(object value)
    {
        try
        {
            Attribute.Validate(value, PropertyName);
            return true;
        }
        catch (ValidationException ex)
        {
            Message = ex.Message;
            return false;
        }
    }

    public static IEnumerable<IValidation> CreateValidations(PropertyInfo property)
    {
        var attributes = property.GetCustomAttributes<ValidationAttribute>();

        foreach (var attribute in attributes)
        {
            yield return new DataAnnotationValidation(attribute, property.Name);
        }
    }
}