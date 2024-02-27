using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Connector
{
    internal class Utils
    {
        /// <summary>
        /// Checks the name. Returns only valid names. Throws a GameException with the code
        /// GameException.InvalidValue on invalid names.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>The name if valid.</returns>
        public static string CheckedName32OrThrowInvalidValue(string? name)
        {
            if (!CheckName32(name))
            {
                throw new GameException(GameException.InvalidValue, "Name has to consist of 2 to 32 valid characters.");
            }

            return name!;
        }

        /// <summary>
        /// Checks the name.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>truw if the name is proper, false if the name doesn't pass the check.</returns>
        public static bool CheckName32(string? name)
        {
            if (name is null || name.Length < 2 || name.Length > 32)
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

        /// <summary>
        /// Checks the message. Only valid messages are returned. Throws a GameException with the code
        /// GameException.InvalidValue on invalid names.
        /// </summary>
        /// <param name="message">The message to check.</param>
        /// <returns>The name if valid.</returns>
        public static string CheckMessageThrowInvalidValue(string? message)
        {
            if (!CheckMessage(message))
            {
                throw new GameException(GameException.InvalidValue,
                    "The message has to consist of 1 to 512 valid characters.");
            }

            return message!;
        }


        /// <summary>
        /// Checks the message. Only valid messages are returned. Throws a GameException with the code
        /// GameException.InvalidValue on invalid names.
        /// </summary>
        /// <param name="message">The message to check.</param>
        /// <returns>The name if valid.</returns>
        public static bool CheckMessage(string? message)
        {
            if (message is null || message.Length < 1 || message.Length > 512)
                return false;

            if (message.StartsWith(' ') || message.EndsWith(' '))
                return false;

            foreach (char c in message)
            {
                if (c >= 32 && c <= 126)
                    continue;

                if (c >= 192 && c <= 214)
                    continue;

                if (c >= 216 && c <= 246)
                    continue;

                if (c >= 248 && c <= 687)
                    continue;

                if (c == '€' || c == '‚' || c == '„' || c == '…' || c == '‰' || c == '‹' || c == '›' || c == '™' ||
                    c == '•' || c == '¢' || c == '£' || c == '¡' || c == '¤' || c == '¥' || c == '©' || c == '®' ||
                    c == '±' || c == '²' || c == '³' || c == 'µ' || c == '¿' || c == '«' || c == '»')
                    continue;

                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks the name. Returns only valid names. Throws a GameException with the code
        /// GameException.InvalidValue on invalid names.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>The name if valid.</returns>
        public static string CheckedName64OrThrowInvalidValue(string? name)
        {
            if (!CheckName64(name))
            {
                throw new GameException(GameException.InvalidValue, "Name has to consist of 2 to 64 valid characters.");
            }

            return name!;
        }


        /// <summary>
        /// Checks the name.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>truw if the name is proper, false if the name doesn't pass the check.</returns>
        public static bool CheckName64(string? name)
        {
            if (name is null || name.Length < 2 || name.Length > 64)
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

        public static bool Traverse(JsonElement element, out bool result, params string[] path)
        {
            int pNum;

            foreach (string p in path)
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Array:
                        if (!int.TryParse(p, out pNum))
                        {
                            result = default;
                            return false;
                        }

                        if (pNum < 0 || pNum >= element.GetArrayLength())
                        {
                            result = default;
                            return false;
                        }

                        element = element[pNum];
                        break;
                    case JsonValueKind.Object:
                        if (!element.TryGetProperty(p, out element))
                        {
                            result = default;
                            return false;
                        }

                        break;
                    default:
                        result = default;
                        return false;
                }
            }

            result = element.ValueKind == JsonValueKind.True;

            return element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False;
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
    }
}