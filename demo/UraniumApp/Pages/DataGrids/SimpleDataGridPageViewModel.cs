using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	public class Student
	{
		[DisplayName("Identity Number")]
		public int Id { get; set; }
		public string Name { get; set; }
		public int Age { get; set; }
	}
}
