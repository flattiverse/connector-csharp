using System.Text;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A vector representing X and Y coordinates and has some support functions.
/// </summary>
public class Vector
{
    private const double GradStep = Math.PI / 180.0;
    
    /// <summary>
    /// The X component of the vector.
    /// </summary>
    public float X;
    
    /// <summary>
    /// The Y component of the vector.
    /// </summary>
    public float Y;

    /// <summary>
    /// Instantiates a null (0, 0) vector.
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
            vector = new Vector();;
            return false;
        }

        vector = new Vector(x, y);
        return true;
    }

    internal void Write(PacketWriter writer)
    {
        writer.Write(X);
        writer.Write(Y);
    }

    /// <summary>
    /// Copies a vector.
    /// </summary>
    /// <param name="vectorToCopy"></param>
    public Vector(Vector vectorToCopy)
    {
        X = vectorToCopy.X;
        Y = vectorToCopy.Y;
    }

    /// <summary>
    /// Creates a new vector with the given coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Vector(float x, float y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Creates a vector form angle and length.
    /// </summary>
    /// <param name="angle">The angle of the new vector.</param>
    /// <param name="length">The length of the new vector.</param>
    /// <returns>The vector</returns>
    public static Vector FromAngleLength(float angle, float length)
    {
        return new Vector((float)(Math.Cos(angle * GradStep) * length), (float)(Math.Sin(angle * GradStep) * length));
    }

    /// <summary>
    /// The Angle of the vector.
    /// </summary>
    public float Angle
    {
        get => (float)((Math.Atan2(Y, X) / GradStep + 360.0) % 360.0);
        set
        {
            double alpha = value * GradStep;
            double length = Math.Sqrt(X * X + Y * Y);

            X = (float)(length * Math.Cos(alpha));
            Y = (float)(length * Math.Sin(alpha));
        }
    }

    /// <summary>
    /// The sqared length of the vector.
    /// </summary>
    public float LengthSquared => X * X + Y * Y;

    /// <summary>
    /// The length of the vector.
    /// </summary>
    public float Length
    {
        get => (float)Math.Sqrt(X * X + Y * Y);
        set
        {
            if (X == 0 && Y == 0)
            {
                X = value;
                Y = 0;
            }
            else
            {
                float lengthFactor = value / (float)Math.Sqrt(X * X + Y * Y);

                X *= lengthFactor;
                Y *= lengthFactor;
            }
        }
    }

    /// <summary>
    /// Rotates the vector by an angle.
    /// </summary>
    /// <param name="angle">The nagle to rotate the vector.</param>
    public void RotatedBy(double angle)
    {
        double alpha = angle * GradStep;

        float x = (float)(Math.Cos(alpha) * X - Math.Sin(alpha) * Y);

        Y = (float)(Math.Sin(alpha) * X + Math.Cos(alpha) * Y);
        X = x;
    }

    /// <summary>
    /// Returns the Angle between this and the given vector.
    /// </summary>
    /// <param name="v">The vector to calculate the angle from.</param>
    /// <returns>The angle in degrees.</returns>
    public float AngleFrom(Vector? v)
    {
        if (v is null)
            return Angle;

        double deg = ((Math.Atan2(v.Y, v.X) * GradStep + 360.0) % 360.0) - ((Math.Atan2(Y, X) * GradStep + 360.0) % 360.0);

        if (deg < 0)
            deg += 360;

        return (float)deg;
    }

    /// <summary>
    /// The addition operator overload.
    /// </summary>
    /// <param name="l">The left vector.</param>
    /// <param name="r">The right vector.</param>
    /// <returns>The summed up vector.</returns>
    public static Vector operator +(Vector l, Vector r) => new Vector(l.X + r.X, l.Y + r.Y);

    /// <summary>
    /// The subtraction operator overload.
    /// </summary>
    /// <param name="l">The left vector.</param>
    /// <param name="r">The right vector.</param>
    /// <returns>The subtracted vector.</returns>
    public static Vector operator -(Vector l, Vector r) => new Vector(l.X - r.X, l.Y - r.Y);

    /// <summary>
    /// Multiplies the vector by the given length.
    /// </summary>
    /// <param name="vector">The vector to multiply.</param>
    /// <param name="factor">The factor to multiply the vector with.</param>
    /// <returns>The multiplied vector.</returns>
    public static Vector operator *(Vector vector, float factor) => new Vector(factor * vector.X, factor * vector.Y);

    /// <summary>
    /// Divides the vector by the given length.
    /// </summary>
    /// <param name="vector">The vector to divide.</param>
    /// <param name="factor">The factor to divide the vector with.</param>
    /// <returns>The divided vector.</returns>
    public static Vector operator /(Vector vector, float divisor) =>
        new Vector(vector.X / divisor, vector.Y / divisor);

    /// <summary>
    /// Compares two vectors.
    /// </summary>
    /// <param name="l">The vector to compare.</param>
    /// <param name="r">The other vector to compare.</param>
    /// <returns>true if the vector is round about the same.</returns>
    public static bool operator ==(Vector l, Vector r) => l - r < 0.00015f;

    /// <summary>
    /// Compares the length of the vector.
    /// </summary>
    /// <param name="l">The vector to compare.</param>
    /// <param name="r">The length to compare.</param>
    /// <returns>true if the vector is round about the length.</returns>
    public static bool operator ==(Vector l, float r)
    {
        double length = Math.Sqrt(l.X * l.X + l.Y * l.Y);

        return length - 0.00015 < r && length + 0.00015 > r;
    }

    /// <summary>
    /// Compares if one vector is longer than the other.
    /// </summary>
    /// <param name="l">The left vector.</param>
    /// <param name="r">The right vector.</param>
    /// <returns>true if the left vector is longer than the right vector.</returns>
    public static bool operator >(Vector l, Vector r) => l.X * l.X + l.Y * l.Y > r.X * r.X + r.Y * r.Y;

    /// <summary>
    /// Compares if one vector is not like the other vector.
    /// </summary>
    /// <param name="l">The left vector.</param>
    /// <param name="r">The right vector.</param>
    /// <returns>true if the left and the right vector are not the same in direction and length.</returns>
    public static bool operator !=(Vector l, Vector r) => !(l == r);

    /// <summary>
    /// Compares if one vector is not as long as the given factor.
    /// </summary>
    /// <param name="l">The left vector.</param>
    /// <param name="r">The right length.</param>
    /// <returns>true if the left vector and the length are not about the same length.</returns>
    public static bool operator !=(Vector l, float r) => !(l == r);

    /// <summary>
    /// Compares if one vector is shorter than the other.
    /// </summary>
    /// <param name="l">The left vector.</param>
    /// <param name="r">The right vector.</param>
    /// <returns>true if the left vector is shorter than the right vector.</returns>
    public static bool operator <(Vector l, Vector r) => l.X * l.X + l.Y * l.Y < r.X * r.X + r.Y * r.Y;

    /// <summary>
    /// Compares if the vector is longer than the scalar.
    /// </summary>
    /// <param name="l">The vector.</param>
    /// <param name="r">The scalar.</param>
    /// <returns>true if the vector is longer than the scalar.</returns>
    public static bool operator >(Vector l, float r) => l.X * l.X + l.Y * l.Y > r * r;

    /// <summary>
    /// Compares if the vector is shorter than the scalar.
    /// </summary>
    /// <param name="l">The vector.</param>
    /// <param name="r">The scalar.</param>
    /// <returns>true if the vector is shorter than the scalar.</returns>
    public static bool operator <(Vector l, float r) => l.X * l.X + l.Y * l.Y < r * r;

    /// <summary>
    /// true, if any component contains NaN or such values.
    /// </summary>
    public bool IsDamaged => float.IsInfinity(X) || float.IsNaN(X) || float.IsInfinity(Y) || float.IsNaN(Y);

    /// <summary>
    /// Returns the squared distance between the two vectors.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>The squared distance.</returns>
    public static float SquaredDistance(Vector a, Vector b) => (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);

    /// <summary>
    /// Returns the distance between the two vectors.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>The distance.</returns>
    public static float Distance(Vector a, Vector b) =>
        (float)Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));

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
    /// Compares if the object is equal.
    /// </summary>
    /// <param name="obj">The object to compare</param>
    /// <returns>truw, if they are equal.</returns>
    public override bool Equals(object? obj) => obj is Vector vector && this == vector;

    /// <summary>
    /// Generates the HashCode.
    /// </summary>
    /// <returns>The hashcode.</returns>
    public override int GetHashCode() => HashCode.Combine(X, Y);

    /// <summary>
    /// Tests if the other vector is equal to this one.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>true, if compatible</returns>
    public bool Equals(Vector other) => this == other;
}