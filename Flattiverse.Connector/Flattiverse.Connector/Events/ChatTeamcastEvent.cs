using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event informs of a chatmessage to a team.
    /// </summary>
    [FlattiverseEventIdentifier("chatTeamcast")]
    public class ChatTeamcastEvent : ChatEvent
    {
        public readonly Team Team;

        internal ChatTeamcastEvent(UniverseGroup group, JsonElement element) : base(group, element)
        {
            Utils.Traverse(element, out int teamID, "destination");
            Team = group.teamsId[teamID];
        }

        public override EventKind Kind => EventKind.ChatTeamcast;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} MSG4T Player {Source.Name} has sent a message to your team: \"{Message}\".";
        }
    }
}