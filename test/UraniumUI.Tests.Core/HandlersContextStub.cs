using Microsoft.Maui.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.Tests.Core;

public class HandlersContextStub : IMauiContext
{
    public HandlersContextStub(IServiceProvider services)
    {
        Services = services;
        Handlers = Services.GetRequiredService<IMauiHandlersFactory>();
        AnimationManager = Services.GetRequiredService<IAnimationManager>();
    }

    public IServiceProvider Services { get; }

    public IMauiHandlersFactory Handlers { get; }

    public IAnimationManager AnimationManager { get; }
}
