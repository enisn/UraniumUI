# DataGrid
DataGrid is a table to present complicated data.

## Getting Started
DataGrid is included in the `Uranium.UI.Controls` namespace. You can add it to your XAML like this:

```
xmlns:controls="clr-namespace:UraniumUI.Material.Controls;assembly=UraniumUI.Material"
```

DataGrid can't be used standalone without csharp code. You need to bind some data to `ItemsSource` property.

```csharp
public class MainPageViewModel : BindableObject
{
	static Random random = new();
	public ObservableCollection<Student> Items { get; } = new();
	public MainPageViewModel()
	{
		for (int i = 0; i < 10; i++)
		{
			Items.Add(new Student
			{
				Id = i,
				Name = "Person " + i,
				Age = random.Next(14, 85),
			});
		}
    }
	public class Student
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Age { get; set; }
	}
}
```

Then use it in XAML like this:

```xml
 <controls:DataGrid ItemsSource="{Binding Items}" HorizontalOptions="Center" Margin="30" />
```

![MAUI datagrid](images/datagrid-demo.png)


## Customizations

### Columns
Columns are automatically detected by **DataGrid**. It uses reflection to get properties of the data source. You can customize the columns by using following attributes.

_... coming soon ..._