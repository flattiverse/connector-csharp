using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event informs of the disconnect of a player from the universeGroup.
    /// </summary>
    [FlattiverseEventIdentifier("chatTeamcast")]
    public class ChatTeamcastEvent : ChatEvent
    {
        public readonly Team Team;

        internal ChatTeamcastEvent(UniverseGroup group, JsonElement element) : base(group, element)
        {
            Utils.Traverse(element, out int teamID, "destination");
            Team = group.teams[teamID];
        }

        public override EventKind Kind => EventKind.ChatTeamcast;
    }
}