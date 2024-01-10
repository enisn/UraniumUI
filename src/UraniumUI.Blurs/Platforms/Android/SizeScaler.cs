using Java.Lang;

namespace UraniumUI.Blurs;

public class SizeScaler
{
    // Bitmap size should be divisible by ROUNDING_VALUE to meet stride requirement.
    // This will help avoiding an extra bitmap allocation when passing the bitmap to RenderScript for blur.
    // Usually it's 16, but on Samsung devices it's 64 for some reason.
    private static readonly int ROUNDING_VALUE = 64;
    private readonly float scaleFactor;

    public SizeScaler(float scaleFactor)
    {
        this.scaleFactor = scaleFactor;
    }

    public Size scale(int width, int height)
    {
        int nonRoundedScaledWidth = downscaleSize(width);
        int scaledWidth = roundSize(nonRoundedScaledWidth);
        //Only width has to be aligned to ROUNDING_VALUE
        float roundingScaleFactor = (float)width / scaledWidth;
        //Ceiling because rounding or flooring might leave empty space on the View's bottom
        int scaledHeight = (int)Java.Lang.Math.Ceil(height / roundingScaleFactor);

        return new Size(scaledWidth, scaledHeight, roundingScaleFactor);
    }

    public bool IsZeroSized(int measuredWidth, int measuredHeight)
    {
        return downscaleSize(measuredHeight) == 0 || downscaleSize(measuredWidth) == 0;
    }

    /**
     * Rounds a value to the nearest divisible by {@link #ROUNDING_VALUE} to meet stride requirement
     */
    private int roundSize(int value)
    {
        if (value % ROUNDING_VALUE == 0)
        {
            return value;
        }
        return value - (value % ROUNDING_VALUE) + ROUNDING_VALUE;
    }

    private int downscaleSize(float value)
    {
        return (int)Java.Lang.Math.Ceil(value / scaleFactor);
    }

    public class Size
    {
        public int width;
        public int height;
        // TODO this is probably not needed anymore
        float scaleFactor;

        public Size(int width, int height, float scaleFactor)
        {
            this.width = width;
            this.height = height;
            this.scaleFactor = scaleFactor;
        }

        public override bool Equals(object o)
        {
            if (this == o) return true;
            if (o == null || GetType() != o.GetType()) return false;

            Size size = (Size)o;

            if (width != size.width) return false;
            if (height != size.height) return false;
            return Float.Compare(size.scaleFactor, scaleFactor) == 0;
        }

        public override int GetHashCode()
        {
            int result = width;
            result = 31 * result + height;
            result = 31 * result + (scaleFactor != +0.0f ? Float.FloatToIntBits(scaleFactor) : 0);
            return result;
        }

        public override string ToString()
        {
            return "Size{" +
                    "width=" + width +
                    ", height=" + height +
                    ", scaleFactor=" + scaleFactor +
                    '}';
        }
    }
}
