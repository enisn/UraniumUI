using System.Collections.ObjectModel;
using System.Windows.Input;
using UraniumUI;

namespace MyCompany.MyProject;
public class MainPageViewModel : BindableObject
{
    public ObservableCollection<TodoItem> Items { get; protected set; } = new ObservableCollection<TodoItem>();

    public ObservableCollection<TodoItem> SelectedItems { get; set; } = new ObservableCollection<TodoItem>();

    private TodoItem newItem = new();
    public TodoItem NewItem { get => newItem; set { newItem = value; OnPropertyChanged(); } }

    public ICommand AddNewItemCommand { get; protected set; }

    public ICommand RemoveSelectedItemsCommand { get; protected set; }

    public MainPageViewModel()
    {
        if (Items.Count == 0)
        {
            Items.Add(new TodoItem { Content = "Throw away the rubbish", Type = TodoItem.TodoItemType.Personal });
            Items.Add(new TodoItem { Content = "Attend the meeting today\n11:00AM", Type = TodoItem.TodoItemType.Work });
            Items.Add(new TodoItem { Content = "Prepare presentation for new project", Type = TodoItem.TodoItemType.Work });
            Items.Add(new TodoItem { Content = "Spend time with family", Type = TodoItem.TodoItemType.Family });
            Items.Add(new TodoItem { Content = "Complete the puzzle", Type = TodoItem.TodoItemType.Hobby });
            Items.Add(new TodoItem { Content = "Don't forget to call dad", Type = TodoItem.TodoItemType.Family });
        }

        AddNewItemCommand = new Command(() =>
        {
            Items.Insert(0, NewItem);
            NewItem = new();
        });

        RemoveSelectedItemsCommand = new Command(() =>
        {
            foreach (var item in SelectedItems)
            {
                Items.Remove(item);
            }
        });
    }
}

public class TodoItem : UraniumBindableObject
{
    public string Content { get; set; }

    private bool isDone;
    public bool IsDone { get => isDone; set => SetProperty(ref isDone,value); }

    public TodoItemType Type { get; set; }

    public static TodoItemType[] AvailableTypes => Enum.GetValues(typeof(TodoItemType)) as TodoItemType[];

    public enum TodoItemType
    {
        Personal,
        Work,
        Hobby,
        Family
    }
}