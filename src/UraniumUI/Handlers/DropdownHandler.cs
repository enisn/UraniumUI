using Microsoft.Maui.Handlers;
using UraniumUI.Controls;

namespace UraniumUI.Handlers;
public partial class DropdownHandler
{
    public Dropdown VirtualViewDropdown => VirtualView as Dropdown;
    public static IPropertyMapper<Dropdown, DropdownHandler> DropdownPropertyMapper =>
        new PropertyMapper<Dropdown, DropdownHandler>(ButtonHandler.Mapper)
        {
            [nameof(Dropdown.ItemsSource)] = MapItemsSource,
            [nameof(Dropdown.SelectedItem)] = MapSelectedItem,
        };

    public DropdownHandler() : base(DropdownPropertyMapper)
    {
    }
}

#if (NET8_0) && !ANDROID && !IOS && !MACCATALYST && !WINDOWS
public partial class DropdownHandler : ViewHandler<Dropdown, object>
{
	public DropdownHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(DropdownPropertyMapper, commandMapper)
    {

	}

    protected override object CreatePlatformView()
    {
        throw new NotImplementedException();
    }

    public static void MapItemsSource(DropdownHandler handler, Dropdown dropdown)
    {

    }
    public static void MapSelectedItem(DropdownHandler handler, Dropdown dropdown)
    {

    }
}

#endif