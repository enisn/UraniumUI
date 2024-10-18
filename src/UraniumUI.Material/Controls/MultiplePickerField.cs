using Microsoft.Maui.Layouts;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
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

    public event EventHandler<object> SelectedValuesChanged;

    protected IDialogService DialogService { get; }

    protected FlexLayout chipsHolderLayout;

    private Command _destroyChipCommand;
    private Command _pickSelectionsCommand;

    public MultiplePickerField()
    {
        MainContentView.Content = chipsHolderLayout = CreateLayout();
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
    }

    protected FlexLayout CreateLayout()
    {
        var layout = new FlexLayout
        {
            HorizontalOptions = LayoutOptions.Start,
            AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center,
            AlignContent = Microsoft.Maui.Layouts.FlexAlignContent.Center,
            Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap,
            Margin = new Thickness(4),
#if IOS || MACCATALYST
        VerticalOptions = LayoutOptions.Center,
#endif
        };

        BindableLayout.SetItemTemplate(layout, new DataTemplate(() =>
        {
            var chip = new Chip();
            chip.SetBinding(Chip.TextProperty, new Binding("."));
            chip.SelfDestruct = false;
            chip.DestroyCommand = _destroyChipCommand;
            return chip;
        }));

        BindableLayout.SetItemsSource(layout, SelectedItems);

        return layout;
    }

    protected virtual void OnItemsSourceSet()
    {

    }

    protected virtual void OnSelectedItemsSet(IList oldValue, IList newValue)
    {
        BindableLayout.SetItemsSource(chipsHolderLayout, SelectedItems);
        UpdateState();

        if (oldValue is INotifyCollectionChanged oldObservable)
        {
            oldObservable.CollectionChanged -= SelectedItemsChanged;
        }

        if (newValue is INotifyCollectionChanged observable)
        {
            observable.CollectionChanged -= SelectedItemsChanged;
            observable.CollectionChanged += SelectedItemsChanged;
        }
    }

    private void SelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateState();
        SelectedValuesChangedCommand?.Execute(SelectedItems);
        SelectedValuesChanged?.Invoke(sender, SelectedItems);
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
        propertyChanged: (bindable, oldValue, newValue) => (bindable as MultiplePickerField).OnSelectedItemsSet(oldValue as IList, newValue as IList));

    public ICommand SelectedValuesChangedCommand { get => (ICommand)GetValue(SelectedValuesChangedCommandProperty); set => SetValue(SelectedValuesChangedCommandProperty, value); }

    public static readonly BindableProperty SelectedValuesChangedCommandProperty = BindableProperty.Create(
        nameof(SelectedValuesChangedCommand),
        typeof(ICommand), typeof(MultiplePickerField),
        defaultValue: null);
}