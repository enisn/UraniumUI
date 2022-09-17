using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.Tests.Core;
public static class ApplicationExtensions
{
    public static Window LoadPage(this Application app, Page page)
    {
        app.MainPage = page;

        return ((IApplication)app).CreateWindow(null) as Window;
    }

    public static void CreateAndSetMockApplication()
    {
        var appBuilder = MauiApp.CreateBuilder()
                                .UseMauiApp<MockApplication>();
        var mauiApp = appBuilder.Build();
        var application = mauiApp.Services.GetRequiredService<IApplication>();
        application.Handler = new ApplicationHandlerStub();
        application.Handler.SetMauiContext(new HandlersContextStub(mauiApp.Services));
    }
}
