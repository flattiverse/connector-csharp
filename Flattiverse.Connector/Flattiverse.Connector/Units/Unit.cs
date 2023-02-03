using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    /// <summary>
    /// TOG: Überall XML-Kommentare.
    /// </summary>
    public class Unit
    {
        private static readonly Dictionary<string, ConstructorInfo> routes = InitializeEventRoutes();

        public string Name;

        public double Radius;

        public Vector Position;

        public Vector Movement;

        public double Direction;

        public Team? Team;

        public double Gravity;

        public Mobility Mobility;

        public double EnergyOutput;

        public bool IsMasking;

        public bool IsSolid;

        public virtual bool MapEditable => false;

        public virtual UnitKind Kind => throw new NotImplementedException("Somebody fucked up...");

        private static Dictionary<string, ConstructorInfo> InitializeEventRoutes()
        {
            Type[] jsonElement = new Type[] { typeof(UniverseGroup), typeof(JsonElement) };

            Dictionary<string, ConstructorInfo> routes = new Dictionary<string, ConstructorInfo>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                UnitIdentifier? eventIdentifier = type.GetCustomAttribute<UnitIdentifier>(false);

                if (eventIdentifier != null)
                {
                    Debug.Assert(!routes.ContainsKey(eventIdentifier.Identifier), $"Identifier \"{eventIdentifier.Identifier}\" already mapped.");
                    Debug.Assert(type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, jsonElement) != null, $"Class of identifier \"{eventIdentifier.Identifier}\" doesn't have a matching constructor.");

                    routes.Add(eventIdentifier.Identifier, type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, jsonElement)!);
                }
            }

            return routes;
        }

        public Unit(string name, Vector position, Vector movement)
        {
            Name = name;
            Position = position;
            Movement = movement;
        }

        public Unit(string name, Vector position)
        {
            Name = name;
            Position = position;
            Movement = Vector.Null;
        }

        public Unit(string name)
        {
            Name = name;
            Position = Vector.Null;
            Movement = Vector.Null;
        }

        public Unit()
        {
            Name = "Unnamed";
            Position = Vector.Null;
            Movement = Vector.Null;
        }

        internal static Unit CreateFromJson(JsonElement element)
        {
            string kind;

            if (!Utils.Traverse(element, out kind, "kind"))
                throw new GameException(0xA8);

            ConstructorInfo? constructorInfo;

            if (!routes.TryGetValue(kind, out constructorInfo))
                throw new GameException(0xA9);

            return (Unit)constructorInfo.Invoke(new object[] { element });
        }

        internal Unit(UniverseGroup group, JsonElement element)
        {
            if (!Utils.Traverse(element, out Name, "name") || !Utils.Traverse(element, out Radius, "setRadius") || !Utils.Traverse(element, out Position, "setPosition"))
                throw new GameException(0xA0);

            Utils.Traverse(element, out Gravity, "gravity");
            Utils.Traverse(element, out Movement, "movement");

            if (Utils.Traverse(element, out int teamID, "team"))
            {
                throw new NotImplementedException("tog");
                //if (UniverseGroup.Teams.Length <= teamID || UniverseGroup.This.Teams[teamID] == null)
                //    throw new GameException(0xA4);

                //team = UniverseGroup.This.Teams[teamID];
            }
        }
    }
}