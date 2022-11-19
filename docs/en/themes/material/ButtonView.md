# ButtonView
ButtonView is a plain control that allows you place your content inside it. It is a simple wrapper around the [StatefulContentView](../../../en/infrastructure/StatefulContentView.md) control. 

ButtonView included in **Material Theme**.

## Usage

`ButtonView` is defined in `UraniumUI.Material.Controls` namespace. You can add it to your XAML like this:

```xml
xmlns:material="clr-namespace:UraniumUI.Material.Controls;assembly=UraniumUI.Material"
```

Then you can use it like this:

```xml
<material:ButtonView>
    <Label Text="Hello UraniumUI 👋" />
</material:ButtonView>
```

![uraniumui buttonview](images/buttonview-demo.png)