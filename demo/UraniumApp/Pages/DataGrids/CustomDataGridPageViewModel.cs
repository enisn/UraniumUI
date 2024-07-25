using Bogus;
using DotNurse.Injector.Attributes;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UraniumUI.Dialogs;

namespace UraniumApp.Pages.DataGrids;

[RegisterAs(typeof(CustomDataGridPageViewModel))]
public class CustomDataGridPageViewModel : BindableObject
{
    private ObservableCollection<Student> items;
    public ObservableCollection<Student> Items { get => items; protected set { items = value; OnPropertyChanged(); } }
    private bool isBusy;
    public bool IsBusy { get => isBusy; set { isBusy = value; OnPropertyChanged(); } }
    protected StudentDataStore DataStore { get; } = new StudentDataStore();
    public ICommand AddNewCommand { get; set; }
    public ICommand RemoveItemCommand { get; set; }
    public int Row { get; set; }

    public CustomDataGridPageViewModel(IDialogService dialogService)
    {
        Initialize();

        AddNewCommand = new Command(async () =>
        {
            var newStudent = StudentDataStore.faker.Generate();

            var result = await dialogService.DisplayFormViewAsync("New Student", newStudent);
            if (result != null)
            {
                Items.Add(result);
            }
        });

        RemoveItemCommand = new Command((item) =>
        {
            if (item is Student student)
            {
                items.Remove(student);
            }
        });
    }

    protected async virtual void Initialize()
    {
        IsBusy = true;
        Items = new ObservableCollection<Student>(await DataStore.GetListAsync());
        IsBusy = false;
    }
}

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public Guid SecurityStamp { get; set; } = Guid.NewGuid();
    public DateTime RegistrationDate { get; set; }
}

public class StudentDataStore
{
    internal static Faker<Student> faker = new Faker<Student>()
        .RuleFor(x => x.Id, f => f.IndexFaker)
        .RuleFor(x => x.Name, f => f.Person.FullName)
        .RuleFor(x => x.Age, f => f.Random.Number(14, 85))
        .RuleFor(x => x.SecurityStamp, f => f.Random.Guid())
        .RuleFor(x => x.RegistrationDate, f => f.Date.Past(1));

    public async Task<List<Student>> GetListAsync(bool simulateNetwork = true)
    {
        if (simulateNetwork)
        {
            await Task.Delay(Random.Shared.Next(500, 2000));
        }

        var list = new List<Student>();

        for (int i = 0; i < 12; i++)
        {
            list.Add(faker.Generate());
        }

        return list;
    }
}
