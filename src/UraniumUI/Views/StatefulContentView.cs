using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UraniumUI.Views;
public class StatefulContentView : ContentView
{
	public StatefulContentView()
	{
	}

    // TODO: Convert them to BindableProperty
	public ICommand PressedCommand { get; set; }

	public ICommand HoverCommand { get; set; }

	public ICommand HoverExitCommand { get; set; }

	public ICommand LongPressCommand { get; set; }

	public ICommand TappedCommand { get; set; }

	public object CommandParameter { get; set; }
}
