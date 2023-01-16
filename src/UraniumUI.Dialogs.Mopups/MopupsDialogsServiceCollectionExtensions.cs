using Microsoft.Extensions.DependencyInjection.Extensions;
using UraniumUI.Dialogs;

namespace UraniumUI;
public static class MopupsDialogsServiceCollectionExtensions
{
    public static IServiceCollection AddMopupsDialogs(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        services.Replace(new ServiceDescriptor(typeof(IDialogService), typeof(MopupsDialogService), lifetime));
        return services;
    }
}
