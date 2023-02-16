using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// Notifies about the depletion and possibly overuse of a resource of your controllable.
    /// </summary>
    [FlattiverseEventIdentifier("resourceDepleted")]
    public class DepletedResourceEvent : UnitEvent
    {
        public readonly Controllable Controllable;
        public readonly double EnergyOveruse;
        public readonly double ParticleOveruse;

        internal DepletedResourceEvent(UniverseGroup group, JsonElement element) : base(group, element)
        {
            Utils.Traverse(element, out int controllableID, "controllableID");
            if (!group.TryGetControllable(controllableID, out Controllable))
            {
                group.connection.PushFailureEvent($"Couldn't get the controllable with the id {controllableID}.");

                return;
            }

            Utils.Traverse(element, out EnergyOveruse, "energyOveruse");
            Utils.Traverse(element, out ParticleOveruse, "particleOveruse");
        }

        public override EventKind Kind => EventKind.ControllableDeath;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} DPLTD Your controllable {Controllable.Name} has depleted a resource: There was a deficit of {EnergyOveruse} energy and {ParticleOveruse} particles.";
        }
    }
}