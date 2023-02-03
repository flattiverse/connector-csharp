using System.Text.Json;

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
        /// The GameMode of the UniverseGroup.
        /// </summary>
        public readonly GameMode Mode;

        /// <summary>
        /// The amount of players together in the UniverseGroup.
        /// </summary>
        public readonly int MaxPlayers;

        /// <summary>
        /// The amount of ships a player can have in the UniverseGroup.
        /// </summary>
        public readonly int MaxShipsPerPlayer;

        /// <summary>
        /// The amount of ships a team can have in the UniverseGroup.
        /// </summary>
        public readonly int MaxShipsPerTeam;

        /// <summary>
        /// The amount of bases a player can have in the UniverseGroup.
        /// </summary>
        public readonly int MaxBasesPerPlayer;

        /// <summary>
        /// The amount of bases a team can have in the UniverseGroup.
        /// </summary>
        public readonly int MaxBasesPerTeam;

        /// <summary>
        /// True, if joining this universe as a spectator is allowed.
        /// </summary>
        public readonly bool Spectators;

        /// <summary>
        /// The teams in the UniverseGroup.
        /// </summary>
        public readonly IReadOnlyCollection<Team> Teams;

        private Team[] teams;

        /// <summary>
        /// The universes in the UniverseGroup.
        /// </summary>
        public readonly IReadOnlyCollection<Universe> Universes;

        private Universe[] universes;
        
        internal UniverseGroupInfoEvent(UniverseGroup group, JsonElement element) : base()
        {
            Utils.Traverse(element, out Name, "name");
            Utils.Traverse(element, out Description, "description");
            Utils.Traverse(element, out string mode, "mode");
            Enum.TryParse(mode, true, out Mode);

            Utils.Traverse(element, out MaxPlayers, "metrics", "maxPlayers");
            Utils.Traverse(element, out MaxShipsPerPlayer, "metrics", "maxShipsPerPlayer");
            Utils.Traverse(element, out MaxShipsPerTeam, "metrics", "maxShipsPerTeam");
            Utils.Traverse(element, out MaxBasesPerPlayer, "metrics", "maxBasesPerPlayer");
            Utils.Traverse(element, out MaxBasesPerTeam, "metrics", "maxBasesPerTeam");
            Utils.Traverse(element, out Spectators, "metrics", "spectators");

            Utils.Traverse(element, out JsonElement teamsArray, "teams");
            teams = new Team[16];
            foreach (JsonElement teamObject in teamsArray.EnumerateArray())
            {
                Team team = new Team(teamObject);
                teams[team.ID] = team;
            }
            Teams = teams;

            Utils.Traverse(element, out JsonElement universesArray, "universes");
            universes = new Universe[64];
            foreach (JsonElement universeObject in universesArray.EnumerateArray())
            {
                Universe universe = new Universe(group, universeObject);
                universes[universe.ID] = universe;
            }
            Universes = universes;
        }

        /// <summary>
        /// Specifies the kind of the event for a better switch() experience.
        /// </summary>
        public override EventKind Kind => EventKind.UniverseGroupInfo;
    }
}