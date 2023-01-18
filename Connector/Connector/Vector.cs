using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse
{
    public class Vector
    {
        public static readonly Vector Null = new Vector();

        private static double tolerance = 0.0125;

        public double X;
        public double Y;
        private double lastAngle;

        internal void WriteJson(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteNumber("X", X);
            writer.WriteNumber("Y", Y);
            writer.WriteEndObject();
        }
    }
}
