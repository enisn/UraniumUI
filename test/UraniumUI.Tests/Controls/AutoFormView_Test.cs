using InputKit.Shared.Abstraction;
using InputKit.Shared.Validations;
using UraniumUI.Controls;
using UraniumUI.Options;
using UraniumUI.Tests.Core;

namespace UraniumUI.Tests.Controls;

public class AutoFormView_Test
{
    public AutoFormView_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
    }

    [Fact]
    public void PropertyDefinitions_ShouldRenderDescriptorsWithoutPropertyInfo()
    {
        var source = new TestModel { Name = "Jane" };
        var formView = new AutoFormView
        {
            ShowSubmitButton = false,
            ShowResetButton = false,
            PropertyDefinitions = new[]
            {
                new AutoFormProperty(nameof(TestModel.Name), typeof(string), "Full name")
            },
            Source = source
        };

        Assert.Empty(formView.EditingProperties);
        var property = Assert.Single(formView.EditingPropertyDefinitions);
        Assert.Null(property.PropertyInfo);

        var editor = Assert.Single(GetRenderedEditors(formView));
        var layout = Assert.IsType<VerticalStackLayout>(editor);
        var label = Assert.IsType<Label>(layout.Children[0]);
        var entry = Assert.IsType<Entry>(layout.Children[1]);

        Assert.Equal("Full name", label.Text);
        Assert.Equal(nameof(TestModel.Name), FormView.GetValidationPath(entry));
    }

    [Fact]
    public void PropertyProvider_ShouldOverrideReflectionDiscovery()
    {
        var providerCalled = false;
        var formView = new AutoFormView
        {
            ShowSubmitButton = false,
            ShowResetButton = false,
            PropertyProvider = source =>
            {
                providerCalled = true;
                Assert.IsType<TestModel>(source);

                return new[]
                {
                    new AutoFormProperty(nameof(TestModel.Seats), typeof(int?), "Seats", isNullable: true)
                };
            },
            Source = new TestModel()
        };

        Assert.True(providerCalled);
        Assert.Empty(formView.EditingProperties);
        Assert.Equal(nameof(TestModel.Seats), Assert.Single(formView.EditingPropertyDefinitions).Name);

        var editor = Assert.Single(GetRenderedEditors(formView));
        var layout = Assert.IsType<VerticalStackLayout>(editor);
        var label = Assert.IsType<Label>(layout.Children[0]);

        Assert.Equal("Seats", label.Text);
    }

    [Fact]
    public void ReflectionFallback_ShouldUseLegacyPropertyNameFactory()
    {
        ApplicationExtensions.CreateAndSetMockApplication(builder =>
        {
            builder.Services.Configure<AutoFormViewOptions>(options =>
            {
                options.PropertyNameFactory = property => $"Legacy {property.Name}";
            });
        });

        var formView = new AutoFormView
        {
            ShowSubmitButton = false,
            ShowResetButton = false,
            Source = new NameOnlyModel()
        };

        Assert.NotEmpty(formView.EditingProperties);

        var editor = Assert.Single(GetRenderedEditors(formView));
        var layout = Assert.IsType<VerticalStackLayout>(editor);
        var label = Assert.IsType<Label>(layout.Children[0]);

        Assert.Equal("Legacy Name", label.Text);
    }

    [Fact]
    public void ReflectionFallback_ShouldUseLegacyEditorMapping()
    {
        ApplicationExtensions.CreateAndSetMockApplication(builder =>
        {
            builder.Services.Configure<AutoFormViewOptions>(options =>
            {
                options.EditorMapping[typeof(string)] = (property, propertyNameFactory, source) => new Label
                {
                    Text = $"Legacy {propertyNameFactory(property)}"
                };
            });
        });

        var formView = new AutoFormView
        {
            ShowSubmitButton = false,
            ShowResetButton = false,
            Source = new NameOnlyModel()
        };

        var editor = Assert.Single(GetRenderedEditors(formView));
        var label = Assert.IsType<Label>(editor);

        Assert.Equal("Legacy Name", label.Text);
    }

    [Fact]
    public void DescriptorOptions_ShouldApplyValidationAndPostEditorActions()
    {
        ApplicationExtensions.CreateAndSetMockApplication(builder =>
        {
            builder.Services.Configure<AutoFormViewOptions>(options =>
            {
                options.PropertyEditorMapping[typeof(string)] = (_, _, _) => new ValidatableView();
                options.PropertyValidationFactory = property => property
                    .GetCustomAttributes<MarkerAttribute>()
                    .Select(attribute => new MarkerValidation(attribute.Message));
                options.PostPropertyEditorActions.Add((view, property) => view.AutomationId = $"editor-{property.Name}");
            });
        });

        var formView = new AutoFormView
        {
            ShowSubmitButton = false,
            ShowResetButton = false,
            PropertyDefinitions = new[]
            {
                new AutoFormProperty(
                    nameof(TestModel.Name),
                    typeof(string),
                    attributes: new Attribute[] { new MarkerAttribute("Generated metadata") })
            },
            Source = new TestModel()
        };

        var field = Assert.IsType<ValidatableView>(Assert.Single(GetRenderedEditors(formView)));
        var validation = Assert.IsType<MarkerValidation>(Assert.Single(field.Validations));

        Assert.Equal("Generated metadata", validation.Message);
        Assert.Equal("editor-Name", field.AutomationId);
    }

    private static IView[] GetRenderedEditors(AutoFormView formView)
    {
        return formView.ItemsLayout.Children
            .Where(child => !ReferenceEquals(child, formView.FooterLayout))
            .ToArray();
    }

    private sealed class TestModel
    {
        public string Name { get; set; }

        public int? Seats { get; set; }
    }

    private sealed class NameOnlyModel
    {
        public string Name { get; set; }
    }

    private sealed class MarkerAttribute : Attribute
    {
        public MarkerAttribute(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    private sealed class MarkerValidation : IValidation
    {
        public MarkerValidation(string message)
        {
            Message = message;
        }

        public string Message { get; }

        public bool Validate(object value) => true;
    }

    private sealed class ValidatableView : ContentView, IValidatable
    {
        public List<IValidation> Validations { get; } = new();

        public bool IsValid => true;

        public void DisplayValidation()
        {
        }

        public void ResetValidation()
        {
        }
    }
}
