using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public void FirstPageButtonImage_BindingForInitialization_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new Paginator());
        var viewModel = new { FirstPageButtonImage = new FontImageSource() };
        control.BindingContext = viewModel;
        control.SetBinding(Paginator.FirstPageButtonImageProperty, new Binding(nameof(viewModel.FirstPageButtonImage)));
        control.FirstPageButtonImage.ShouldNotBeNull();
    }

    [Fact]
    public void ButtonTexts_DefaultValues_AreCorrect()
    {
        var control = AnimationReadyHandler.Prepare(new Paginator());
        
        control.FirstPageButtonText.ShouldBe("<<");
        control.PreviousPageButtonText.ShouldBe("<");
        control.NextPageButtonText.ShouldBe(">");
        control.LastPageButtonText.ShouldBe(">>");
    }

    [Fact]
    public void ButtonImages_DefaultValues_AreNull()
    {
        var control = AnimationReadyHandler.Prepare(new Paginator());
        
        control.FirstPageButtonImage.ShouldBeNull();
        control.PreviousPageButtonImage.ShouldBeNull();
        control.NextPageButtonImage.ShouldBeNull();
        control.LastPageButtonImage.ShouldBeNull();
    }

    [Fact]
    public void ButtonTexts_CanBeSet_AndRetrieved()
    {
        var control = AnimationReadyHandler.Prepare(new Paginator());
        
        control.FirstPageButtonText = "First";
        control.PreviousPageButtonText = "Prev";
        control.NextPageButtonText = "Next";
        control.LastPageButtonText = "Last";

        control.FirstPageButtonText.ShouldBe("First");
        control.PreviousPageButtonText.ShouldBe("Prev");
        control.NextPageButtonText.ShouldBe("Next");
        control.LastPageButtonText.ShouldBe("Last");
    }

    [Fact]
    public void ButtonImages_CanBeSet_AndRetrieved()
    {
        var control = AnimationReadyHandler.Prepare(new Paginator());
        var firstImage = new FontImageSource();
        var prevImage = new FontImageSource();
        var nextImage = new FontImageSource();
        var lastImage = new FontImageSource();
        
        control.FirstPageButtonImage = firstImage;
        control.PreviousPageButtonImage = prevImage;
        control.NextPageButtonImage = nextImage;
        control.LastPageButtonImage = lastImage;

        control.FirstPageButtonImage.ShouldBe(firstImage);
        control.PreviousPageButtonImage.ShouldBe(prevImage);
        control.NextPageButtonImage.ShouldBe(nextImage);
        control.LastPageButtonImage.ShouldBe(lastImage);
    }

    [Fact]
    public void CurrentPage_DefaultValue_IsOne()
    {
        var control = AnimationReadyHandler.Prepare(new Paginator());
        control.CurrentPage.ShouldBe(1);
    }

    [Fact]
    public void TotalPageCount_DefaultValue_IsZero()
    {
        var control = AnimationReadyHandler.Prepare(new Paginator());
        control.TotalPageCount.ShouldBe(0);
    }

    [Fact]
    public void PageStepCount_DefaultValue_IsTwo()
    {
        var control = AnimationReadyHandler.Prepare(new Paginator());
        control.PageStepCount.ShouldBe(2);
    }

    [Fact]
    public void CanGoNext_WhenOnLastPage_IsFalse()
    {
        var control = AnimationReadyHandler.Prepare(new Paginator());
        control.TotalPageCount = 5;
        control.CurrentPage = 5;
        
        control.CanGoNext.ShouldBeFalse();
    }

    [Fact]
    public void CanGoPrevious_WhenOnFirstPage_IsFalse()
    {
        var control = AnimationReadyHandler.Prepare(new Paginator());
        control.TotalPageCount = 5;
        control.CurrentPage = 1;
        
        control.CanGoPrevious.ShouldBeFalse();
    }

    [Fact]
    public void CanGoNext_WhenNotOnLastPage_IsTrue()
    {
        var control = AnimationReadyHandler.Prepare(new Paginator());
        control.TotalPageCount = 5;
        control.CurrentPage = 3;
        
        control.CanGoNext.ShouldBeTrue();
    }

    [Fact]
    public void CanGoPrevious_WhenNotOnFirstPage_IsTrue()
    {
        var control = AnimationReadyHandler.Prepare(new Paginator());
        control.TotalPageCount = 5;
        control.CurrentPage = 3;
        
        control.CanGoPrevious.ShouldBeTrue();
    }

    [Fact]
    public void ChangePageCommand_WhenExecuted_UpdatesCurrentPage()
    {
        var control = AnimationReadyHandler.Prepare(new Paginator());
        var commandExecuted = false;
        var newPage = 0;

        control.ChangePageCommand = new Command<int>(page =>
        {
            commandExecuted = true;
            newPage = page;
        });

        control.ChangePageCommand.Execute(3);

        commandExecuted.ShouldBeTrue();
        newPage.ShouldBe(3);
    }

    [Fact]
    public void PageStepCount_WhenChanged_UpdatesVisiblePageNumbers()
    {
        var control = AnimationReadyHandler.Prepare(new Paginator());
        control.TotalPageCount = 10;
        control.CurrentPage = 5;
        control.PageStepCount = 1;

        // Get the page numbers from the stack layout
        var pagesStackLayout = control.FindByViewQueryId<HorizontalStackLayout>("PagesStackLayout");
        var pageNumbers = BindableLayout.GetItemsSource(pagesStackLayout) as IEnumerable<int>;
        
        pageNumbers.ShouldNotBeNull();
        pageNumbers.Count().ShouldBe(3); // Current page (5) and one page on each side
        pageNumbers.ShouldContain(4);
        pageNumbers.ShouldContain(5);
        pageNumbers.ShouldContain(6);
    }
}