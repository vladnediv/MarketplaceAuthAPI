using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BLL.Model.Attribute;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class NotEqualAttribute : ValidationAttribute
{
    private string _otherPropertyName { get; set; }
    public NotEqualAttribute(string otherPropertyName)
    {
        this._otherPropertyName = otherPropertyName;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        PropertyInfo? otherProperty = validationContext.ObjectType.GetProperty(_otherPropertyName);

        if (otherProperty == null)
        {
            return new ValidationResult($"Unknown property '{_otherPropertyName}'");
        }
        object? otherValue = otherProperty.GetValue(validationContext.ObjectInstance);

        if( Equals(value, otherValue))
        {
            return new ValidationResult($"'{validationContext.MemberName}' must not be the same as '{_otherPropertyName}'");
        }
        return ValidationResult.Success;
    }
}