using InputKit.Handlers;

namespace UraniumUI.Handlers;
public static class UraniumUIHandlersCollectionExtension
{
    public static IMauiHandlersCollection AddUraniumUIHandlers(this IMauiHandlersCollection collection)
    {
        return collection.AddInputKitHandlers();
    }
}
