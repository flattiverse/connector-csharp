using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    /// <summary>
    /// A unit in the universe.
    /// </summary>
    public class Unit
    {
        private static readonly Dictionary<string, ConstructorInfo> routes = InitializeEventRoutes();

        /// <summary>
        /// The name of the unit.
        /// </summary>
        public string Name;

        /// <summary>
        /// The radius of the unit.
        /// </summary>
        public double Radius;

        /// <summary>
        /// The absolute position of the unit.
        /// </summary>
        public Vector Position;

        /// <summary>
        /// The movement vector of the unit.
        /// </summary>
        public Vector Movement;

        /// <summary>
        /// The direction the unit is facing towerds.
        /// </summary>
        public double Direction;

        /// <summary>
        /// The team this unit belongs to, if any.
        /// </summary>
        public Team? Team;

        /// <summary>
        /// The gravity exercised by this unit.
        /// </summary>
        public double Gravity;

        /// <summary>
        /// The mobility status of this unit.
        /// </summary>
        public Mobility Mobility;

        /// <summary>
        /// The energy output of this unit.
        /// </summary>
        public double EnergyOutput;

        /// <summary>
        /// Whether this unit is masking.
        /// </summary>
        public bool IsMasking;

        /// <summary>
        /// Whether this unit is solid.
        /// </summary>
        public bool IsSolid;

        /// <summary>
        /// Wheter it is possible to edit this unit via admin commands.
        /// </summary>
        public virtual bool MapEditable => false;

        /// <summary>
        /// The kind of this unit.
        /// </summary>
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

        /// <summary>
        /// Create a new unit.
        /// </summary>
        /// <param name="name">The name of the unit.</param>
        /// <param name="position">The position of the unit.</param>
        /// <param name="movement">The movement of the unit.</param>
        public Unit(string name, Vector position, Vector movement)
        {
            Name = name;
            Position = position;
            Movement = movement;
        }

        /// <summary>
        /// Create a new unit which has no initial movement.
        /// </summary>
        /// <param name="name">The name of the unit.</param>
        /// <param name="position">The position of the unit.</param>
        public Unit(string name, Vector position)
        {
            Name = name;
            Position = position;
            Movement = Vector.Null;
        }

        /// <summary>
        /// Creates a new unit which has no initial movement or position.
        /// </summary>
        /// <param name="name"></param>
        public Unit(string name)
        {
            Name = name;
            Position = Vector.Null;
            Movement = Vector.Null;
        }

        /// <summary>
        /// Create a new "Unnamed" unit which has no initial movement or position.
        /// </summary>
        public Unit()
        {
            Name = "Unnamed";
            Position = Vector.Null;
            Movement = Vector.Null;
        }

        internal static Unit CreateFromJson(UniverseGroup group, JsonElement element)
        {
            string kind;

            if (!Utils.Traverse(element, out kind, "kind"))
                throw new GameException(0xA8);

            ConstructorInfo? constructorInfo;

            if (!routes.TryGetValue(kind, out constructorInfo))
                throw new GameException(0xA9);

            return (Unit)constructorInfo.Invoke(new object[] { group, element });
        }

        internal Unit(UniverseGroup group, JsonElement element)
        {
            if (!Utils.Traverse(element, out Name, "name") || !Utils.Traverse(element, out Radius, "radius") || !Utils.Traverse(element, out Position, "position"))
                throw new GameException(0xA0);

            Utils.Traverse(element, out Gravity, "gravity");
            Utils.Traverse(element, out Movement, "movement");

            if (Utils.Traverse(element, out int teamID, "team") && teamID >= 0 && teamID < 16)
                Team = group.teams[teamID];
        }
    }
}