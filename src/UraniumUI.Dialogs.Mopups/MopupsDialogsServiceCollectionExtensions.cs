using Microsoft.Extensions.DependencyInjection.Extensions;
using UraniumUI.Dialogs;
using UraniumUI.Dialogs.Mopups;

namespace UraniumUI;
public static class MopupsDialogsServiceCollectionExtensions
{
    public static IServiceCollection AddMopupsDialogs(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        services.Add(new ServiceDescriptor(typeof(IDialogService), typeof(MopupsDialogService), lifetime));
        services.Add(new ServiceDescriptor(typeof(MopupsDialogService), typeof(MopupsDialogService), lifetime));
        return services;
    }
}
