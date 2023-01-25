using System.Reflection;
using System.Text.Json;

namespace Flattiverse.Units
{
    public class Unit
    {
        public readonly string Name;

        public readonly Vector Position;

        public readonly double Radius;
        public readonly double Gravity;

        private static Dictionary<string, Type> unitClasses = initializeUnitClasses();
        private static Dictionary<string, Type> initializeUnitClasses()
        {
            Dictionary<string, Type> unitClasses = new Dictionary<string, Type>();
            Type? temp;
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                temp = t;

                while ((temp = temp.BaseType) != null)
                    if (temp == typeof(Unit))
                    {
                        unitClasses[t.Name.ToLower()] = t;
                        break;
                    }
            }
            return unitClasses;
        }

        internal Unit(JsonElement json) 
        {
            JsonElement subElement;

            if(!Utils.Traverse(json, out Name, false, "name"))
                throw new InvalidDataException($"Unit doesn't contain avalid name property.");

            if (!Utils.Traverse(json, out subElement, JsonValueKind.Object, "position"))
                throw new InvalidDataException("Unit doesn't contain a valid position property.");

            Position = new Vector(subElement);

            if (!Utils.Traverse(json, out Radius, "radius"))
                throw new InvalidDataException("Unit doesn't contain a valid radius property.");

            if (!Utils.Traverse(json, out Gravity, "gravity"))
                throw new InvalidDataException("Unit doesn't contain a valid gravity property.");
        }



        internal static Unit DeseializeJson(JsonElement element)
        {
            if(!Utils.Traverse(element, out string kind, false, "kind"))
                throw new InvalidDataException("Unit does not contain valid kind property.");

            Type? t;
            if (!unitClasses!.TryGetValue(kind, out t))
                throw new InvalidDataException($"Unknown Unit kind: \"{kind}\"");

            if (t.IsAbstract)
                throw new InvalidDataException($"Unit kind {kind} can't be instantiated");

            ConstructorInfo? ctor = t.GetConstructor(new[] { typeof(JsonElement) });

            if (ctor is null)
                throw new SystemException($"Unit {kind} has no valid Json Constructor");

            return (Unit)ctor.Invoke(new object[] { element });
        }

    }
}
