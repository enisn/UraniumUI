using Android.Graphics;

namespace UraniumUI.Blurs;

public interface IBlurAlgorithm
{
    Bitmap Blur(Bitmap bitmapExportContext, float blurRadius);

    void Destroy();

    bool CanModifyBitmap { get; }

    Bitmap.Config SupportedBitmapConfig { get; }

    float ScaleFactor { get; }

    void Render(Canvas canvas, Bitmap bitmap);
}