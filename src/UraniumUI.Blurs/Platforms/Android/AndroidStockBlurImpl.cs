using Android.Content;
using Android.Graphics;
using Android.Renderscripts;
using Element = Android.Renderscripts.Element;

namespace UraniumUI.Blurs.Platforms.Android;

public class AndroidStockBlurImpl : IBlurImpl
{

    private RenderScript _mRenderScript;

    private ScriptIntrinsicBlur _mBlurScript;

    private Allocation _mBlurInput;

    private Allocation _mBlurOutput;

    public bool Prepare(Context context, Bitmap buffer, float radius)
    {
        if (_mRenderScript == null)
        {
            try
            {
                _mRenderScript = RenderScript.Create(context);
                _mBlurScript = ScriptIntrinsicBlur.Create(_mRenderScript, Element.U8_4(_mRenderScript));
            }
            catch (RSRuntimeException)
            {
                #if DEBUG
                    throw;
                #else
                    // In release mode, just ignore
                    Release();
                    return false;
                #endif
            }
        }

        _mBlurScript.SetRadius(radius);

        _mBlurInput = Allocation.CreateFromBitmap(
            _mRenderScript,
            buffer,
            Allocation.MipmapControl.MipmapNone,
            AllocationUsage.Script);
        _mBlurOutput = Allocation.CreateTyped(_mRenderScript, _mBlurInput.Type);

        return true;
    }

    public void Release()
    {
        if (!_mBlurInput.IsNullOrDisposed())
        {
            _mBlurInput.Destroy();
            _mBlurInput = null;
        }

        if (!_mBlurOutput.IsNullOrDisposed())
        {
            _mBlurOutput.Destroy();
            _mBlurOutput = null;
        }

        if (!_mBlurScript.IsNullOrDisposed())
        {
            _mBlurScript.Destroy();
            _mBlurScript = null;
        }

        if (!_mRenderScript.IsNullOrDisposed())
        {
            _mRenderScript.Destroy();
            _mRenderScript = null;
        }
    }

    public void Blur(Bitmap input, Bitmap output)
    {
        _mBlurInput.CopyFrom(input);
        _mBlurScript.SetInput(_mBlurInput);
        _mBlurScript.ForEach(_mBlurOutput);
        _mBlurOutput.CopyTo(output);
    }
}
