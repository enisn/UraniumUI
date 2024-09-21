using System.Collections;
using System.Collections.ObjectModel;
using UraniumUI.Dialogs;

namespace UraniumUI.Material.Controls;
public partial class MultiplePickerField : InputField
{
    public ContentView MainContentView => Content as ContentView;

    private bool isBusy;
    public bool IsBusy
    {
        get => isBusy;
        protected set
        {
            isBusy = value;
            UpdateState();
        }
    }

    public override View Content { get; set; } = new ContentView();

    public override bool HasValue { get => IsBusy || SelectedItems?.Count > 0; }

    protected IDialogService DialogService { get; }

    protected HorizontalStackLayout chipsHolderStackLayout = new HorizontalStackLayout
    {
        HorizontalOptions = LayoutOptions.Start,
#if IOS || MACCATALYST
        VerticalOptions = LayoutOptions.Center,
#endif
    };

    private Command _destroyChipCommand;
    private Command _pickSelectionsCommand;

    public MultiplePickerField()
    {

        MainContentView.Content = new ScrollView
        {
            Orientation = ScrollOrientation.Horizontal,
#if ANDROID
            HorizontalOptions = LayoutOptions.Start,
#endif
#if !IOS && !MACCATALYST
            VerticalOptions = LayoutOptions.Center,
#endif
            Content = chipsHolderStackLayout,
        };

        base.RegisterForEvents();

        DialogService = UraniumServiceProvider.Current.GetRequiredService<IDialogService>();

        _pickSelectionsCommand = new Command(async () =>
        {
            if (SelectedItems is null)
            {
                SelectedItems = new ObservableCollection<object>();
            }

            IsBusy = true;
            var result = await DialogService.DisplayCheckBoxPromptAsync(
                this.Title,
                ItemsSource as IEnumerable<object>,
                SelectedItems as IEnumerable<object>
                );

            if (result != null)
            {
                SelectedItems.Clear();
                foreach (var item in result)
                {
                    SelectedItems.Add(item);
                }

                UpdateState();
            }
            IsBusy = false;
        });

        this.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = _pickSelectionsCommand
        });

        _destroyChipCommand = new Command((param) =>
        {
            if (param is Chip chip)
            {
                SelectedItems.Remove(chip.BindingContext);
                UpdateState();
            }
        });

        BindableLayout.SetItemTemplate(chipsHolderStackLayout, new DataTemplate(() =>
        {
            var chip = new Chip();
            chip.SetBinding(Chip.TextProperty, new Binding("."));
            chip.DestroyCommand = _destroyChipCommand;
            return chip;
        }));
    }

    protected virtual void OnItemsSourceSet()
    {

    }

    protected virtual void OnSelectedItemsSet()
    {
        BindableLayout.SetItemsSource(chipsHolderStackLayout, SelectedItems);
    }

    public IList ItemsSource { get => (IList)GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IList),
        typeof(MultiplePickerField),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as MultiplePickerField).OnItemsSourceSet());

    public IList SelectedItems { get => (IList)GetValue(SelectedItemsProperty); set => SetValue(SelectedItemsProperty, value); }

    public static readonly BindableProperty SelectedItemsProperty = BindableProperty.Create(
        nameof(SelectedItems),
        typeof(IList),
        typeof(MultiplePickerField),
        propertyChanged: (bindable, oldValue, newValue) => (bindable as MultiplePickerField).OnSelectedItemsSet());
}