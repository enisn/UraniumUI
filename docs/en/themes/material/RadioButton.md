# RadioButton
The `RadioButton` component is used to select a single option from a set of options. UraniumU UI uses [InputKit CheckBox](https://enisn-projects.io/docs/en/inputkit/latest/components/controls/CheckBox) instead of MAUI one. It is because InputKit CheckBox is more customizable and already has a Material theme.

![MAUI Material Design Radio Button](https://lh3.googleusercontent.com/7IADEr51nTh9IXxTqAYHJ50lpdDR8ZiBLvpAckIhRgD66Xtn-kkX2I8iVx3vZZ8sh6_fAqscCyxkbpdlDdzbRVukOOgB5SHxBXJF9KI=w1064-v0)


## Features

You can visit [InputKit CheckBox Documentation](https://enisn-projects.io/docs/en/inputkit/latest/components/controls/CheckBox) to see features. UraniumUI applies only visual changes on it.


## Usage

CheckBox is defined in `UraniumUI.Material.Controls` namespace. You can use it like this:


```xml
xmlns:material="clr-namespace:UraniumUI.Material.Controls;assembly=UraniumUI.Material"
```

```xml
<StackLayout MaximumWidthRequest="400" Margin="20">

    <material:RadioButtonGroupView>
        <material:RadioButton Text="Option 1" />
        <material:RadioButton Text="Option 2" />
        <material:RadioButton Text="Option 3 (Disabled)" IsDisabled="True" />
        <material:RadioButton Text="Option 4 (Disabled)" IsDisabled="True" IsChecked="True" />
    </material:RadioButtonGroupView>

    <BoxView StyleClass="Divider" />

    <material:RadioButtonGroupView>
        <material:RadioButton Text="Option 1" LabelPosition="Before" />
        <material:RadioButton Text="Option 2" LabelPosition="Before" />
        <material:RadioButton Text="Option 3 (Disabled)" IsDisabled="True" LabelPosition="Before" />
        <material:RadioButton Text="Option 4 (Disabled)" IsDisabled="True" IsChecked="True" LabelPosition="Before" />
    </material:RadioButtonGroupView>

    <BoxView StyleClass="Divider" />

    <material:RadioButtonGroupView>
        <material:RadioButton Text="Check Radio Button 1" StyleClass="CheckRadioButton" IsChecked="True"/>
        <material:RadioButton Text="Check Radio Button 1" StyleClass="CheckRadioButton" />
    </material:RadioButtonGroupView>
</StackLayout>
```

| Dark - Desktop | Light - Mobile |
| --- | --- |
| ![MAUI Material Design RadioButton](images/radiobutton-demo-windows-dark.gif) | ![MAUI Material Design RadioButton](images/radiobutton-demo-android-light.gif)  |