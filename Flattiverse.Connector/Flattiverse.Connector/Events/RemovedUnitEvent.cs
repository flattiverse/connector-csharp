using Flattiverse.Connector.Accounts;
using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event informs of the removal of a unit from the universeGroup.
    /// </summary>
    [FlattiverseEventIdentifier("unitRemoved")]
    public class RemovedUnitEvent : UnitEvent
    {
        /// <summary>
        /// The name of the unit, representing his slot in the universegroup's player array.
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// The player that controls the unit, if applicable.
        /// </summary>
        public readonly Player? Player;
        /// <summary>
        /// The controllable of the player, if applicable.
        /// </summary>
        public readonly int Controllable;

        internal RemovedUnitEvent(UniverseGroup group, JsonElement element) : base(group, element)
        {
            Utils.Traverse(element, out Name, "name");
            if (Utils.Traverse(element, out int playerID, "player"))
                Player = group.players[playerID];
            if (!Utils.Traverse(element, out Controllable, "controllable"))
                Controllable = -1;
        }

        public override EventKind Kind => EventKind.UnitRemoved;
    }
}