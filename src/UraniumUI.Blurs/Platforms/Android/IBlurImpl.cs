using Android.Content;
using Android.Graphics;

namespace UraniumUI.Blurs.Platforms.Android;
public interface IBlurImpl
{
    bool Prepare(Context context, Bitmap buffer, float radius);

    void Release();

    void Blur(Bitmap input, Bitmap output);
}

public class EmptyBlurImpl : IBlurImpl
{
    public bool Prepare(Context context, Bitmap buffer, float radius)
    {
        return false;
    }

    public void Release()
    {
    }

    public void Blur(Bitmap input, Bitmap output)
    {
    }
}
