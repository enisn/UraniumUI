using Plainer.Maui.Controls;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Material.Controls;
using UraniumUI.Tests.Core;

namespace UraniumUI.Material.Tests.Controls;
public class TimePickerField_Test
{
    public TimePickerField_Test()
    {
        ApplicationExtensions.CreateAndSetMockApplication();
    }

    [Fact]
    public void Time_BindingForInitializtion_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new TimePickerField());
        var viewModel = new TestViewModel { Time = TimeSpan.FromHours(2) };
        control.BindingContext = viewModel;
        control.SetBinding(TimePickerField.TimeProperty, new Binding(nameof(TestViewModel.Time)));

        // Assert
        control.Time.ShouldBe(viewModel.Time);
    }

    [Fact]
    public void Time_Binding_FromSource()
    {
        var control = AnimationReadyHandler.Prepare(new TimePickerField());
        var viewModel = new TestViewModel { Time = TimeSpan.FromHours(2) };
        control.BindingContext = viewModel;
        control.SetBinding(TimePickerField.TimeProperty, new Binding(nameof(TestViewModel.Time)));

        // Act
        viewModel.Time = TimeSpan.Parse("09:05");

        // Assert
        control.Time.ShouldBe(viewModel.Time);
    }

    [Fact]
    public void Time_Binding_ToSource()
    {
        var control = AnimationReadyHandler.Prepare(new TimePickerField());
        var viewModel = new TestViewModel { Time = TimeSpan.FromHours(2) };
        control.BindingContext = viewModel;
        control.SetBinding(TimePickerField.TimeProperty, new Binding(nameof(TestViewModel.Time)));

        // Act
        control.Time = TimeSpan.Parse("09:05");

        // Assert
        viewModel.Time.ShouldBe(control.Time);
    }

    public class TestViewModel : UraniumBindableObject
    {
        private TimeSpan time;

        public TimeSpan Time { get => time; set => SetProperty(ref time, value); }
    }
}
