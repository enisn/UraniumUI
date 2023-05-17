using UraniumUI.WebComponents.Controls;
using UraniumUI.WebComponents.Handlers;

namespace UraniumUI;

public static class MauiProgramExtensions
{
    public static MauiAppBuilder UseUraniumUIWebComponents(this MauiAppBuilder builder, bool useBlurryDialogs = true)
    {

        builder.ConfigureMauiHandlers(options =>
        {
            options.AddUraniumUIWebComponentsHandlers();
        });

        return builder;
    }

    public static IMauiHandlersCollection AddUraniumUIWebComponentsHandlers(this IMauiHandlersCollection handlers)
    {
        return handlers
            .AddHandler(typeof(CodeView), typeof(CodeViewHandler));
    }
}
