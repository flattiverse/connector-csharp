using System.Text.Json;

namespace Flattiverse
{
    internal static class Utils
    {
        internal static bool Traverse(JsonElement element, out string text, bool allowEmpty, params string[] path)
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

            if(!allowEmpty && text == string.Empty)
                return false;

            return true;
        }

        internal static bool Traverse(JsonElement element, out double number, params string[] path)
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

        internal static bool Traverse(JsonElement element, out int number, params string[] path)
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

        internal static bool Traverse(JsonElement element, out DateTime stamp, params string[] path)
        {
            int pNum;

            foreach (string p in path)
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Array:
                        if (!int.TryParse(p, out pNum))
                        {
                            stamp = default;
                            return false;
                        }

                        element = element[pNum];
                        break;
                    case JsonValueKind.Object:
                        if (!element.TryGetProperty(p, out element))
                        {
                            stamp = default;
                            return false;
                        }
                        break;
                    default:
                        stamp = default;
                        return false;
                }
            }

            return element.TryGetDateTime(out stamp);
        }

        internal static bool Traverse(JsonElement element, out JsonElement subElement, JsonValueKind kind, params string[] path)
        {
            int pNum;

            foreach (string p in path)
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Array:
                        if (!int.TryParse(p, out pNum))
                        {
                            subElement = default;
                            return false;
                        }

                        element = element[pNum];
                        break;
                    case JsonValueKind.Object:
                        if (!element.TryGetProperty(p, out element))
                        {
                            subElement = default;
                            return false;
                        }
                        break;
                    default:
                        subElement = default;
                        return false;
                }
            }

            if(element.ValueKind != kind)
            {
                subElement = default;
                return false;
            }

            subElement = element;
            return true;
        }
    }
}
