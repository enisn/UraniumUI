using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UraniumUI.Pages;

namespace UraniumUI.Material.Attachments;
public class BackdropView : Frame, IPageAttachment
{
    public AttachmentPosition AttachmentPosition => AttachmentPosition.Behind;

    public void OnAttached(UraniumContentPage attachedPage)
    {
        throw new NotImplementedException();
    }
}
