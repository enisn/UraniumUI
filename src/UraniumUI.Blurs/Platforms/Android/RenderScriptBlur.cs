using Android.OS;
using Android.Renderscripts;
using Android.Graphics;
using Android.Content;
using Paint = Android.Graphics.Paint;
using Element = Android.Renderscripts.Element;
using AndroidX.Annotations;

namespace UraniumUI.Blurs;

public class RenderScriptBlur : IBlurAlgorithm
{
    private readonly Paint paint = new Paint(PaintFlags.FilterBitmap);
    private readonly RenderScript renderScript;
    private readonly ScriptIntrinsicBlur blurScript;
    private Allocation outAllocation;

    private int lastBitmapWidth = -1;
    private int lastBitmapHeight = -1;

    public RenderScriptBlur(Context context)
    {
        renderScript = RenderScript.Create(context);
        blurScript = ScriptIntrinsicBlur.Create(renderScript, Element.U8_4(renderScript));
    }

    private bool canReuseAllocation(Bitmap bitmap)
    {
        return bitmap.Height == lastBitmapHeight && bitmap.Width == lastBitmapWidth;
    }

    /**
     * @param bitmap     bitmap to blur
     * @param blurRadius blur radius (1..25)
     * @return blurred bitmap
     */

    [RequiresApi(Value = (int)BuildVersionCodes.JellyBeanMr1)]
    public Bitmap Blur(Bitmap bitmap, float blurRadius)
    {
        //Allocation will use the same backing array of pixels as bitmap if created with USAGE_SHARED flag
        Allocation inAllocation = Allocation.CreateFromBitmap(renderScript, bitmap);

        if (!canReuseAllocation(bitmap))
        {
            if (outAllocation != null)
            {
                outAllocation.Destroy();
            }
            outAllocation = Allocation.CreateTyped(renderScript, inAllocation.Type);
            lastBitmapWidth = bitmap.Width;
            lastBitmapHeight = bitmap.Height;
        }

        blurScript.SetRadius(blurRadius);
        blurScript.SetInput(inAllocation);
        //do not use inAllocation in forEach. it will cause visual artifacts on blurred Bitmap
        blurScript.ForEach(outAllocation);
        outAllocation.CopyTo(bitmap);

        inAllocation.Destroy();
        return bitmap;
    }

    public void Destroy()
    {
        blurScript.Destroy();
        renderScript.Destroy();
        if (outAllocation != null)
        {
            outAllocation.Destroy();
        }
    }

    public bool CanModifyBitmap => true;

    public Bitmap.Config SupportedBitmapConfig => Bitmap.Config.Argb8888;

    public float ScaleFactor => BlurViewDefaults.SCALE_FACTOR;

    public void Render(Canvas canvas, Bitmap bitmap)
    {
        canvas.DrawBitmap(bitmap, 0f, 0f, paint);
    }
}
