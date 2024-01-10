namespace UraniumUI.Tests.Core;
public static class ApplicationExtensions
{
    public static Window LoadPage(this Application app, Page page)
    {
        app.MainPage = page;

        return ((IApplication)app).CreateWindow(null) as Window;
    }

    public static void CreateAndSetMockApplication(Action<MauiAppBuilder> builder = null)
    {
        var appBuilder = MauiApp.CreateBuilder()
                                .UseMauiApp<MockApplication>();

        builder?.Invoke(appBuilder);

        var mauiApp = appBuilder.Build();
        var application = mauiApp.Services.GetRequiredService<IApplication>();

        Application.Current = application as Application;

        application.Handler = new ApplicationHandlerStub();
        application.Handler.SetMauiContext(new HandlersContextStub(mauiApp.Services));
    }
}
