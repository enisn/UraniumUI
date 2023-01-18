using InputKit.Handlers;
using Plainer.Maui;
using UraniumUI.Controls;
using UraniumUI.Dialogs;
using UraniumUI.Handlers;
using UraniumUI.Views;

namespace UraniumUI;
public static class MauiProgramExtensions
{
    public static MauiAppBuilder UseUraniumUI(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddUraniumUIHandlers();
        });

        builder.Services.AddTransient<IDialogService, DefaultDialogService>();
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
        // TODO: Find a better way.
        return app;
    }

    public static IMauiHandlersCollection AddUraniumUIHandlers(this IMauiHandlersCollection collection)
    {
        return collection
            .AddInputKitHandlers()
            .AddHandler(typeof(Button), typeof(StatefulButtonHandler))
            .AddHandler(typeof(StatefulContentView), typeof(StatefulContentViewHandler))
            .AddHandler(typeof(AutoCompleteView), typeof(AutoCompleteViewHandler))
            .AddPlainer();
    }
}
