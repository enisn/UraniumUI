using Bogus;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;

namespace UraniumApp.Pages.DataGrids;
public class EditorDataGridPageViewModel : BindableObject
{
    public List<ReactiveStudent> Items { get; } = new();
    public EditorDataGridPageViewModel()
    {
        for (int i = 0; i < 10; i++)
        {
            Items.Add(GenerateStudent());
        }
    }

    private static Faker<ReactiveStudent> studentFaker = new Faker<ReactiveStudent>()
        .RuleFor(x => x.Id, f => f.IndexFaker)
        .RuleFor(x => x.Name, f => f.Person.FullName)
        .RuleFor(x => x.Age, f => f.Random.Number(14, 85));

    public static ReactiveStudent GenerateStudent()
    {
        return studentFaker.Generate();
    }
}

public class ReactiveStudent : ReactiveUI.ReactiveObject
{
    [Reactive] public int Id { get; set; }
    [Reactive] public string Name { get; set; }
    [Reactive] public int Age { get; set; }


}