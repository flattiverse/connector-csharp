using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// The base class for FlattiverseEvents.
    /// </summary>
    public class FlattiverseEvent
    {
        /// <summary>
        /// The timestamp when this event has been received or generated.
        /// </summary>
        public readonly DateTime Stamp;

        internal FlattiverseEvent()
        {
            Stamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Specifies the kind of the event for a better switch() experience.
        /// </summary>
        public virtual EventKind Kind => throw new NotImplementedException("Kind must be overwritten.");

        internal static FlattiverseEvent FromPacketReader(Galaxy galaxy, PacketHeader header, PacketReader reader)
        {
            switch((EventKind)header.Param0)
            {
                //case EventKind.UnitAdded:
                //    return new AddedUnitEvent(galaxy, Unit.FromPacketReader(galaxy, reader));

                //case EventKind.UnitUpdated:
                //    return new UpdatedUnitEvent(galaxy, Unit.FromPacketReader(galaxy, reader));

                //case EventKind.UnitVanished:
                //    return new VanishedUnitEvent(galaxy, reader.ReadUInt32());

                //case EventKind.PlayerAdded:
                //    return new PlayerAddedEvent(galaxy, Player.FromPacketReader(galaxy, reader));

                //case EventKind.PlayerRemoved:
                //    return new PlayerRemovedEvent(galaxy, Player.FromPacketReader(galaxy, reader));

                default:
                    throw new NotImplementedException($"The event kind {(EventKind)header.Param0} is not implemented.");
            }
        }
    }
}
