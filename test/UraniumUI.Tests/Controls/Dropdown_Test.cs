using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using UraniumUI.Controls;
using UraniumUI.Tests.Core;
using Xunit;

namespace UraniumUI.Tests.Controls;

public class Dropdown_Test
{
    public Dropdown_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
    }

    [Fact]
    public void RequestDismissPopup_ShouldRaise_Event()
    {
        var control = new Dropdown();
        var invoked = false;

        control.RequestDismissPopupRequested += (_, _) => invoked = true;

        control.RequestDismissPopup();

        Assert.True(invoked);
    }
}
