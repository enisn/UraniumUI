using UraniumUI.Blurs;
using UraniumUI.Dialogs;

namespace UraniumUI;

public static class MauiProgramExtensions
{
    public static MauiAppBuilder UseUraniumUIBlurs(this MauiAppBuilder builder, bool useBlurryDialogs = true)
    {
        if (useBlurryDialogs)
        {
            builder.Services.Configure<DialogOptions>(options =>
            {
                options.Effects.Add(() => new BlurEffect());
            });
        }

        builder.ConfigureEffects(effects =>
        {
            effects.AddUraniumUIBlurEffects();
        });

        return builder;
    }

    public static IEffectsBuilder AddUraniumUIBlurEffects(this IEffectsBuilder builder)
    {
        return builder
#if WINDOWS || IOS || MACCATALYST || ANDROID
            .Add<BlurEffect, BlurPlatformEffect>()
#endif
            ;
    }
}