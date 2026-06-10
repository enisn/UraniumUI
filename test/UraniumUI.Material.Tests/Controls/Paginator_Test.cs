using Shouldly;
using System.Linq;
using UraniumUI.Material.Controls;
using UraniumUI.Tests.Core;
using UraniumUI.ViewExtensions;

namespace UraniumUI.Material.Tests.Controls;

public class Paginator_Test
{
    public Paginator_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
    }

    [Fact]
    public void CurrentPage_Change_ShouldReenablePreviousPageButton()
    {
        var control = AnimationReadyHandler.Prepare(new Paginator());

        control.TotalPageCount = 5;
        control.CurrentPage = 1;

        GetPageButtons(control)
            .Single(x => (int)x.CommandParameter == 1)
            .IsEnabled
            .ShouldBeFalse();

        control.CurrentPage = 2;

        var pageButtons = GetPageButtons(control).ToList();

        pageButtons.Single(x => (int)x.CommandParameter == 1).IsEnabled.ShouldBeTrue();
        pageButtons.Single(x => (int)x.CommandParameter == 2).IsEnabled.ShouldBeFalse();
    }

    private static IEnumerable<Button> GetPageButtons(Paginator control)
    {
        return control.FindByViewQueryId<HorizontalStackLayout>("PagesStackLayout")
            .Children
            .OfType<Button>()
            .Where(x => ViewQuery.GetId(x) == "paginator-btn");
    }
}
