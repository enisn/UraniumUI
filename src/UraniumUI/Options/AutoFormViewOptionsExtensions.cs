using UraniumUI.Controls;

namespace UraniumUI.Options;
public static class AutoFormViewOptionsExtensions
{
    public static MauiAppBuilder ConfigureAutoFormViewDefaults(this MauiAppBuilder builder)
    {
        builder.Services.Configure<AutoFormViewOptions>(options =>
        {
            options.EditorMapping[typeof(string)] = AutoFormView.EditorForString;
            options.EditorMapping[typeof(int)] = AutoFormView.EditorForNumeric;
            options.EditorMapping[typeof(double)] = AutoFormView.EditorForNumeric;
            options.EditorMapping[typeof(float)] = AutoFormView.EditorForNumeric;
            options.EditorMapping[typeof(bool)] = AutoFormView.EditorForBoolean;
            options.EditorMapping[typeof(Keyboard)] = AutoFormView.EditorForKeyboard;
            options.EditorMapping[typeof(Enum)] = AutoFormView.EditorForEnum;
            options.EditorMapping[typeof(DateTime)] = AutoFormView.EditorForDateTime;
            options.EditorMapping[typeof(TimeSpan)] = AutoFormView.EditorForTimeSpan;
        });

        return builder;
    }
}
