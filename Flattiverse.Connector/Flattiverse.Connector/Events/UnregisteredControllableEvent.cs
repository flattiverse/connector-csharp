using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

            // TOG: Hier ein Beispiel, wie das Controllable "informiert" werden kann.
            Controllable.updateUnregistered();
        }

        public override EventKind Kind => EventKind.ControllableDeath;
    }
}
