using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Java.Lang;
using System;
using Color = Android.Graphics.Color;
using Exception = Java.Lang.Exception;
using Math = Java.Lang.Math;
using Paint = Android.Graphics.Paint;
using Rect = Android.Graphics.Rect;
using RectF = Android.Graphics.RectF;
using View = Android.Views.View;

namespace UraniumUI.Blurs.Platforms.Android;
public class RealtimeBlurView : View
{
    const int BlurProcessingDelayMilliseconds = 200;
    const int BlurAutoUpdateDelayMilliseconds = 200;
    const bool ThrowStopExceptionOnDraw = false;

    private float mDownsampleFactor; // default 4

    private int mOverlayColor; // default #aaffffff

    private float mBlurRadius; // default 10dp (0 < r <= 25)

    private float mCornerRadius; // default 0

    private readonly IBlurImpl mBlurImpl;

    private readonly string _formsId;

    private bool mDirty;

    private Bitmap mBitmapToBlur, mBlurredBitmap;

    private Canvas mBlurringCanvas;

    private bool mIsRendering;

    private Paint mPaint;

    private readonly Rect mRectSrc = new Rect(), mRectDst = new Rect();

    // mDecorView should be the root view of the activity (even if you are on a different window like a dialog)
    // private View mDecorView;

    private JniWeakReference<View> _weakDecorView;

    // If the view is on different root view (usually means we are on a PopupWindow),
    // we need to manually call invalidate() in onPreDraw(), otherwise we will not be able to see the changes
    private bool mDifferentRoot;

    private bool _isContainerShown;

    private bool _autoUpdate;

    private static int RENDERING_COUNT;

    private static int BLUR_IMPL;

    public RealtimeBlurView(Context context, string formsId)
        : base(context)
    {
        mBlurImpl = GetBlurImpl(); // provide your own by override getBlurImpl()
        mPaint = new Paint();

        _formsId = formsId;
        _isContainerShown = true;
        _autoUpdate = true;

        preDrawListener = new PreDrawListener(this);

        //RealtimeBlurViewInstanceCount++;
        //InternalLogger.Debug("RealtimeBlurView", $"Constructor => Active instances: {RealtimeBlurViewInstanceCount}");
    }

    public RealtimeBlurView(nint javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
    }

    //protected override void JavaFinalize()
    //{
    //    base.JavaFinalize();

    //    RealtimeBlurViewInstanceCount--;
    //    InternalLogger.Debug("RealtimeBlurView", $"JavaFinalize() => Active instances: {RealtimeBlurViewInstanceCount}");
    //}

    protected IBlurImpl GetBlurImpl()
    {
        try
        {
            var impl = new AndroidStockBlurImpl();
            var bmp = Bitmap.CreateBitmap(4, 4, Bitmap.Config.Argb8888);
            impl.Prepare(Context, bmp, 4);
            impl.Release();
            bmp.Recycle();
            BLUR_IMPL = 3;
        }
        catch
        {
        }

        if (BLUR_IMPL == 0)
        {
            // fallback to empty impl, which doesn't have blur effect
            BLUR_IMPL = -1;
        }

        switch (BLUR_IMPL)
        {
            case 3:
                return new AndroidStockBlurImpl();
            default:
                return new EmptyBlurImpl();
        }
    }

    public void SetCornerRadius(float radius)
    {
        if (mCornerRadius != radius)
        {
            mCornerRadius = radius;
            mDirty = true;
            Invalidate();
        }
    }

    public void SetDownsampleFactor(float factor)
    {
        if (factor <= 0)
        {
            throw new ArgumentException("Downsample factor must be greater than 0.");
        }

        if (mDownsampleFactor != factor)
        {
            mDownsampleFactor = factor;
            mDirty = true; // may also change blur radius
            ReleaseBitmap();
            Invalidate();
        }
    }

    private void SubscribeToPreDraw(View decorView)
    {
        if (decorView.IsNullOrDisposed() || decorView.ViewTreeObserver.IsNullOrDisposed())
        {
            return;
        }

        decorView.ViewTreeObserver.AddOnPreDrawListener(preDrawListener);
    }

    private void UnsubscribeToPreDraw(View decorView)
    {
        if (decorView.IsNullOrDisposed() || decorView.ViewTreeObserver.IsNullOrDisposed())
        {
            return;
        }

        decorView.ViewTreeObserver.RemoveOnPreDrawListener(preDrawListener);
    }

