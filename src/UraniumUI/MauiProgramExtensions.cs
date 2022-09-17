using InputKit.Handlers;
using Microsoft.Maui.Platform;
using Plainer.Maui;
using UraniumUI.Handlers;
using UraniumUI.Resources;

namespace UraniumUI;
public static class MauiProgramExtensions
{
    public static MauiAppBuilder UseUraniumUI(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddUraniumUIHandlers();
        });
        return builder;
    }

    public static Application InitializeUraniumUIResources(this Application app)
    {
        app.LoadThemeResources(app.RequestedTheme.ToString());
        app.RequestedThemeChanged += (s, e) =>
        {
            app.LoadThemeResources(e.RequestedTheme.ToString());            
        };

        return app;
    }

    public static Application LoadThemeResources(this Application app, string themeName)
    {
        app.Resources.TryGetValue(themeName, out var resource);
        var themeResource = resource as ResourceDictionary;

        foreach (var item in themeResource)
        {
            app.Resources[item.Key] = item.Value;
        }

        app.Resources["Primary08"] = ColorResource.GetColor("Primary").WithAlpha(0.08f);
        app.Resources["Primary12"] = ColorResource.GetColor("Primary").WithAlpha(0.12f);
        app.Resources["Primary20"] = ColorResource.GetColor("Primary").WithAlpha(0.20f);
        
        app.Resources["OnPrimary08"] = ColorResource.GetColor("OnPrimary").WithAlpha(0.08f);
        app.Resources["OnPrimary12"] = ColorResource.GetColor("OnPrimary").WithAlpha(0.12f);
        app.Resources["OnPrimary20"] = ColorResource.GetColor("OnPrimary").WithAlpha(0.20f);

        app.Resources["OnSecondary08"] = ColorResource.GetColor("OnSecondary").WithAlpha(0.08f);

        app.Resources["Outline20"] = ColorResource.GetColor("Outline").WithAlpha(0.20f);

        return app;
    }

    public static IMauiHandlersCollection AddUraniumUIHandlers(this IMauiHandlersCollection collection)
    {
        return collection
            .AddInputKitHandlers()
            .AddHandler(typeof(Button), typeof(StatefulButtonHandler))
            .AddPlainer()
            ;
    }
}
