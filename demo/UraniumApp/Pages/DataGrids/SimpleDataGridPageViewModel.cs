using ReactiveUI.Fody.Helpers;
using System.ComponentModel;

namespace UraniumApp.Pages.DataGrids;
public class SimpleDataGridPageViewModel : BindableObject
{
	static Random random = new();
	public List<Student> Items { get; } = new();
	public SimpleDataGridPageViewModel()
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
	public class Student : ReactiveUI.ReactiveObject
	{
		[DisplayName("Identity Number")]
		[Reactive] public int Id { get; set; }
		[Reactive] public string Name { get; set; }
		[Reactive] public int Age { get; set; }
	}
}
