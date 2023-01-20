using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Events
{
    public class FlattiverseEvent
    {
        internal static FlattiverseEvent parse(Connection connection, JsonElement element)
        {
            string kind;

            if (!traverse(element, out kind, "kind"))
                throw new InvalidDataException("Event does not contain kind.");

            switch (kind)
            {
                case "newUnit":
                    return new UnitEventNew(element);
                case "broadcastMessage":
                    return new FlattiverseBroadCastMessage(connection, element);
                default:
                    throw new NotImplementedException();
            }
        }

        protected static bool traverse(JsonElement element, out string text, params string[] path)
        {
            int pNum;

            foreach (string p in path)
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Array:
                        if (!int.TryParse(p, out pNum))
                        {
                            text = string.Empty;
                            return false;
                        }

                        element = element[pNum];
                        break;
                    case JsonValueKind.Object:
                        if (!element.TryGetProperty(p, out element))
                        {
                            text = string.Empty;
                            return false;
                        }
                        break;
                    default:
                        text = string.Empty;
                        return false;
                }
            }

            if (element.ValueKind != JsonValueKind.String)
            {
                text = string.Empty;
                return false;
            }

            text = element.GetString()!;
            return true;
        }

        protected static bool traverse(JsonElement element, out double number, params string[] path)
        {
            int pNum;

            foreach (string p in path)
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Array:
                        if (!int.TryParse(p, out pNum))
                        {
                            number = 0.0;
                            return false;
                        }

                        element = element[pNum];
                        break;
                    case JsonValueKind.Object:
                        if (!element.TryGetProperty(p, out element))
                        {
                            number = 0.0;
                            return false;
                        }
                        break;
                    default:
                        number = 0.0;
                        return false;
                }
            }

            return element.TryGetDouble(out number);
        }

        protected static bool traverse(JsonElement element, out int number, params string[] path)
        {
            int pNum;

            foreach (string p in path)
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Array:
                        if (!int.TryParse(p, out pNum))
                        {
                            number = 0;
                            return false;
                        }

                        element = element[pNum];
                        break;
                    case JsonValueKind.Object:
                        if (!element.TryGetProperty(p, out element))
                        {
                            number = 0;
                            return false;
                        }
                        break;
                    default:
                        number = 0;
                        return false;
                }
            }

            return element.TryGetInt32(out number);
        }
    }
}
