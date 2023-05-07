using Shouldly;
using UraniumUI.Tests.Core;
using UraniumUI.Views;

namespace UraniumUI.Tests.Views;
public class DynamicContent_Test
{
    public DynamicContent_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
    }

    [Fact]
    public void ContentShouldBeNull_WhenThereIsNoCondition()
    {
        var control = AnimationReadyHandler.Prepare(new DynamicContentView());

        control.Content.ShouldBeNull();
    }

    [Fact]
    public void ShouldRender_DefaultContent_WhenThereIsNoCondition()
    {
        var control = AnimationReadyHandler.Prepare(new DynamicContentView());
        var label = new Label { Text = "Test" };
        control.Content = label;

        control.Content.ShouldBe(label);
    }

    [Fact]
    public void ConditionEqual_ShouldWork_WithOneCondition()
    {
        var control = AnimationReadyHandler.Prepare(new DynamicContentView());
        
        var testValue = "1";

        control.Conditions.Add(new ValueCondition
        {
            Equal = testValue,
            ContentTemplate = new DataTemplate(() => new Label { Text = testValue })
        });

        control.Value = testValue;
        var label = control.Content.ShouldBeOfType<Label>();
        label.Text.ShouldBe(testValue);
    }

    [Fact]
    public void ConditionEqual_ShouldBeUpdatedOnRuntime_WithTwoCondition()
    {
        var control = AnimationReadyHandler.Prepare(new DynamicContentView());


        control.Conditions.Add(new ValueCondition
        {
            Equal = "1",
            ContentTemplate = new DataTemplate(() => new Label { Text = "1" })
        });

        control.Conditions.Add(new ValueCondition
        {
            Equal = "2",
            ContentTemplate = new DataTemplate(() => new Label { Text = "2" })
        });

        control.Value = "1";
        var label = control.Content.ShouldBeOfType<Label>();
        label.Text.ShouldBe("1");

        control.Value = "2";
        var label2 = control.Content.ShouldBeOfType<Label>();
        label2.Text.ShouldBe("2");
    }



    [Fact]
    public void ConditionGreaterThan_ShouldWork_WithOneCondition()
    {
        var control = AnimationReadyHandler.Prepare(new DynamicContentView());

        var text = "Expected Text";

        control.Conditions.Add(new ValueCondition
        {
            GreaterThan = 2,
            ContentTemplate = new DataTemplate(() => new Label { Text = text })
        });

        control.Content.ShouldBeNull();
        control.Value = 3;
        var label = control.Content.ShouldBeOfType<Label>();
        label.Text.ShouldBe(text);
    }

    [Fact]
    public void ConditionGreaterThan_ShouldBeUpdatedOnRuntime_WithTwoCondition()
    {
        var control = AnimationReadyHandler.Prepare(new DynamicContentView());

        control.Conditions.Add(new ValueCondition
        {
            GreaterThan = 10,
            ContentTemplate = new DataTemplate(() => new Label { Text = "second" })
        });

        control.Conditions.Add(new ValueCondition
        {
            GreaterThan = 1,
            ContentTemplate = new DataTemplate(() => new Label { Text = "first" })
        });

        control.Value = 5;
        var label = control.Content.ShouldBeOfType<Label>();
        label.Text.ShouldBe("first");

        control.Value = 12;
        var label2 = control.Content.ShouldBeOfType<Label>();
        label2.Text.ShouldBe("second");
    }
}
