using Flattiverse.Connector.Units;
using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event informs of the untimely demise of a controllable in the universeGroup.
    /// </summary>
    [FlattiverseEventIdentifier("controllableDeath")]
    public class DeathControllableEvent : UnitEvent
    {
        public readonly Controllable Controllable;
        public readonly UnitKind CauserKind;
        public readonly string CauserName;
        public readonly DeathReason Reason;


        internal DeathControllableEvent(UniverseGroup group, JsonElement element) : base(group, element)
        {
            Utils.Traverse(element, out int controllableID, "controllableID");
            if (!group.TryGetControllable(controllableID, out Controllable))
                group.connection.PushFailureEvent($"Couldn't get the controllable with the id {controllableID}.");

            Utils.Traverse(element, out CauserName, "causerName");

            Utils.Traverse(element, out string causerKind, "causerKind");
            if (!Enum.TryParse(causerKind, true, out CauserKind))
                group.connection.PushFailureEvent($"Couldn't parse causerKind {causerKind}.");

            Utils.Traverse(element, out string reason, "reason");
            if (!Enum.TryParse(reason, true, out Reason))
                group.connection.PushFailureEvent($"Couldn't parse reason {reason}.");
        }

        public override EventKind Kind => EventKind.ControllableDeath;
    }
}