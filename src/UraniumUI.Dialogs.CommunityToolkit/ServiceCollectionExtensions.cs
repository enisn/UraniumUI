using Microsoft.Extensions.DependencyInjection.Extensions;
using UraniumUI.Dialogs;

namespace UraniumUI;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommunityToolkitDialogs(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        services.Replace(new ServiceDescriptor(typeof(IDialogService), typeof(CommunityToolkitDialogService), lifetime));
        return services;
    }
}
