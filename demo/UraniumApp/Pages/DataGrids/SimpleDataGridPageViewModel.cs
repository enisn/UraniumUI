using Bogus;
using System.ComponentModel;

namespace UraniumApp.Pages.DataGrids;
public class SimpleDataGridPageViewModel : BindableObject
{
    public List<Student> Items { get; } = new();
    public SimpleDataGridPageViewModel()
    {
        for (int i = 0; i < 10; i++)
        {
            Items.Add(GenerateStudent());
        }
    }

    private static Faker<Student> studentFaker = new Faker<Student>()
        .RuleFor(x => x.Id, f => f.IndexFaker)
        .RuleFor(x => x.Name, f => f.Person.FullName)
        .RuleFor(x => x.Age, f => f.Random.Number(14, 85));

    public static Student GenerateStudent()
    {
        return studentFaker.Generate();
    }

    public class Student
    {
        [DisplayName("Identity")]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
