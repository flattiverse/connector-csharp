using System.Diagnostics;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

static class RangeTolerance
{
    public const float LowerFactor = 0.999f;
    public const float UpperFactor = 1.001f;

    public static InvalidArgumentKind Validate(float value)
    {
        if (float.IsNaN(value))
            return InvalidArgumentKind.ContainedNaN;

        if (float.IsInfinity(value))
            return InvalidArgumentKind.ContainedInfinity;

        return InvalidArgumentKind.Valid;
    }

    public static InvalidArgumentKind Validate(Vector value)
    {
        if (float.IsNaN(value.X) || float.IsNaN(value.Y))
            return InvalidArgumentKind.ContainedNaN;

        if (float.IsInfinity(value.X) || float.IsInfinity(value.Y))
            return InvalidArgumentKind.ContainedInfinity;

        return InvalidArgumentKind.Valid;
    }

    public static InvalidArgumentKind ClampMaximum(float value, float maximum, out float clampedValue)
    {
        Debug.Assert(maximum >= 0f && !float.IsNaN(maximum) && !float.IsInfinity(maximum), "Invalid scalar maximum specified.");

        InvalidArgumentKind validity = Validate(value);

        if (validity != InvalidArgumentKind.Valid)
        {
            clampedValue = 0f;
            return validity;
        }

        if (value < 0f)
        {
            clampedValue = 0f;
            return InvalidArgumentKind.TooSmall;
        }

        if (value > maximum * UpperFactor)
        {
            clampedValue = 0f;
            return InvalidArgumentKind.TooLarge;
        }

        clampedValue = value > maximum ? maximum : value;
        return InvalidArgumentKind.Valid;
    }

    public static InvalidArgumentKind ClampRange(float value, float minimum, float maximum, out float clampedValue)
    {
        Debug.Assert(maximum >= minimum && !float.IsNaN(minimum) && !float.IsInfinity(minimum) &&
            !float.IsNaN(maximum) && !float.IsInfinity(maximum), "Invalid scalar range specified.");

        InvalidArgumentKind validity = Validate(value);

        if (validity != InvalidArgumentKind.Valid)
        {
            clampedValue = 0f;
            return validity;
        }

        float toleratedMinimum = minimum >= 0f ? minimum * LowerFactor : minimum * UpperFactor;
        float toleratedMaximum = maximum >= 0f ? maximum * UpperFactor : maximum * LowerFactor;

        if (value < toleratedMinimum)
        {
            clampedValue = 0f;
            return InvalidArgumentKind.TooSmall;
        }

        if (value > toleratedMaximum)
        {
            clampedValue = 0f;
            return InvalidArgumentKind.TooLarge;
        }

        if (value < minimum)
        {
            clampedValue = minimum;
            return InvalidArgumentKind.Valid;
        }

        clampedValue = value > maximum ? maximum : value;
        return InvalidArgumentKind.Valid;
    }

    public static InvalidArgumentKind ClampMaximum(Vector value, float maximum, out Vector clampedValue)
    {
        Debug.Assert(maximum >= 0f && !float.IsNaN(maximum) && !float.IsInfinity(maximum), "Invalid vector maximum specified.");

        InvalidArgumentKind validity = Validate(value);

        if (validity != InvalidArgumentKind.Valid)
        {
            clampedValue = new Vector();
            return validity;
        }

        double length = Math.Sqrt((double)value.X * value.X + (double)value.Y * value.Y);

        if (length > maximum * UpperFactor)
        {
            clampedValue = new Vector();
            return InvalidArgumentKind.TooLarge;
        }

        if (length <= maximum)
        {
            clampedValue = value;
            return InvalidArgumentKind.Valid;
        }

        TryClampLength(value, length, maximum, out clampedValue);
        return InvalidArgumentKind.Valid;
    }

    public static InvalidArgumentKind ClampRange(Vector value, float minimum, float maximum, out Vector clampedValue)
    {
        Debug.Assert(minimum >= 0f && maximum >= minimum && !float.IsNaN(minimum) && !float.IsInfinity(minimum) &&
            !float.IsNaN(maximum) && !float.IsInfinity(maximum), "Invalid vector range specified.");

        InvalidArgumentKind validity = Validate(value);

        if (validity != InvalidArgumentKind.Valid)
        {
            clampedValue = new Vector();
            return validity;
        }

        double length = Math.Sqrt((double)value.X * value.X + (double)value.Y * value.Y);

        if (length < minimum * LowerFactor)
        {
            clampedValue = new Vector();
            return InvalidArgumentKind.TooSmall;
        }

        if (length > maximum * UpperFactor)
        {
            clampedValue = new Vector();
            return InvalidArgumentKind.TooLarge;
        }

        if (length < minimum)
        {
            TryClampLength(value, length, minimum, out clampedValue);
            return InvalidArgumentKind.Valid;
        }

        if (length > maximum)
        {
            TryClampLength(value, length, maximum, out clampedValue);
            return InvalidArgumentKind.Valid;
        }

        clampedValue = value;
        return InvalidArgumentKind.Valid;
    }

    private static void TryClampLength(Vector value, double currentLength, float targetLength, out Vector clampedValue)
    {
        Debug.Assert(targetLength >= 0f && !float.IsNaN(targetLength) && !float.IsInfinity(targetLength), "Invalid vector target length specified.");

        if (targetLength == 0f)
        {
            clampedValue = new Vector();
            return;
        }

        if (currentLength == 0d)
        {
            clampedValue = new Vector();
            return;
        }

        double scale = targetLength / currentLength;
        Vector candidate = new Vector((float)((double)value.X * scale), (float)((double)value.Y * scale));

        if (candidate > targetLength)
        {
            float safeTargetLength = MathF.BitDecrement(targetLength);

            if (safeTargetLength <= 0f)
            {
                clampedValue = new Vector();
                return;
            }

            scale = safeTargetLength / currentLength;
            candidate = new Vector((float)((double)value.X * scale), (float)((double)value.Y * scale));
        }

        clampedValue = candidate;
    }
}
