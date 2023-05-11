using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UraniumUI;
using UraniumUI.Dialogs;

namespace UraniumApp;
public class MainViewModel : UraniumBindableObject
{
    private string status;

    public ICommand UnFocusedCommand { get; set; }
    public ICommand FocusedCommand { get; set; }
    public ICommand CompletedCommand { get; set; }

    public string Status { get => status; set => SetProperty(ref status, value); }

    public MainViewModel()
    {
        UnFocusedCommand = new Command(() =>
        {
            Status = "UnFocused";
            Console.WriteLine(Status);
        });

        FocusedCommand = new Command(() =>
        {
            Status = "Focused";
            Console.WriteLine(Status);
        });

        CompletedCommand = new Command(() =>
        {
            Status = "Completed";
            Console.WriteLine(Status);
        });
    }
}
