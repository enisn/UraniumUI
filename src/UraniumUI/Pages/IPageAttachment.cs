namespace UraniumUI.Pages;

public interface IPageAttachment : IView
{
    void OnAttached(UraniumContentPage attachedPage);

    AttachmentPosition AttachmentPosition { get; }
}

public interface IStickedPageAttachment : IPageAttachment
{
    // TODO: Start, Top, End, Bottom stick options.
}