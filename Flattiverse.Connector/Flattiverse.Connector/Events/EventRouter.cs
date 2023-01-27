using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Events
{
    static class EventRouter
    {
        private static Dictionary<string, ConstructorInfo> routes = InitializeEventRoutes();

        private static Dictionary<string, ConstructorInfo> InitializeEventRoutes()
        {
            Type[] jsonElement = new Type[] { typeof(JsonElement) };

            Dictionary<string, ConstructorInfo> routes = new Dictionary<string, ConstructorInfo>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                FlattiverseEventIdentifier? eventIdentifier = type.GetCustomAttribute<FlattiverseEventIdentifier>(false);

                if (eventIdentifier != null)
                {
                    Debug.Assert(!routes.ContainsKey(eventIdentifier.Identifier), $"Identifier \"{eventIdentifier.Identifier}\" already mapped.");
                    Debug.Assert(type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, jsonElement) != null, $"Class of identifier \"{eventIdentifier.Identifier}\" doesn't have a matching constructor.");
                    
                    routes.Add(eventIdentifier.Identifier, type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, jsonElement)!);
                }
            }

            return routes;
        }

        public static FlattiverseEvent CreateFromJson(JsonElement element)
        {
            string kind;

            if (!Utils.Traverse(element, out kind, "kind"))
                return new RawEvent(element);

            ConstructorInfo? constructorInfo;

            if (!routes.TryGetValue(kind, out constructorInfo))
                return new RawEvent(element);

            try
            {
                return (FlattiverseEvent)constructorInfo.Invoke(new object[] { element });
            }
            catch
            {
                return new RawEvent(element);
            }
        }
    }
}
