# DynamicContentView
DynamicContentView is a view that can be used to display different content based on the current state. It's a useful when you want to render different content based on a value. Multiple conditions can be defined to **DynamicContentView** and it finds the matching condition according to your rules and render matching condision's content. Conditions use `DataTemplate` to define content. So, how many conditions you define isn't so important. They won't be initialized and attached to visual tree until your condition has met. It's a memory efficient way to render different content instead setting IsVisible properties of views.

## Usage

DynamicContentView is defined in `UraniumUI.Views` namespace. You can use it in XAML like this:

```
xmlns:uranium="http://schemas.enisn-projects.io/dotnet/maui/uraniumui"
```

Then you can use it with `uranium:DynamicContentView` tag.

```xml
<uranium:DynamicContentView Value="{Binding IsBusy}">
    <uranium:DynamicContentView.Conditions>

        <uranium:ValueCondition Equal="True">
            <DataTemplate>
                <ActivityIndicator IsRunning="True" />
            </DataTemplate>
        </uranium:ValueCondition>

        <uranium:ValueCondition Equal="False">
            <DataTemplate>
                <Label  Text="Actual Content Here"/>
            </DataTemplate>
        </uranium:ValueCondition>

    </uranium:DynamicContentView.Conditions>
</uranium:DynamicContentView>
```

### Conditions
`Condition` is a bindable object that contains a `DataTemplate` as Content and `Equal`, `GreaterThan`, `LessThan`, `Not` properties. Each property represents a comparison to `Value` property of `DynamicContentView`. When `Value` property of `DynamicContentView` changes, it finds the matching condition and renders the content of it. If no condition matches, it doesn't render anything. You can define multiple conditions and it finds the matching condition according to your rules. If multiple condition matches, it renders the first matching condition's content. So, order of condition definition is important. You can place a fallback condition at the bottom that matches everything as a fallback condition.

```xml
<uranium:DynamicContentView Value="{Binding MyNumber}">
    <uranium:DynamicContentView.Conditions>

        <uranium:ValueCondition GreaterThan="5">
            <DataTemplate>
                <Label  Text="Value is true" TextColor="Green"/>
            </DataTemplate>
        </uranium:ValueCondition>
        
        <uranium:ValueCondition Equal="5">
            <DataTemplate>
                <Label  Text="Value is exactly 5"/>
            </DataTemplate>
        </uranium:ValueCondition>

        <uranium:ValueCondition LessThan="5">
            <DataTemplate>
                <Label  Text="Value is false" TextColor="Red"/>
            </DataTemplate>
        </uranium:ValueCondition>

    </uranium:DynamicContentView.Conditions>
</uranium:DynamicContentView>
```