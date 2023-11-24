using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UraniumUI;

namespace UraniumApp.ViewModels.Blurs;
public class BlursDemoViewModel : UraniumBindableObject
{
    public ObservableCollection<string> Items { get; } = new ObservableCollection<string>
    {
        "https://source.unsplash.com/random/1920x1080?nature",
    };

    public ICommand BackCommand { get; }

    public BlursDemoViewModel()
    {
        for (int i = 0; i < 8; i++)
        {
            AddRandomItem();
        }

        BackCommand = new Command(() =>
        {
            App.Current.MainPage.Navigation.PopAsync();
        });
    }

    private void AddRandomItem()
    {
        Items.Add("https://source.unsplash.com/random/480x320?nature&" + Guid.NewGuid().ToString("N"));
    }
}
