namespace UraniumUI.Material.Controls;
public partial class TextField
{
#if WINDOWS // Workaround for https://github.com/enisn/UraniumUI/issues/373
    partial void AfterConstructor()
    {
        EntryView.HandlerChanged += (s, e) =>
        {
            if (EntryView.Handler.PlatformView is Microsoft.UI.Xaml.Controls.TextBox view)
            {
                view.LosingFocus += (s, e) =>
                {
                    if (!canUnfocus)
                    {
                        e.TryCancel();
                    }
                };
            }
        };
    }

    protected bool canUnfocus = true;
    CancellationTokenSource unfocusCts = new();

    protected override async void CheckAndShowValidations()
    {
        base.CheckAndShowValidations();

        canUnfocus = false;
        unfocusCts.Cancel();
        unfocusCts = new CancellationTokenSource();
        /* Disable focus for a short time to prevent unfocusing
         * while validation adding to the layout */
        await EnableFocusAfterAsync(100, unfocusCts.Token);
    }

    async Task EnableFocusAfterAsync(int delayMs, CancellationToken cancellationToken)
    {
        await Task.Delay(delayMs);
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        canUnfocus = true;
    }
#endif
}
