//using CommunityToolkit.Maui;
using CommunityToolkit.Maui;
using DotNurse.Injector;
using MemoryToolkit.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using ReactiveUI;
using System.Reactive;
using UraniumUI;
using UraniumUI.Dialogs;
using UraniumUI.Options;
using UraniumUI.Validations;

namespace UraniumApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseUraniumUI()
            .UseUraniumUIMaterial()
            .UseUraniumUIBlurs(false)
            .UseUraniumUIWebComponents()
            .ConfigureMopups()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFontAwesomeIconFonts();
                fonts.AddMaterialSymbolsFonts();
                fonts.AddFluentIconFonts();
            });

#if DEBUG
        builder.Logging.AddDebug();

        var memoryLeakEvents = new MemoryLeakDetectEvents();
        builder.Services.AddSingleton(memoryLeakEvents);
        builder.UseLeakDetection(onLeaked: memoryLeakEvents.InvokeOnLeaked, memoryLeakEvents.InvokeOnCollected);
#endif
        
        builder.Services.Configure<AutoFormViewOptions>(options =>
        {
            options.ValidationFactory = DataAnnotationValidation.CreateValidations;
        });

        RxApp.DefaultExceptionHandler = new AnonymousObserver<Exception>(ex =>
        {
            App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");

            // Track the exception here... (e.g. AppCenter, Sentry, etc.)
        });

        var thisAssembly = typeof(MauiProgram).Assembly;

        builder.Services.AddServicesFrom(
            type => typeof(Page).IsAssignableFrom(type),
            ServiceLifetime.Transient,
            options => options.Assembly = thisAssembly)
        .AddServicesByAttributes(assembly: thisAssembly);

        builder.Services.AddCommunityToolkitDialogs();
        builder.Services.AddMopupsDialogs();

        return builder.Build();
    }
}

public class MemoryLeakDetectEvents
{
    public event EventHandler<CollectionTarget> OnCollected;
    public event EventHandler<CollectionTarget> OnLeaked;

    internal void InvokeOnCollected(CollectionTarget target) => OnCollected?.Invoke(this, target);
    internal void InvokeOnLeaked(CollectionTarget target) => OnLeaked?.Invoke(this, target);
    
}