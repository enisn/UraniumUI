using System.Collections.ObjectModel;
using System.Windows.Input;

namespace UraniumApp.Pages.DataGrids;

public class CustomDataGridPageViewModel : BindableObject
{
    static Random random = new();

    private ObservableCollection<Student> items;
    public ObservableCollection<Student> Items { get => items; protected set { items = value; OnPropertyChanged(); } }
    private bool isBusy;
    public bool IsBusy { get => isBusy; set { isBusy = value; OnPropertyChanged(); } }
    protected StudentDataStore DataStore { get; } = new StudentDataStore();
    public ICommand AddNewCommand { get; set; }
    public ICommand RemoveItemCommand { get; set; }
    public int Row { get; set; }
    public string Name { get; set; } = "Student Custom 1";

    public CustomDataGridPageViewModel()
    {
        Initialize();

        AddNewCommand = new Command(() =>
        {
            Items.Insert(Row, new Student
            {
                Age = random.Next(10, 99),
                Id = random.Next(0, int.MaxValue),
                Name = Name
            });
        });

        RemoveItemCommand = new Command((item) =>
        {
            if (item is Student student)
            {
                items.Remove(student);
            }
        });
    }
    protected virtual bool SimulateNetwork => true;

    async void Initialize()
    {
        IsBusy = true;
        Items = new ObservableCollection<Student>(await DataStore.GetListAsync(SimulateNetwork));
        IsBusy = false;
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public Guid SecurityStamp { get; set; } = Guid.NewGuid();
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow.AddDays(-1 * random.Next(1, 2100));
    }

    public class StudentDataStore
    {
        public async Task<List<Student>> GetListAsync(bool simulateNetwork = true)
        {
            if (simulateNetwork)
            {
                await Task.Delay(random.Next(500, 2000));
            }

            var list = new List<Student>();

            for (int i = 0; i < 18; i++)
            {
                list.Add(new Student
                {
                    Id = i,
                    Name = "Student " + i,
                    Age = random.Next(14, 85),
                });
            }

            return list;
        }
    }
}