    public void Destroy()
    {
        if (_weakDecorView != null && _weakDecorView.TryGetTarget(out var mDecorView))
        {
            UnsubscribeToPreDraw(mDecorView);
        }

        Release();
        _weakDecorView = null;
    }

    public void Release()
    {
        SetRootView(null);
        ReleaseBitmap();

        mBlurImpl?.Release();
    }

    public void SetBlurRadius(float radius, bool invalidate = true)
    {
        if (mBlurRadius != radius)
        {
            mBlurRadius = radius;
            mDirty = true;
            if (invalidate)
            {
                Invalidate();
            }
        }
    }

    public void SetOverlayColor(int color, bool invalidate = true)
    {
        if (mOverlayColor != color)
        {
            mOverlayColor = color;
            if (invalidate)
            {
                Invalidate();
            }
        }
    }

    public void SetRootView(View rootView)
    {
        var mDecorView = GetRootView();
        if (mDecorView != rootView)
        {
            UnsubscribeToPreDraw(mDecorView);

            _weakDecorView = new JniWeakReference<View>(rootView);

            if (IsAttachedToWindow)
            {
                OnAttached(rootView);
            }
        }
    }

    private View GetRootView()
    {
        View mDecorView = null;
        _weakDecorView?.TryGetTarget(out mDecorView);
        return mDecorView;
    }

    private void OnAttached(View mDecorView)
    {
        if (mDecorView != null)
        {
            using var handler = new Handler();
            handler.PostDelayed(
                () =>
                {
                    SubscribeToPreDraw(mDecorView);
                    mDifferentRoot = mDecorView.RootView != RootView;
                    if (mDifferentRoot)
                    {
                        mDecorView.PostInvalidate();
                    }
                },
                BlurProcessingDelayMilliseconds);
        }
        else
        {
            mDifferentRoot = false;
        }
    }

    protected override void OnVisibilityChanged(View changedView, [GeneratedEnum] ViewStates visibility)
    {
        base.OnVisibilityChanged(changedView, visibility);

        if (changedView.GetType().Name == "PageContainer")
        {
            _isContainerShown = visibility == ViewStates.Visible;
            SetAutoUpdate(_isContainerShown);
        }
    }

    private void SetAutoUpdate(bool autoUpdate)
    {
        if (autoUpdate)
        {
            EnableAutoUpdate();
            return;
        }

        DisableAutoUpdate();
    }

    private void EnableAutoUpdate()
    {
        if (_autoUpdate)
        {
            return;
        }

        _autoUpdate = true;
        using var handler = new Handler();
        handler.PostDelayed(
            () =>
            {
                var mDecorView = GetRootView();
                if (mDecorView == null || !_autoUpdate)
                {
                    return;
                }

                SubscribeToPreDraw(mDecorView);
            },
            BlurAutoUpdateDelayMilliseconds);
    }

    private void DisableAutoUpdate()
    {
        if (!_autoUpdate)
        {
            return;
        }

        _autoUpdate = false;
        var mDecorView = GetRootView();
        if (mDecorView == null)
        {
            return;
        }

        UnsubscribeToPreDraw(mDecorView);
    }

    private void ReleaseBitmap()
    {
        if (!mBitmapToBlur.IsNullOrDisposed())
        {
            mBitmapToBlur.Recycle();
            mBitmapToBlur = null;
        }

        if (!mBlurredBitmap.IsNullOrDisposed())
        {
            mBlurredBitmap.Recycle();
            mBlurredBitmap = null;
        }
    }

