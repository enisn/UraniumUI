using UraniumUI.Blurs;

namespace UraniumUI;

public static class MauiProgramExtensions
{
    public static MauiAppBuilder UseUraniumUIBlurs(this MauiAppBuilder builder)
    {
        builder.ConfigureEffects(effects =>
        {
            effects.AddUraniumUIBlurEffects();
        });

        return builder;
    }

    public static IEffectsBuilder AddUraniumUIBlurEffects(this IEffectsBuilder builder)
    {
        return builder
#if WINDOWS || IOS || MACCATALYST ||ANDROID
            .Add<BlurEffect, BlurPlatformEffect>()
#endif
            ;
    }
}