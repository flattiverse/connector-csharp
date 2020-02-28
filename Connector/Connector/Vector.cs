using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// A vector.
    /// </summary>
    public class Vector
    {
        private static float PI = (float)Math.PI;

        private float x;
        private float y;
        private float lastAngle;

        /// <summary>
        /// Creates a new empty vector.
        /// </summary>
        public Vector()
        {
        }

        /// <summary>
        /// Creates a copy of a vector.
        /// </summary>
        /// <param name="vectorToCopy">The vector which will be copied.</param>
        public Vector(Vector vectorToCopy)
        {
            x = vectorToCopy.x;
            y = vectorToCopy.y;
        }

        /// <summary>
        /// Creates a new vector from xy coordinates.
        /// </summary>
        /// <param name="x">The x component of the vector.</param>
        /// <param name="y">The y component of the vector.</param>
        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Returns a new vector from xy coordinates.
        /// </summary>
        /// <param name="x">The x component of the vector.</param>
        /// <param name="y">The y component of the vector.</param>
        /// <returns>The new vector.</returns>
        public static Vector FromXY(float x, float y)
        {
            return new Vector(x, y);
        }

        /// <summary>
        /// Returns a new vector from specified angle and length.
        /// </summary>
        /// <param name="angle">The angle of the vector.</param>
        /// <param name="length">The length of the vector.</param>
        /// <returns>The new vector.</returns>
        public static Vector FromAngleLength(float angle, float length)
        {
            return new Vector((float)Math.Cos((double)(angle * PI / 180f)) * length, (float)Math.Sin((double)(angle * PI / 180f)) * length);
        }

        /// <summary>
        /// Returns an empty vector.
        /// </summary>
        /// <returns>An empty vector.</returns>
        public static Vector FromNull()
        {
            return new Vector();
        }

        internal Vector(ref BinaryMemoryReader reader)
        {
            x = reader.ReadSingle();
            y = reader.ReadSingle();
        }

        internal void Write(ManagedBinaryMemoryWriter writer)
        {
            writer.Write(x);
            writer.Write(y);
        }

        /// <summary>
        /// The x component of the vector.
        /// </summary>
        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        /// <summary>
        /// The y component of the vector.
        /// </summary>
        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        /// <summary>
        /// The angle of the vector.
        /// </summary>
        public float Angle
        {
            get
            {
                if (x == 0f && y == 0f)
                    return lastAngle;
                else
                    return ((float)Math.Atan2(y, x) * 180f / PI + 360f) % 360f;
            }
            set
            {
                double alpha = value * PI / 180f;
                float length = (float)Math.Sqrt(x * x + y * y);

                x = length * (float)Math.Cos(alpha);
                y = length * (float)Math.Sin(alpha);
            }
        }

        /// <summary>
        /// The length of the vector.
        /// </summary>
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y);
            }
            set
            {
                if (value == 0f)
                    lastAngle = Angle;

                if (x == 0f && y == 0f)
                {
                    double alpha = lastAngle * PI / 180f;

                    x = value * (float)Math.Cos(alpha);
                    y = value * (float)Math.Sin(alpha);
                }
                else
                {
                    float lengthFactor = value / Length;

                    x *= lengthFactor;
                    y *= lengthFactor;
                }
            }
        }

        /// <summary>
        /// Returns a new and rotated vector.
        /// </summary>
        /// <param name="angle">The angle the vector is rotated by.</param>
        /// <returns>The rotated vector.</returns>
        public Vector RotatedBy(float angle)
        {
            double alpha = angle * PI / 180f;

            return new Vector((float)Math.Cos(alpha) * x - (float)Math.Sin(alpha) * y, (float)Math.Sin(alpha) * x + (float)Math.Cos(alpha) * y);
        }

        /// <summary>
        /// Returns the angle of a vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <returns></returns>
        public float AngleFrom(Vector v)
        {
            float deg = v.Angle - Angle;

            if (deg < 0)
                deg += 360f;

            return deg;
        }

        /// <summary>
        /// Returns the sum of two vectors.
        /// </summary>
        /// <param name="l">The first vector.</param>
        /// <param name="r">The second vector, which will be added to the first.</param>
        /// <returns>The summation of the two vectors.</returns>
        public static Vector operator +(Vector l, Vector r)
        {
            return new Vector(l.x + r.x, l.y + r.y);
        }

        /// <summary>
        /// Returns the difference of two vectors.
        /// </summary>
        /// <param name="l">The first vector.</param>
        /// <param name="r">The second vector, which will be subtracted from the first.</param>
        /// <returns>The difference of the two vectors.</returns>
        public static Vector operator -(Vector l, Vector r)
        {
            return new Vector(l.x - r.x, l.y - r.y);
        }

        /// <summary>
        /// Returns a new vector with a modified length.
        /// </summary>
        /// <param name="vector">The old vector.</param>
        /// <param name="factor">The factor by which length of the old vector is stretched.</param>
        /// <returns>The stretched vector.</returns>
        public static Vector operator *(Vector vector, float factor)
        {
            Vector resultingVector = new Vector(factor * vector.x, factor * vector.y);

            if (factor == 0)
                resultingVector.lastAngle = vector.Angle;

            return resultingVector;
        }

        /// <summary>
        /// Returns a new vector with a modified length.
        /// </summary>
        /// <param name="vector">The old vector.</param>
        /// <param name="divisor">The factor by which the length of the old vector will be divided.</param>
        /// <returns>The compressed vector.</returns>
        public static Vector operator /(Vector vector, float divisor)
        {
            return new Vector(vector.x / divisor, vector.y / divisor);
        }

        /// <summary>
        /// Checks if the two vectors match with a tolerance of 0.25
        /// </summary>
        /// <param name="l">The first vector.</param>
        /// <param name="r">The second vector.</param>
        /// <returns>True or false.</returns>
        public static bool operator ==(Vector l, Vector r)
        {
            if ((object)l == null && (object)r == null)
                return true;

            if ((object)l == null || (object)r == null)
                return false;

            return l - r < 0.25f;
        }

        /// <summary>
        /// Checks if the length of a vector matches a value with a tolerance of 0.25
        /// </summary>
        /// <param name="l">The vector.</param>
        /// <param name="r">The desired length.</param>
        /// <returns>True or false.</returns>
        public static bool operator ==(Vector l, float r)
        {
            if ((object)l == null)
                return false;

            float length = l.Length;

            return length - 0.25f < r && length + 0.25f > r;
        }

        /// <summary>
        /// Checks if the first vector is larger than the second.
        /// </summary>
        /// <param name="l">The first vector.</param>
        /// <param name="r">The second vector.</param>
        /// <returns>True or false.</returns>
        public static bool operator >(Vector l, Vector r)
        {
            return l.x * l.x + l.y * l.y > r.x * r.x + r.y * r.y;
        }

        /// <summary>
        /// Checks if two vectors are the same.
        /// </summary>
        /// <param name="l">The first vector.</param>
        /// <param name="r">The second vector.</param>
        /// <returns>True or false.</returns>
        public static bool operator !=(Vector l, Vector r)
        {
            return !(l == r);
        }

        /// <summary>
        /// Checks if the length of the vector equals the given length.
        /// </summary>
        /// <param name="l">The vector.</param>
        /// <param name="r">The length to compare.</param>
        /// <returns>True or false.</returns>
        public static bool operator !=(Vector l, float r)
        {
            return !(l == r);
        }

        /// <summary>
        /// Checks if the first vector is smaller than the second.
        /// </summary>
        /// <param name="l">The first vector.</param>
        /// <param name="r">The second vector.</param>
        /// <returns>True or false.</returns>
        public static bool operator <(Vector l, Vector r)
        {
            return l.x * l.x + l.y * l.y < r.x * r.x + r.y * r.y;
        }

        /// <summary>
        /// Checks if the vector is shorter.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool operator >(Vector l, float r)
        {
            return l.x * l.x + l.y * l.y > r * r;
        }

        /// <summary>
        /// Checks if the vector is longer.
        /// </summary>
        /// <param name="l">The vector.</param>
        /// <param name="r">The length to compare.</param>
        /// <returns>True or false.</returns>
        public static bool operator <(Vector l, float r)
        {
            return l.x * l.x + l.y * l.y < r * r;
        }

        /// <summary>
        /// Unary minus operator
        /// </summary>
        /// <param name="v">Input vector</param>
        /// <returns>A Vector pointing in the opposite direction</returns>
        public static Vector operator -(Vector v)
        {
            return new Vector(-v.X, -v.Y);
        }

        /// <summary>
        /// Checks if any component of the vector is infinite or not a number.
        /// </summary>
        public bool IsDamaged
        {
            get { return float.IsInfinity(x) || float.IsNaN(x) || float.IsInfinity(y) || float.IsNaN(y); }
        }

        /// <summary>
        /// Returns the vector.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (float.IsInfinity(x) || float.IsNaN(x) || float.IsInfinity(y) || float.IsNaN(y))
                sb.Append("DAMAGED: ");

            if (float.IsPositiveInfinity(x))
                sb.Append("+INV");
            else if (float.IsNegativeInfinity(x))
                sb.Append("-INV");
            else if (float.IsNaN(x))
                sb.Append("NAN");
            else
                sb.Append(x.ToString("F"));

            sb.Append('/');

            if (float.IsPositiveInfinity(y))
                sb.Append("+INV");
            else if (float.IsNegativeInfinity(y))
                sb.Append("-INV");
            else if (float.IsNaN(y))
                sb.Append("NAN");
            else
                sb.Append(y.ToString("F"));

            return sb.ToString();
        }

        /// <summary>
        /// Checks if this equals an object.
        /// </summary>
        /// <param name="obj">The object to compare with this vector.</param>
        /// <returns>True or false.</returns>
        public override bool Equals(object obj)
        {
            return (object)this == obj;
        }

        /// <summary>
        /// Returns the hashcode of this vector.
        /// </summary>
        /// <returns>The hashcode as an int.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
