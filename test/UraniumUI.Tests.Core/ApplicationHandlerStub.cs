using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.Tests.Core;

public class ApplicationHandlerStub : ElementHandler<IApplication, object>
{
    public static IPropertyMapper<IApplication, ApplicationHandlerStub> Mapper = new PropertyMapper<IApplication, ApplicationHandlerStub>(ElementMapper)
    {
    };
    public ApplicationHandlerStub()
        : base(Mapper)
    {
    }


    protected override object CreatePlatformElement() => default;
}