    protected bool Prepare()
    {
        if (mBlurRadius == 0)
        {
            Release();
            return false;
        }

        var downsampleFactor = mDownsampleFactor;
        var radius = mBlurRadius / downsampleFactor;
        if (radius > 25)
        {
            downsampleFactor = downsampleFactor * radius / 25;
            radius = 25;
        }

        var width = Width;
        var height = Height;

        var scaledWidth = Math.Max(1, (int)(width / downsampleFactor));
        var scaledHeight = Math.Max(1, (int)(height / downsampleFactor));

        var dirty = mDirty;

        if (mBlurringCanvas == null
            || mBlurredBitmap == null
            || mBlurredBitmap.Width != scaledWidth
            || mBlurredBitmap.Height != scaledHeight)
        {
            dirty = true;
            ReleaseBitmap();

            var r = false;
            try
            {
                mBitmapToBlur = Bitmap.CreateBitmap(scaledWidth, scaledHeight, Bitmap.Config.Argb8888);
                if (mBitmapToBlur == null)
                {
                    return false;
                }

                mBlurringCanvas = new Canvas(mBitmapToBlur);

                mBlurredBitmap = Bitmap.CreateBitmap(scaledWidth, scaledHeight, Bitmap.Config.Argb8888);
                if (mBlurredBitmap == null)
                {
                    return false;
                }

                r = true;
            }
            catch (OutOfMemoryError e)
            {
                // Bitmap.createBitmap() may cause OOM error
                // Simply ignore and fallback
                //InternalLogger.Warn($"OutOfMemoryError occured while trying to render the blur view: {e.Message}");
            }
            finally
            {
                if (!r)
                {
                    Release();
                }
            }

            if (!r)
            {
                return false;
            }
        }

        if (dirty)
        {
            //InternalLogger.Debug($"BlurView@{GetHashCode()}", $"Prepare() => dirty: mBlurImpl.Prepare()");
            if (mBlurImpl.Prepare(Context, mBitmapToBlur, radius))
            {
                mDirty = false;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    protected void Blur(Bitmap bitmapToBlur, Bitmap blurredBitmap)
    {
        mBlurImpl.Blur(bitmapToBlur, blurredBitmap);
    }

    private readonly PreDrawListener preDrawListener;

    private class PreDrawListener : Java.Lang.Object, ViewTreeObserver.IOnPreDrawListener
    {
        private readonly JniWeakReference<RealtimeBlurView> _weakBlurView;

        public PreDrawListener(RealtimeBlurView blurView)
        {
            _weakBlurView = new JniWeakReference<RealtimeBlurView>(blurView);
        }

        public PreDrawListener(nint handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
        }

        public bool OnPreDraw()
        {
            if (!_weakBlurView.TryGetTarget(out var blurView))
            {
                return false;
            }

            if (!blurView._isContainerShown)
            {
                return false;
            }

            var mDecorView = blurView.GetRootView();

            //InternalLogger.Debug($"BlurView@{blurView.GetHashCode()}", $"OnPreDraw()");

            var locations = new int[2];
            var oldBmp = blurView.mBlurredBitmap;
            var decor = mDecorView;
            if (!decor.IsNullOrDisposed() && blurView.IsShown && blurView.Prepare())
            {
                //InternalLogger.Debug($"BlurView@{blurView.GetHashCode()}", $"OnPreDraw(formsId: {blurView._formsId}) => calling draw on decor");
                var redrawBitmap = blurView.mBlurredBitmap != oldBmp;
                oldBmp = null;
                decor.GetLocationOnScreen(locations);
                var x = -locations[0];
                var y = -locations[1];

                blurView.GetLocationOnScreen(locations);
                x += locations[0];
                y += locations[1];

                // just erase transparent
                blurView.mBitmapToBlur.EraseColor(blurView.mOverlayColor & 0xffffff);

                var rc = blurView.mBlurringCanvas.Save();
                blurView.mIsRendering = true;
                RENDERING_COUNT++;
                try
                {
                    blurView.mBlurringCanvas.Scale(
                        1f * blurView.mBitmapToBlur.Width / blurView.Width,
                        1f * blurView.mBitmapToBlur.Height / blurView.Height);
                    blurView.mBlurringCanvas.Translate(-x, -y);
                    if (decor.Background != null)
                    {
                        decor.Background.Draw(blurView.mBlurringCanvas);
                    }

                    decor.Draw(blurView.mBlurringCanvas);
                }
                catch (StopException)
                {
                    //InternalLogger.Debug($"BlurView@{blurView.GetHashCode()}", $"OnPreDraw(formsId: {blurView._formsId}) => in catch StopException");
                }
                catch (Exception)
                {
                    //InternalLogger.Debug($"BlurView@{blurView.GetHashCode()}", $"OnPreDraw(formsId: {blurView._formsId}) => in catch global exception");
                }
                finally
                {
                    blurView.mIsRendering = false;
                    RENDERING_COUNT--;
                    blurView.mBlurringCanvas.RestoreToCount(rc);
                }

                //InternalLogger.Debug($"BlurView@{blurView.GetHashCode()}", $"OnPreDraw(formsId: {blurView._formsId}) => blurView.Blur()");
                blurView.Blur(blurView.mBitmapToBlur, blurView.mBlurredBitmap);

                if (redrawBitmap || blurView.mDifferentRoot)
                {
                    //InternalLogger.Debug(
                    //	$"BlurView@{blurView.GetHashCode()}",
                    //	$"OnPreDraw(formsId: {blurView._formsId}, redrawBitmap: {redrawBitmap}, differentRoot: {blurView.mDifferentRoot}) => blurView.Invalidate()");
                    blurView.Invalidate();
                }
            }

            return true;
        }
    }

    protected View GetActivityDecorView()
    {
        var ctx = Context;
        for (var i = 0; i < 4 && ctx != null && !(ctx is Activity) && ctx is ContextWrapper; i++)
        {
            ctx = ((ContextWrapper)ctx).BaseContext;
        }

        if (ctx is Activity)
        {
            return ((Activity)ctx).Window.DecorView;
        }
        else
        {
            return null;
        }
    }

    protected override void OnAttachedToWindow()
    {
        //InternalLogger.Debug($"BlurView@{GetHashCode()}", $"OnAttachedToWindow()");
        base.OnAttachedToWindow();

        var mDecorView = GetRootView();
        if (mDecorView == null)
        {
            SetRootView(GetActivityDecorView());
        }
        else
        {
            OnAttached(mDecorView);
        }
    }

    protected override void OnDetachedFromWindow()
    {
        var mDecorView = GetRootView();
        if (mDecorView != null)
        {
            UnsubscribeToPreDraw(mDecorView);
        }

        //InternalLogger.Debug($"BlurView@{GetHashCode()}", $"OnDetachedFromWindow()");
        Release();
        base.OnDetachedFromWindow();
    }

    public override void Draw(Canvas canvas)
    {
        if (mIsRendering)
        {
            //InternalLogger.Debug($"BlurView@{GetHashCode()}", $"Draw() => throwing stop exception");

            // Quit here, don't draw views above me
            if (ThrowStopExceptionOnDraw)
            {
                throw new StopException();
            }

            return;
        }

        if (RENDERING_COUNT > 0)
        {
            //InternalLogger.Debug($"BlurView@{GetHashCode()}", $"Draw() => Doesn't support blurview overlap on another blurview");

            // Doesn't support blurview overlap on another blurview
        }
        else
        {
            //InternalLogger.Debug($"BlurView@{GetHashCode()}", $"Draw() => calling base draw");
            base.Draw(canvas);
        }
    }

    protected override void OnDraw(Canvas canvas)
    {
        base.OnDraw(canvas);

        //InternalLogger.Debug($"BlurView@{GetHashCode()}", $"OnDraw(formsId: {_formsId})");
        DrawRoundedBlurredBitmap(canvas, mBlurredBitmap, mOverlayColor);

        // DrawBlurredBitmap(canvas, mBlurredBitmap, mOverlayColor);
    }

    /**
	 * Custom draw the blurred bitmap and color to define your own shape
	 *
	 * @param canvas
	 * @param blurredBitmap
	 * @param overlayColor
	 */
    protected void DrawBlurredBitmap(Canvas canvas, Bitmap blurredBitmap, int overlayColor)
    {
        if (blurredBitmap != null)
        {
            mRectSrc.Right = blurredBitmap.Width;
            mRectSrc.Bottom = blurredBitmap.Height;
            mRectDst.Right = Width;
            mRectDst.Bottom = Height;
            canvas.DrawBitmap(blurredBitmap, mRectSrc, mRectDst, null);
        }

        mPaint.Color = new Color(overlayColor);
        canvas.DrawRect(mRectDst, mPaint);
    }

    private void DrawRoundedBlurredBitmap(Canvas canvas, Bitmap blurredBitmap, int overlayColor)
    {
        if (blurredBitmap != null)
        {
            //InternalLogger.Debug(
            //	$"BlurView@{GetHashCode()}", $"DrawRoundedBlurredBitmap( mCornerRadius: {mCornerRadius}, mOverlayColor: {mOverlayColor} )");

            var mRectF = new RectF { Right = Width, Bottom = Height };

            mPaint.Reset();
            mPaint.AntiAlias = true;
            var shader = new BitmapShader(blurredBitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp);
            var matrix = new Matrix();
            matrix.PostScale(mRectF.Width() / blurredBitmap.Width, mRectF.Height() / blurredBitmap.Height);
            shader.SetLocalMatrix(matrix);
            mPaint.SetShader(shader);
            canvas.DrawRoundRect(mRectF, mCornerRadius, mCornerRadius, mPaint);

            mPaint.Reset();
            mPaint.AntiAlias = true;
            mPaint.Color = new Color(overlayColor);
            canvas.DrawRoundRect(mRectF, mCornerRadius, mCornerRadius, mPaint);
        }
    }

    private class StopException : Exception
    {
    }
}
