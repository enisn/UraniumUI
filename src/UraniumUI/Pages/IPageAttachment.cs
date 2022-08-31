using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.Pages;

public interface IPageAttachment : IView
{
    void OnAttached(UraniumContentPage attachedPage);
}

public interface IStickedPageAttachment : IPageAttachment
{
    // TODO: Start, Top, End, Bottom stick options.
}
