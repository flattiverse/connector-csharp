using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event informs of the update of a controllable in the universeGroup.
    /// </summary>
    [FlattiverseEventIdentifier("controllableUpdated")]
    public class FullUpdateControllableEvent : UnitEvent
    {
        public readonly Controllable Controllable;

        internal FullUpdateControllableEvent(UniverseGroup group, JsonElement element) : base(group, element)
        {
            Utils.Traverse(element, out JsonElement controllable, "controllable");
            Utils.Traverse(element, out int controllableID, "controllableID");

            // TOG: Hier bitte eine zwischenvariable machen, die auch tatsächlich null sein kann. Sonst ist Dein Controllable? ziemlich sus.
            if (!group.TryGetControllable(controllableID, out Controllable))
                group.connection.PushFailureEvent($"Couldn't get the controllable with the id {controllableID}.");

            Controllable?.update(group, controllable);
        }

        public override EventKind Kind => EventKind.ControllableUpdated;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} CONUP Controllable {Controllable.Name} was updated.";
        }
    }
}