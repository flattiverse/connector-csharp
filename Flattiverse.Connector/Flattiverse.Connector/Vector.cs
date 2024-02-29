using Flattiverse.Connector.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Connector
{
    /// <summary>
    /// The vector class used by flattiverse
    /// </summary>
    public class Vector
    {
        /// <summary>
        /// The null or zero vector, with both elements set to zero.
        /// </summary>
        public static Vector Null => new Vector();

        private static double tolerance = 0.000015;

        /// <summary>
        /// The X component of the vector.
        /// </summary>
        public double X;

        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        public double Y;
        private double lastAngle;

        /// <summary>
        /// The default constructor.
        /// </summary>
        public Vector()
        {
        }

        internal Vector(PacketReader reader)
        {
            X = reader.ReadDouble();
            Y = reader.ReadDouble();
        }

        internal void Write(PacketWriter writer)
        {
            if (IsDamaged)
            {
                writer.Write(0UL);

                return;
            }

            if (X < -21470)
                writer.Write(-2147000000);
            else if (X > 21470)
                writer.Write(2147000000);
            else
                writer.Write(X);

            if (Y < -21470)
                writer.Write(-2147000000);
            else if (Y > 21470)
                writer.Write(2147000000);
            else
                writer.Write(Y);
        }

        /// <summary>
        /// The copy constructor.
        /// </summary>
        public Vector(Vector vectorToCopy)
        {
            X = vectorToCopy.X;
            Y = vectorToCopy.Y;
        }

        /// <summary>
        /// The constructor with the X and Y components.
        /// </summary>
        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }
        
        /// <summary>
        /// Construct a vector from polar coordinates.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="length"></param>
        /// <returns cref="Vector"></returns>
        public static Vector FromAngleLength(double angle, double length)
        {
            return new Vector(Math.Cos(angle * Math.PI / 180f) * length, Math.Sin(angle * Math.PI / 180f) * length);
        }

        /// <summary>
        /// The polar angle of a vector
        /// </summary>
        public double Angle
        {
            get
            {
                if (X == 0f && Y == 0f)
                    return lastAngle;

                return (Math.Atan2(Y, X) * 180 / Math.PI + 360) % 360f;
            }
            set
            {
                double alpha = value * Math.PI / 180;
                double length = Math.Sqrt(X * X + Y * Y);

                X = length * Math.Cos(alpha);
                Y = length * Math.Sin(alpha);
            }
        }

        /// <summary>
        /// The squared length of the vector.
        /// </summary>
        public double LengthSquared => X * X + Y * Y;

        /// <summary>
        /// The length of the vector.
        /// </summary>
        public double Length
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y);
            }
            set
            {
                if (value == 0f)
                    lastAngle = Angle;

                if (X == 0f && Y == 0f)
                {
                    double alpha = lastAngle * Math.PI / 180f;

                    X = value * Math.Cos(alpha);
                    Y = value * Math.Sin(alpha);
                }
                else
                {
                    double lengthFactor = value / Length;

                    X *= lengthFactor;
                    Y *= lengthFactor;
                }
            }
        }

        /// <summary>
        /// Rotates a vector by an angle
        /// </summary>
        /// <param name="angle"></param>
        public void RotatedBy(double angle)
        {
            double alpha = angle * Math.PI / 180;

            double x = Math.Cos(alpha) * X - Math.Sin(alpha) * Y;
            Y = Math.Sin(alpha) * X + Math.Cos(alpha) * Y;
            X = x;
        }

        /// <summary>
        /// The angle between two vectors
        /// </summary>
        /// <param name="v"></param>
        /// <returns cref="double"></returns>
        public double AngleFrom(Vector? v)
        {
            if (v is null)
                return -Angle;
            
            double deg = v.Angle - Angle;

            if (deg < 0)
                deg += 360;

            return deg;
        }

        /// <summary>
        /// Component wise addition of two vectors
        /// </summary>
        /// <returns cref="Vector"></returns>
        public static Vector operator +(Vector? l, Vector? r)
        {
            if (l is null && r is null)
                return Null;

            if (l is null)
                return r!;

            if (r is null)
                return l;
            
            return new Vector(l.X + r.X, l.Y + r.Y);
        }

        /// <summary>
        /// Component wise subtraction of two vectors
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns cref="Vector"></returns>
        public static Vector operator -(Vector? l, Vector? r)
        {
            if (l is null && r is null)
                return Null;

            if (l is null)
                return new Vector(-r.X, -r.Y);

            if (r is null)
                return l;
            
            return new Vector(l.X - r.X, l.Y - r.Y);
        }

        /// <summary>
        /// Component wise multiplication of two vectors with a scalar
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="factor"></param>
        /// <returns cref="Vector"></returns>
        public static Vector operator *(Vector? vector, double factor)
        {
            if (vector is null)
                return Null;
            
            Vector resultingVector = new Vector(factor * vector.X, factor * vector.Y);

            if (factor == 0)
                resultingVector.lastAngle = vector.Angle;

            return resultingVector;
        }

        /// <summary>
        /// Component wise division of two vectors with a scalar
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="divisor"></param>
        /// <returns cref="Vector"></returns>
        public static Vector operator /(Vector? vector, double divisor)
        {
            if (vector is null)
                return Null;
            
            return new Vector(vector.X / divisor, vector.Y / divisor);
        }

        /// <summary>
        /// Equality check between vectors
        /// </summary>
        /// <returns cref="bool"></returns>
        public static bool operator ==(Vector? l, Vector? r)
        {
            if (l is null && r is null)
                return true;

            if (l is null || r is null)
                return false;

            return l - r < tolerance;
        }

        /// <summary>
        /// Equality check between the length of a vector and a scalar
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns cref="bool"></returns>
        public static bool operator ==(Vector? l, double r)
        {
            if (l is null)
                return false;

            double length = l.Length;

            return length - tolerance < r && length + tolerance > r;
        }

        /// <summary>
        /// Greater than check between vector lengths
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns cref="bool"></returns>
        public static bool operator >(Vector? l, Vector? r)
        {
            if (l is null)
                return false;

            if (r is null)
                return true;
            
            return l.X * l.X + l.Y * l.Y > r.X * r.X + r.Y * r.Y;
        }

        /// <summary>
        /// Inequality check between vectors
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns cref="bool"></returns>
        public static bool operator !=(Vector? l, Vector? r)
        {
            return !(l == r);
        }

        /// <summary>
        /// Inequality check between the length of a vector and a scalar
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns cref="bool"></returns>
        public static bool operator !=(Vector? l, double r)
        {
            return !(l == r);
        }

        /// <summary>
        /// Greater than check between two vectors
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns cref="bool"></returns>
        public static bool operator <(Vector? l, Vector? r)
        {
            if (l is null && r is null)
                return false;
            
            if (l is null)
                return true;

            if (r is null)
                return false;
            
            return l.X * l.X + l.Y * l.Y < r.X * r.X + r.Y * r.Y;
        }

        /// <summary>
        /// Greater than check between the length of a vector and a scalar
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns cref="bool"></returns>
        public static bool operator >(Vector? l, double r)
        {
            if (l is null)
                return false;
            
            return l.X * l.X + l.Y * l.Y > r * r;
        }

        /// <summary>
        /// Less than check between the length of a vector and a scalar
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns cref="bool"></returns>
        public static bool operator <(Vector? l, double r)
        {
            if (l is null)
                return true;
            
            return l.X * l.X + l.Y * l.Y < r * r;
        }

        /// <summary>
        /// Indicates if the vector contains any invalid values
        /// </summary>
        public bool IsDamaged => double.IsInfinity(X) || double.IsNaN(X) || double.IsInfinity(Y) || double.IsNaN(Y);

        /// <summary>
        /// The squared distance between two vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns cref="double"></returns>
        public static double SquaredDistance(Vector? a, Vector? b)
        {
            if (a is null && b is null)
                return 0.0;

            if (a is null)
                return b!.X * b.X + b.Y * b.Y;

            if (b is null)
                return a.X * a.X + a.Y * a.Y;
            
            return (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
        }

        /// <summary>
        /// The distance between two vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns cref="double"></returns>
        public static double Distance(Vector? a, Vector? b)
        {
            if (a is null)
                a = Null;

            if (b is null)
                b = Null;
            
            return Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (double.IsInfinity(X) || double.IsNaN(X) || double.IsInfinity(Y) || double.IsNaN(Y))
                sb.Append("DAMAGED: ");

            if (double.IsPositiveInfinity(X))
                sb.Append("+INV");
            else if (double.IsNegativeInfinity(X))
                sb.Append("-INV");
            else if (double.IsNaN(X))
                sb.Append("NAN");
            else
                sb.Append(X.ToString("+0000.000;-0000.000;00000.000"));

            sb.Append('/');

            if (double.IsPositiveInfinity(Y))
                sb.Append("+INV");
            else if (double.IsNegativeInfinity(Y))
                sb.Append("-INV");
            else if (double.IsNaN(Y))
                sb.Append("NAN");
            else
                sb.Append(Y.ToString("+0000.000;-0000.000;00000.000"));

            return sb.ToString();
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is Vector && this == (Vector)obj;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}