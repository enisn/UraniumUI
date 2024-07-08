using Microsoft.Extensions.DependencyInjection.Extensions;
using UraniumUI.Dialogs;
using UraniumUI.Dialogs.CommunityToolkit;

namespace UraniumUI;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommunityToolkitDialogs(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        services.Add(new ServiceDescriptor(typeof(IDialogService), typeof(CommunityToolkitDialogService), lifetime));
        services.Add(new ServiceDescriptor(typeof(CommunityToolkitDialogService), typeof(CommunityToolkitDialogService), lifetime));
        return services;
    }
}
