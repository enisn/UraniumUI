using UraniumUI.Controls;
using UraniumUI.Handlers;

namespace UraniumUI.Tests.Controls;

public class AutoCompleteView_Test
{
    [Fact]
    public void TextColor_ShouldBeRegisteredInPropertyMapper()
    {
        Assert.NotNull(AutoCompleteViewHandler.AutoCompleteViewMapper.GetProperty(nameof(AutoCompleteView.TextColor)));
    }
}
