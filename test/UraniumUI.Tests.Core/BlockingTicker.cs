using Microsoft.Maui.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.Tests.Core;

public class BlockingTicker : Ticker
{
    bool _enabled;

    public override void Start()
    {
        _enabled = true;

        while (_enabled)
        {
            Fire?.Invoke();
            Task.Delay(16).Wait();
        }
    }

    public override void Stop()
    {
        _enabled = false;
    }
}
