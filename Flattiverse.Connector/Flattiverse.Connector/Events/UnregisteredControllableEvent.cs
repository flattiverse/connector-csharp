using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    [FlattiverseEventIdentifier("controllableUnregistered")]
    public class UnregisteredControllableEvent : UnitEvent
    {
        public readonly Controllable Controllable;

        internal UnregisteredControllableEvent(UniverseGroup group, JsonElement element) : base(group, element)
        {
            Utils.Traverse(element, out int controllableID, "controllableID");

            if (!group.TryGetControllable(controllableID, out Controllable))
            {
                group.connection.PushFailureEvent($"Couldn't get the controllable with the id {controllableID}.");

                return;
            }

            Controllable.updateUnregistered();
        }

        public override EventKind Kind => EventKind.ControllableUnregistered;
    }
}