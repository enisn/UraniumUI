using Microsoft.Maui.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.Tests.Core;

public class TestAnimationManager : IAnimationManager
{
    readonly List<Microsoft.Maui.Animations.Animation> _animations = new();

    public TestAnimationManager(ITicker ticker = null)
    {
        Ticker = ticker ?? new BlockingTicker();
        Ticker.Fire = OnFire;
    }

    public double SpeedModifier { get; set; } = 1;

    public bool AutoStartTicker { get; set; } = false;

    public ITicker Ticker { get; }

    public void Add(Microsoft.Maui.Animations.Animation animation)
    {
        _animations.Add(animation);
        if (AutoStartTicker && !Ticker.IsRunning)
            Ticker.Start();
    }

    public void Remove(Microsoft.Maui.Animations.Animation animation)
    {
        _animations.Remove(animation);
        if (!_animations.Any())
            Ticker.Stop();
    }

    void OnFire()
    {
        var animations = _animations.ToList();
        animations.ForEach(animationTick);

        if (!_animations.Any())
            Ticker.Stop();

        void animationTick(Microsoft.Maui.Animations.Animation animation)
        {
            if (animation.HasFinished)
            {
                _animations.Remove(animation);
                animation.RemoveFromParent();
                return;
            }

            animation.Tick(16);
            if (animation.HasFinished)
            {
                _animations.Remove(animation);
                animation.RemoveFromParent();
            }
        }
    }
}

