using Android.Content;
using Android.Graphics;

namespace UraniumUI.Blurs;

public class RenderEffectBlur : IBlurAlgorithm
{
    private readonly RenderNode node = new RenderNode("BlurViewNode");

    private int height, width;
    private float lastBlurRadius = 1f;

    public IBlurAlgorithm fallbackAlgorithm;
    private Context context;

    public Bitmap Blur(Bitmap bitmap, float blurRadius)
    {
        lastBlurRadius = blurRadius;

        if (bitmap.Height != height || bitmap.Width != width)
        {
            height = bitmap.Height;
            width = bitmap.Width;
            node.SetPosition(0, 0, width, height);
        }
        Canvas canvas = node.BeginRecording();
        canvas.DrawBitmap(bitmap, 0, 0, null);
        node.EndRecording();
        node.SetRenderEffect(RenderEffect.CreateBlurEffect(blurRadius, blurRadius, Shader.TileMode.Mirror));
        // returning not blurred bitmap, because the rendering relies on the RenderNode
        return bitmap;
    }

    public void Destroy()
    {
        node.DiscardDisplayList();
        if (fallbackAlgorithm != null)
        {
            fallbackAlgorithm.Destroy();
        }
    }

    public bool CanModifyBitmap => true;

    public Bitmap.Config SupportedBitmapConfig => Bitmap.Config.Argb8888;

    public float ScaleFactor => BlurViewDefaults.SCALE_FACTOR;

    public void Render(Canvas canvas, Bitmap bitmap)
    {
        if (canvas.IsHardwareAccelerated)
        {
            canvas.DrawRenderNode(node);
        }
        else
        {
            if (fallbackAlgorithm == null)
            {
                fallbackAlgorithm = new RenderScriptBlur(context);
            }
            fallbackAlgorithm.Blur(bitmap, lastBlurRadius);
            fallbackAlgorithm.Render(canvas, bitmap);
        }
    }

    public void SetContext(Context context)
    {
        this.context = context;
    }
}
