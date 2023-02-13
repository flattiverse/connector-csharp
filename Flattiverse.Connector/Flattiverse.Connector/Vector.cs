using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Connector
{
    public class Vector
    {
        public static Vector Null => new Vector();

        private static double tolerance = 0.0125;

        public double X;
        public double Y;
        private double lastAngle;

        public Vector()
        {
        }

        public Vector(Vector vectorToCopy)
        {
            X = vectorToCopy.X;
            Y = vectorToCopy.Y;
        }

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector(JsonElement json)
        {
            double x;
            double y;

            JsonElement subElement;

            if (!json.TryGetProperty("x", out subElement))
            {
                throw new ArgumentException($"Vector doesn't contain x sub-parameter.");
            }

            if (!subElement.TryGetDouble(out x))
            {
                throw new ArgumentException($"Vector sub-parameter x doesn't contain a double.");
            }

            if (!json.TryGetProperty("y", out subElement))
            {
                throw new ArgumentException($"Vector doesn't contain y sub-parameter.");
            }

            if (!subElement.TryGetDouble(out y))
            {
                throw new ArgumentException($"Vector sub-parameter y doesn't contain a double.");
            }

            X = x;
            Y = y;

        }

        /// <summary>
        /// Tries to parse a json Element to a Vector.
        /// </summary>
        /// <param name="data">The json data.</param>
        /// <exception cref="ArgumentException"></exception>
        internal static bool TryParse(JsonElement json, out Vector vector)
        {
            double x;
            double y;

            JsonElement subElement;

            if (!json.TryGetProperty("x", out subElement))
            {
                vector = new Vector();
                return false;
            }

            if (!subElement.TryGetDouble(out x))
            {
                vector = new Vector();
                return false;
            }

            if (!json.TryGetProperty("y", out subElement))
            {
                vector = new Vector();
                return false;
            }

            if (!subElement.TryGetDouble(out y))
            {
                vector = new Vector();
                return false;
            }

            vector = new Vector(x, y);
            return true;
        }

        /// <summary>
        /// throws Exception on invalid json Element
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="ArgumentException"></exception>
        internal static void validateJson(JsonElement data)
        {
            double temp;

            JsonElement subElement;

            if (!data.TryGetProperty("x", out subElement))
            {
                throw new ArgumentException($"Vector doesn't contain x sub-parameter.");
            }

            if (!subElement.TryGetDouble(out temp))
            {
                throw new ArgumentException($"Vector sub-parameter x doesn't contain a double.");
            }

            if (!data.TryGetProperty("y", out subElement))
            {
                throw new ArgumentException($"Vector doesn't contain y sub-parameter.");
            }

            if (!subElement.TryGetDouble(out temp))
            {
                throw new ArgumentException($"Vector sub-parameter y doesn't contain a double.");
            }
        }

        public static Vector FromXY(double x, double y)
        {
            return new Vector(x, y);
        }

        public static Vector FromAngleLength(double angle, double length)
        {
            return new Vector(Math.Cos(angle * Math.PI / 180f) * length, Math.Sin(angle * Math.PI / 180f) * length);
        }

        public double Angle
        {
            get
            {
                if (X == 0f && Y == 0f)
                    return lastAngle;
                else
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

        public Vector RotatedBy(double angle)
        {
            double alpha = angle * Math.PI / 180;

            return new Vector(Math.Cos(alpha) * X - Math.Sin(alpha) * Y, Math.Sin(alpha) * X + Math.Cos(alpha) * Y);
        }

        public double AngleFrom(Vector v)
        {
            double deg = v.Angle - Angle;

            if (deg < 0)
                deg += 360;

            return deg;
        }

        public static Vector operator +(Vector l, Vector r)
        {
            return new Vector(l.X + r.X, l.Y + r.Y);
        }

        public static Vector operator -(Vector l, Vector r)
        {
            return new Vector(l.X - r.X, l.Y - r.Y);
        }

        public static Vector operator *(Vector vector, double factor)
        {
            Vector resultingVector = new Vector(factor * vector.X, factor * vector.Y);

            if (factor == 0)
                resultingVector.lastAngle = vector.Angle;

            return resultingVector;
        }

        public static Vector operator /(Vector vector, double divisor)
        {
            return Vector.FromXY(vector.X / divisor, vector.Y / divisor);
        }

        public static bool operator ==(Vector l, Vector r)
        {
            if (l is null && r is null)
                return true;

            if (l is null || r is null)
                return false;

            return l - r < tolerance;
        }

        public static bool operator ==(Vector l, double r)
        {
            if ((object)l == null)
                return false;

            double length = l.Length;

            return length - tolerance < r && length + tolerance > r;
        }

        public static bool operator >(Vector l, Vector r)
        {
            return l.X * l.X + l.Y * l.Y > r.X * r.X + r.Y * r.Y;
        }

        public static bool operator !=(Vector l, Vector r)
        {
            return !(l == r);
        }

        public static bool operator !=(Vector l, double r)
        {
            return !(l == r);
        }

        public static bool operator <(Vector l, Vector r)
        {
            return l.X * l.X + l.Y * l.Y < r.X * r.X + r.Y * r.Y;
        }

        public static bool operator >(Vector l, double r)
        {
            return l.X * l.X + l.Y * l.Y > r * r;
        }

        public static bool operator <(Vector l, double r)
        {
            return l.X * l.X + l.Y * l.Y < r * r;
        }

        public bool IsDamaged
        {
            get { return double.IsInfinity(X) || double.IsNaN(X) || double.IsInfinity(Y) || double.IsNaN(Y); }
        }

        public static double SquaredDistance(Vector a, Vector b)
        {
            return (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
        }

        public static double Distance(Vector a, Vector b)
        {
            return Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));
        }

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

        public override bool Equals(object? obj)
        {
            return obj is Vector && this == (Vector)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        internal void writeJson(Utf8JsonWriter writer)
        {
            writer.WriteNumber("x", X);
            writer.WriteNumber("y", Y);
        }
    }
}