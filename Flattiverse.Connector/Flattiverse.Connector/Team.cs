using Flattiverse.Connector.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Flattiverse.Connector
{
    /// <summary>
    /// This is a team of Players in a UniverseGroup. Players of the same Team are not necessarily friends, depending on the GameMode.
    /// </summary>
    public class Team
    {
        /// <summary>
        /// The ID of the team.
        /// </summary>
        public readonly int ID;

        /// <summary>
        /// The name of the team.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The red value of the team's color.
        /// </summary>
        public readonly double R;

        /// <summary>
        /// The green value of the team's color.
        /// </summary>
        public readonly double G;

        /// <summary>
        /// The blue value of the team's color.
        /// </summary>
        public readonly double B;

        public readonly UniverseGroup Group;

        public Team(UniverseGroup group, JsonElement element)
        {
            Group = group;

            Utils.Traverse(element, out ID, "id");
            Utils.Traverse(element, out Name, "name");
            Utils.Traverse(element, out R, "r");
            Utils.Traverse(element, out G, "g");
            Utils.Traverse(element, out B, "b");
        }

        public async Task Chat(string message)
        {
            if (!Utils.CheckMessage(message))
                throw new GameException(0xB5);

            using (Query query = Group.connection.Query("chatTeamcast"))
            {
                query.Write("team", ID);

                query.Write("message", message);

                await query.Send().ConfigureAwait(false);

                await query.Wait().ConfigureAwait(false);
            }
        }
    }
}