using System.Text.Json;

namespace Flattiverse.Connector
{
    public class Region
    {
        public double Left;
        public double Top;
        public double Right;
        public double Bottom;

        public Region(JsonElement element)
        {
            Utils.Traverse(element, out Left, "left");
            Utils.Traverse(element, out Top, "top");
            Utils.Traverse(element, out Right, "right");
            Utils.Traverse(element, out Bottom, "bottom");
        }

        public Region(double left, double top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}