using System.Text;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Mutable two-dimensional vector with degree-based angle helpers.
/// The public fields <see cref="X" /> and <see cref="Y" /> can be changed directly.
/// </summary>
public class Vector
{
    private const float GradStep = MathF.PI / 180.0f;
    
    /// <summary>
    /// The X component of the vector.
    /// </summary>
    public float X;
    
    /// <summary>
    /// The Y component of the vector.
    /// </summary>
    public float Y;

    /// <summary>
    /// Creates the zero vector <c>(0, 0)</c>.
    /// </summary>
    public Vector()
    {
        X = 0;
        Y = 0;
    }

    internal static bool FromReader(PacketReader reader, out Vector vector)
    {
        float x;
        float y;

        if (!reader.Read(out x) || !reader.Read(out y))
        {
            vector = new Vector();
            return false;
        }

        vector = new Vector(x, y);
        return true;
    }

    internal void Write(ref PacketWriter writer)
    {
        writer.Write(X);
        writer.Write(Y);
    }

    /// <summary>
    /// Creates a copy of another vector.
    /// </summary>
    /// <param name="vectorToCopy">Vector to copy.</param>
    public Vector(Vector vectorToCopy)
    {
        X = vectorToCopy.X;
        Y = vectorToCopy.Y;
    }

    /// <summary>
    /// Creates a new vector with the given coordinates.
    /// </summary>
    /// <param name="x">X component.</param>
    /// <param name="y">Y component.</param>
    public Vector(float x, float y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Creates a vector from a direction and a length.
    /// </summary>
    /// <param name="angle">Direction in degrees.</param>
    /// <param name="length">Vector length.</param>
    /// <returns>New vector with the requested polar coordinates.</returns>
    public static Vector FromAngleLength(float angle, float length)
    {
        return new Vector(MathF.Cos(angle * GradStep) * length, MathF.Sin(angle * GradStep) * length);
    }

    /// <summary>
    /// Direction of the vector in degrees.
    /// Setting this keeps the current <see cref="Length" /> and rotates the vector accordingly.
    /// </summary>
    public float Angle
    {
        get => (MathF.Atan2(Y, X) / GradStep + 360.0f) % 360.0f;
        set
        {
            float alpha = value * GradStep;
            float length = MathF.Sqrt(X * X + Y * Y);

            X = length * MathF.Cos(alpha);
            Y = length * MathF.Sin(alpha);
        }
    }

    /// <summary>
    /// Squared length of the vector.
    /// </summary>
    public float LengthSquared => X * X + Y * Y;

    /// <summary>
    /// Length of the vector.
    /// Setting this scales the vector while keeping its current direction whenever possible.
    /// </summary>
    public float Length
    {
        get => MathF.Sqrt(X * X + Y * Y);
        set
        {
            if (X == 0 && Y == 0)
            {
                X = value;
                Y = 0;
            }
            else
            {
                float lengthFactor = value / MathF.Sqrt(X * X + Y * Y);

                X *= lengthFactor;
                Y *= lengthFactor;
            }
        }
    }

    /// <summary>
    /// Rotates the vector in place by the specified angle in degrees.
    /// </summary>
    /// <param name="angle">Rotation angle in degrees.</param>
    public void RotatedBy(float angle)
    {
        float alpha = angle * GradStep;

        float x = MathF.Cos(alpha) * X - MathF.Sin(alpha) * Y;

        Y = MathF.Sin(alpha) * X + MathF.Cos(alpha) * Y;
        X = x;
    }

    /// <summary>
    /// Returns the wrapped angle difference in degrees between this vector and another vector.
    /// </summary>
    /// <param name="v">Other vector. If <see langword="null" />, this vector's own <see cref="Angle" /> is returned.</param>
    /// <returns>Angle difference in degrees in the range <c>[0; 360)</c>.</returns>
    public float AngleFrom(Vector? v)
    {
        if (v is null)
            return Angle;

        float deg = (MathF.Atan2(v.Y, v.X) * GradStep + 360.0f) % 360.0f - (MathF.Atan2(Y, X) * GradStep + 360.0f) % 360.0f;

        if (deg < 0)
            deg += 360;

        return deg;
    }

    /// <summary>
    /// Adds two vectors component-wise.
    /// </summary>
    /// <param name="l">Left vector.</param>
    /// <param name="r">Right vector.</param>
    /// <returns>Sum of both vectors.</returns>
    public static Vector operator +(Vector l, Vector r) => new Vector(l.X + r.X, l.Y + r.Y);

    /// <summary>
    /// Subtracts one vector from another component-wise.
    /// </summary>
    /// <param name="l">Left vector.</param>
    /// <param name="r">Right vector.</param>
    /// <returns>Difference of both vectors.</returns>
    public static Vector operator -(Vector l, Vector r) => new Vector(l.X - r.X, l.Y - r.Y);

    /// <summary>
    /// Scales the vector by a scalar factor.
    /// </summary>
    /// <param name="vector">Vector to scale.</param>
    /// <param name="factor">Scalar factor.</param>
    /// <returns>Scaled vector.</returns>
    public static Vector operator *(Vector vector, float factor) => new Vector(factor * vector.X, factor * vector.Y);

    /// <summary>
    /// Divides the vector by a scalar factor.
    /// </summary>
    /// <param name="vector">Vector to divide.</param>
    /// <param name="divisor">Scalar divisor.</param>
    /// <returns>Scaled vector.</returns>
    public static Vector operator /(Vector vector, float divisor) =>
        new Vector(vector.X / divisor, vector.Y / divisor);

    /// <summary>
    /// Compares two vectors using the connector's small tolerance threshold.
    /// </summary>
    /// <param name="l">Left vector.</param>
    /// <param name="r">Right vector.</param>
    /// <returns><see langword="true" /> if both vectors are approximately equal.</returns>
    public static bool operator ==(Vector l, Vector r) => l - r < 0.00015f;

    /// <summary>
    /// Compares the vector length against a scalar using the connector's tolerance threshold.
    /// </summary>
    /// <param name="l">Vector to compare.</param>
    /// <param name="r">Length to compare against.</param>
    /// <returns><see langword="true" /> if the vector length is approximately equal to the scalar.</returns>
    public static bool operator ==(Vector l, float r)
    {
        float length = MathF.Sqrt(l.X * l.X + l.Y * l.Y);

        return length - 0.00015 < r && length + 0.00015 > r;
    }

    /// <summary>
    /// Compares whether the left vector is longer than the right vector.
    /// </summary>
    /// <param name="l">Left vector.</param>
    /// <param name="r">Right vector.</param>
    /// <returns><see langword="true" /> if the left vector has a greater length.</returns>
    public static bool operator >(Vector l, Vector r) => l.X * l.X + l.Y * l.Y > r.X * r.X + r.Y * r.Y;

    /// <summary>
    /// Compares two vectors for approximate inequality.
    /// </summary>
    /// <param name="l">Left vector.</param>
    /// <param name="r">Right vector.</param>
    /// <returns><see langword="true" /> if the vectors are not approximately equal.</returns>
    public static bool operator !=(Vector l, Vector r) => !(l == r);

    /// <summary>
    /// Compares the vector length against a scalar for approximate inequality.
    /// </summary>
    /// <param name="l">Vector to compare.</param>
    /// <param name="r">Length to compare against.</param>
    /// <returns><see langword="true" /> if the vector length is not approximately equal to the scalar.</returns>
    public static bool operator !=(Vector l, float r) => !(l == r);

    /// <summary>
    /// Compares whether the left vector is shorter than the right vector.
    /// </summary>
    /// <param name="l">Left vector.</param>
    /// <param name="r">Right vector.</param>
    /// <returns><see langword="true" /> if the left vector has a smaller length.</returns>
    public static bool operator <(Vector l, Vector r) => l.X * l.X + l.Y * l.Y < r.X * r.X + r.Y * r.Y;

    /// <summary>
    /// Compares whether the vector is longer than the given scalar length.
    /// </summary>
    /// <param name="l">Vector to compare.</param>
    /// <param name="r">Scalar length.</param>
    /// <returns><see langword="true" /> if the vector length is greater than the scalar.</returns>
    public static bool operator >(Vector l, float r) => l.X * l.X + l.Y * l.Y > r * r;

    /// <summary>
    /// Compares whether the vector is shorter than the given scalar length.
    /// </summary>
    /// <param name="l">Vector to compare.</param>
    /// <param name="r">Scalar length.</param>
    /// <returns><see langword="true" /> if the vector length is smaller than the scalar.</returns>
    public static bool operator <(Vector l, float r) => l.X * l.X + l.Y * l.Y < r * r;

    /// <summary>
    /// True if either component contains <c>NaN</c> or an infinity value.
    /// </summary>
    public bool IsDamaged => float.IsInfinity(X) || float.IsNaN(X) || float.IsInfinity(Y) || float.IsNaN(Y);

    /// <summary>
    /// Returns the squared distance between two vectors.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <returns>Squared Euclidean distance.</returns>
    public static float SquaredDistance(Vector a, Vector b) => (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);

    /// <summary>
    /// Returns the distance between two vectors.
    /// </summary>
    /// <param name="a">First vector.</param>
    /// <param name="b">Second vector.</param>
    /// <returns>Euclidean distance.</returns>
    public static float Distance(Vector a, Vector b) =>
        MathF.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));

    /// <summary>
    /// Returns the vector represented in string form.
    /// </summary>
    /// <returns>The string representation of the vector.</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        if (float.IsInfinity(X) || float.IsNaN(X) || float.IsInfinity(Y) || float.IsNaN(Y))
            sb.Append("DAMAGED: ");

        if (float.IsPositiveInfinity(X))
            sb.Append("+INV");
        else if (float.IsNegativeInfinity(X))
            sb.Append("-INV");
        else if (float.IsNaN(X))
            sb.Append("NAN");
        else
            sb.Append(X.ToString("+0000.000;-0000.000;00000.000"));

        sb.Append('/');

        if (float.IsPositiveInfinity(Y))
            sb.Append("+INV");
        else if (float.IsNegativeInfinity(Y))
            sb.Append("-INV");
        else if (float.IsNaN(Y))
            sb.Append("NAN");
        else
            sb.Append(Y.ToString("+0000.000;-0000.000;00000.000"));

        return sb.ToString();
    }

    /// <summary>
    /// Compares this vector with another object using the connector's approximate vector equality.
    /// </summary>
    /// <param name="obj">Object to compare with.</param>
    /// <returns><see langword="true" /> if the object is a vector and is approximately equal.</returns>
    public override bool Equals(object? obj) => obj is Vector vector && this == vector;

    /// <summary>
    /// Generates the hash code from the raw vector components.
    /// </summary>
    /// <returns>Hash code derived from <see cref="X" /> and <see cref="Y" />.</returns>
    public override int GetHashCode() => HashCode.Combine(X, Y);

    /// <summary>
    /// Tests whether another vector is approximately equal to this one.
    /// </summary>
    /// <param name="other">Other vector.</param>
    /// <returns><see langword="true" /> if the vectors are approximately equal.</returns>
    public bool Equals(Vector other) => this == other;
}
