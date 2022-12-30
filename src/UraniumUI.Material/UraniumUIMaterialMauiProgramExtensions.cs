using UraniumUI.Material.Controls;
using UraniumUI.Material.Handlers;

namespace UraniumUI;
public static class UraniumUIMaterialMauiProgramExtensions
{
    public static MauiAppBuilder UseUraniumUIMaterial(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler(typeof(ButtonView), typeof(ButtonViewHandler));
            handlers.AddHandler(typeof(AutoCompleteView), typeof(AutoCompleteViewHandler));
        });
        
        return builder;
    }
}
