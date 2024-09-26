using InputKit.Shared.Abstraction;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace UraniumUI.Validations;

public class DataAnnotationsBehavior : Behavior<View>
{
    public BindingBase Binding { get; set; }

    protected BindableObject bindable;

    protected override void OnAttachedTo(BindableObject bindable)
    {
        base.OnAttachedTo(bindable);
        this.bindable = bindable;
        Apply();
    }

    protected override void OnAttachedTo(View bindable)
    {
        base.OnAttachedTo(bindable);
        Apply();

        bindable.BindingContextChanged -= Bindable_BindingContextChanged;
        bindable.BindingContextChanged += Bindable_BindingContextChanged;
    }

    protected override void OnDetachingFrom(View bindable)
    {
        base.OnDetachingFrom(bindable);
        bindable.BindingContextChanged -= Bindable_BindingContextChanged;
    }

    private void Bindable_BindingContextChanged(object sender, EventArgs e)
    {
        Apply();
    }

    void Apply()
    {
        if (bindable is not IValidatable validatable)
        {
            return;
        }

        if (Binding is Binding newBinding)
        {
            var source = newBinding.Source ?? bindable.BindingContext;

            if (source is null)
            {
                return;
            }

            var propertyInfo = source.GetType().GetProperty(newBinding.Path);
            var validationAttributes = propertyInfo.GetCustomAttributes<ValidationAttribute>(true);
            var displayAttribute = propertyInfo.GetCustomAttribute<DisplayAttribute>(true);

            foreach (var attribute in validationAttributes)
            {
                validatable.Validations.Add(new DataAnnotationValidation(attribute, displayAttribute?.GetName() ?? propertyInfo.Name));
            }
        }
    }
}
