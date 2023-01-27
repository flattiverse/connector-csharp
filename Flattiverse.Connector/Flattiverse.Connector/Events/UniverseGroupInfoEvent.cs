using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event notifies about the meta informations a UniverseGroup has, like Name, Description, Teams, Rules...
    /// You actually don't need to parse this event because it's also parsed by the connector and the results are
    /// presented in fields on the UniverseGroup.
    /// </summary>
    [FlattiverseEventIdentifier("universeGroupInfo")]
    public class UniverseGroupInfoEvent : FlattiverseEvent
    {
        /// <summary>
        /// The name of the UniverseGroup.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The Descrition of the UniverseGroup.
        /// </summary>
        public readonly string Description;
        
        /// <summary>
        /// The amount of max Players together in the UniverseGroup.
        /// </summary>
        public readonly int MaxPlayers;

        internal UniverseGroupInfoEvent(JsonElement element)
        {
            // TOG: Hier aus dem element parsen, bitte.
        }

        internal override void Process(UniverseGroup group)
        {
            group.name = Name;
            group.description = Description;
            group.maxPlayers = MaxPlayers;

            // TOG
        }

        /// <summary>
        /// Specifies the kind of the event for a better switch() experience.
        /// </summary>
        public override EventKind Kind => EventKind.UniverseGroupInfo;
    }
}
