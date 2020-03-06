using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// The scores of a player.
    /// </summary>
    public class Scores
    {
        private int kills;
        private int deaths;
        private int crashes;
        private int mission;

        internal Scores()
        {
        }

        internal void update(BinaryMemoryReader reader)
        {
            kills = reader.ReadUInt24();
            deaths = reader.ReadUInt24();
            crashes = reader.ReadUInt24();
            mission = reader.ReadUInt24();
        }

        internal void clear()
        {
            kills = 0;
            deaths = 0;
            crashes = 0;
            mission = 0;
        }

        /// <summary>
        /// The kills of the player.
        /// </summary>
        public int Kills => kills;

        /// <summary>
        /// The deaths of a player.
        /// </summary>
        public int Deaths => deaths;

        /// <summary>
        /// The crashes of the player.
        /// </summary>
        public int Crashes => crashes;

        /// <summary>
        /// The mission score of the player.
        /// </summary>
        public int Mission => mission;
    }
}
