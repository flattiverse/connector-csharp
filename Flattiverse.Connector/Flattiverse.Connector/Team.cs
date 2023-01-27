using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public Team(JsonElement element)
        {
            Utils.Traverse(element, out ID, "id");
            Utils.Traverse(element, out Name, "name");
            Utils.Traverse(element, out R, "r");
            Utils.Traverse(element, out G, "g");
            Utils.Traverse(element, out B, "b");
        }
    }
}