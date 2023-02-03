using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Connector
{
    static class Utils
    {
        /// <summary>
        /// Checks the name.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>truw if the name is proper, false if the name doesn't pass the check.</returns>
        public static bool CheckName(string? name)
        {
            if (name == null || name.Length < 2 || name.Length > 32)
                return false;

            if (name.StartsWith(' ') || name.EndsWith(' '))
                return false;

            foreach (char c in name)
            {
                if (c >= 'a' && c <= 'z')
                    continue;

                if (c >= 'A' && c <= 'Z')
                    continue;

                if (c >= '0' && c <= '9')
                    continue;

                if (c >= 192 && c <= 214)
                    continue;

                if (c >= 216 && c <= 246)
                    continue;

                if (c >= 248 && c <= 687)
                    continue;

                if (c == ' ' || c == '.' || c == '_' || c == '-')
                    continue;

                return false;
            }

            return true;
        }

        public static char[] GenerateAllAllowedOneByteUtf8CharsWithoutSpace()
        {
            List<char> allowedChars = new List<char> { '.', '-', '_' };

            for (char c = 'A'; c <= 'Z'; c++)
                allowedChars.Add(c);

            for (char c = 'a'; c <= 'z'; c++)
                allowedChars.Add(c);

            for (char c = '0'; c <= '9'; c++)
                allowedChars.Add(c);

            return allowedChars.ToArray();
        }

        public static bool Traverse(JsonElement element, out string text, params string[] path)
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

                        if (pNum < 0 || pNum >= element.GetArrayLength())
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

        public static bool Traverse(JsonElement element, out double number, params string[] path)
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

                        if (pNum < 0 || pNum >= element.GetArrayLength())
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

        public static bool Traverse(JsonElement element, out int number, params string[] path)
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

                        if (pNum < 0 || pNum >= element.GetArrayLength())
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

        public static bool Traverse(JsonElement element, out long number, params string[] path)
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

                        if (pNum < 0 || pNum >= element.GetArrayLength())
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

            return element.TryGetInt64(out number);
        }

        public static bool Traverse(JsonElement element, out bool @bool, params string[] path)
        {
            int pNum;

            foreach (string p in path)
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Array:
                        if (!int.TryParse(p, out pNum))
                        {
                            @bool = false;
                            return false;
                        }

                        if (pNum < 0 || pNum >= element.GetArrayLength())
                        {
                            @bool = false;
                            return false;
                        }

                        element = element[pNum];
                        break;
                    case JsonValueKind.Object:
                        if (!element.TryGetProperty(p, out element))
                        {
                            @bool = false;
                            return false;
                        }
                        break;
                    default:
                        @bool = false;
                        return false;
                }
            }

            @bool = element.ValueKind == JsonValueKind.True;

            return element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False;
        }

        public static bool Traverse(JsonElement element, out JsonElement result, params string[] path)
        {
            int pNum;
            result = element;

            foreach (string p in path)
            {
                switch (result.ValueKind)
                {
                    case JsonValueKind.Array:
                        if (!int.TryParse(p, out pNum))
                            return false;

                        if (pNum < 0 || pNum >= result.GetArrayLength())
                            return false;

                        result = result[pNum];
                        break;
                    case JsonValueKind.Object:
                        if (!result.TryGetProperty(p, out result))
                            return false;

                        break;
                    default:
                        return false;
                }
            }

            return true;
        }

        public static bool Traverse(JsonElement element, out Vector vector, params string[] path)
        {
            int pNum;

            foreach (string p in path)
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Array:
                        if (!int.TryParse(p, out pNum))
                        {
                            vector = new Vector();
                            return false;
                        }

                        if (pNum < 0 || pNum >= element.GetArrayLength())
                        {
                            vector = new Vector();
                            return false;
                        }

                        element = element[pNum];
                        break;
                    case JsonValueKind.Object:
                        if (!element.TryGetProperty(p, out element))
                        {
                            vector = new Vector();
                            return false;
                        }
                        break;
                    default:
                        vector = new Vector();
                        return false;
                }
            }

            return Vector.TryParse(element, out vector);
        }
    }
}
