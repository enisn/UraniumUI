using UraniumUI.Dialogs.Mopups;
using UraniumUI.Material.Controls;

namespace UraniumApp.Pages;

public partial class ChipsPage : ContentPage
{
	public ChipsPage()
	{
		InitializeComponent();
        contentView.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () =>
            {
                var selecteds = chipsHolder.Children.Where(x => x is Chip).Cast<Chip>().Select(s => s.Text).ToArray();
                var result = await this.DisplayCheckBoxPromptAsync("Pick some of them",
                    new[] { "Chip A", "Chip B", "Chip C", "Chip D", "Chip E", "Chip F", "Chip G" },
                    selectedItems: selecteds);

                if (result is null)
                {
                    return;
                }

                foreach (var oldItem in selecteds)
                {
                    if (!result.Any(x => x == oldItem))
                    {
                        chipsHolder.Remove(chipsHolder.Children.FirstOrDefault(x => x is Chip chip && chip.Text == oldItem));
                    }
                }

                foreach (var item in result)
                {
                    if (!selecteds.Contains(item))
                    {
                        chipsHolder.Add(new Chip { Text = item });
                    }
                }
            })
        });
	}

private void Chip_Destroyed(object sender, EventArgs e)
    {
        if (sender is View view && view.Parent is Layout parentLayout)
        {
            parentLayout.Children.Remove(view);
        }
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await this.DisplayCheckBoxPromptAsync("Pick some of them", new[] { "Chip A", "Chip B", "Chip C" });
    }
}