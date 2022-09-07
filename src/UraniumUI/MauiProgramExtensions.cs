using InputKit.Handlers;
using UraniumUI.Handlers;

namespace UraniumUI;
public static class MauiProgramExtensions
{
    public static IMauiHandlersCollection AddUraniumUIHandlers(this IMauiHandlersCollection collection)
    {
        return collection.AddInputKitHandlers()
            .AddHandler(typeof(Button),typeof(StatefulButtonHandler))
            ;
    }
}
