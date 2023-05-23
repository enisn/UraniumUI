using Shouldly;
using UraniumUI.Extensions;

namespace UraniumUI.Tests.Extensions;
public class NumericExtensions_Test
{
    [Fact]
    public void Clamp_ShouldReturnSameNumber()
    {
        var number = 15;

        var clamped = number.Clamp(10, 25);

        clamped.ShouldBe(15);
    }

    [Fact]
    public void Clamp_ShouldReturnMin()
    {

        var number = 15;
        var min = 20;

        var clamped = number.Clamp(min, 25);

        clamped.ShouldBe(min);
    }

    [Fact]
    public void Clamp_ShouldReturnMax()
    {

        var number = 15;
        var max = 10;

        var clamped = number.Clamp(0, max);

        clamped.ShouldBe(max);
    }
}
