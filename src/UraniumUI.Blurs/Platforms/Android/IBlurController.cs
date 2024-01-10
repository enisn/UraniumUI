using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.Blurs;

public interface IBlurController : IBlurViewFacade
{
    /**
  * Draws blurred content on given canvas
  *
  * @return true if BlurView should proceed with drawing itself and its children
  */
    bool Draw(Canvas canvas);

    /**
     * Must be used to notify Controller when BlurView's size has changed
     */
    void UpdateBlurViewSize();

    /**
     * Frees allocated resources
     */
    void Destroy();
}

public static class BlurViewDefaults
{
    public const float SCALE_FACTOR = 6f;
    public const float BLUR_RADIUS = 16f;
}
