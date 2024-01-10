using Android.Graphics;

namespace UraniumUI.Blurs;

public interface IBlurAlgorithm
{
    Bitmap Blur(Bitmap bitmapExportContext, float blurRadius);

    void Destroy();

    bool CanModifyBitmap();  // TODO: Make it property

    Bitmap.Config GetSupportedBitmapConfig();

    float ScaleFactor(); // TODO: Make it property

    void Render(Canvas canvas, Bitmap bitmap);
}