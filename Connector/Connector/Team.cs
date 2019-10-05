using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Represents a team in an universe.
    /// </summary>
    public class Team : UniversalEnumerable
    {
        /// <summary>
        /// The universe this team belongs to.
        /// </summary>
        public readonly Universe Universe;

        /// <summary>
        /// The ID of this Team in the Universe.
        /// </summary>
        public readonly byte ID;

        private string name;

        private float r;
        private float g;
        private float b;

        internal Team(Universe universe, Packet packet)
        {
            Universe = universe;
            ID = packet.SubAddress;

            updateFromPacket(packet);
        }

        internal void updateFromPacket(Packet packet)
        {
            BinaryMemoryReader reader = packet.Read();

            name = reader.ReadString();

            r = reader.ReadSingle();
            g = reader.ReadSingle();
            b = reader.ReadSingle();
        }

        /// <summary>
        /// The name of the team.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// The red color component from 0f to 1f.
        /// </summary>
        public float R => r;
        
        /// <summary>
        /// The green color component from 0f to 1f.
        /// </summary>
        public float G => g;

        /// <summary>
        /// The blue color component from 0f to 1f.
        /// </summary>
        public float B => b;

        /// <summary>
        /// Translates the color to the next console color according to DOS colors.
        /// There may be a slightly mismatch depending on the colormapping configured.
        /// </summary>
        public ConsoleColor ConsoleColor
        {
            get
            {
                float nearestDistance = 9f;
                ConsoleColor color = ConsoleColor.White;

                foreach (float[] colorDef in new float[][] { new float[] { 0.5f, 0f, 0f, 0f },
                                                             new float[] { 1.5f, 0f, 0f, 0.5f },
                                                             new float[] { 2.5f, 0f, 0.5f, 0f },
                                                             new float[] { 3.5f, 0f, 0.5f, 0.5f },
                                                             new float[] { 4.5f, 0.5f, 0f, 0f },
                                                             new float[] { 5.5f, 0.5f, 0f, 0.5f },
                                                             new float[] { 6.5f, 0.5f, 0.5f, 0f },
                                                             new float[] { 7.5f, 0.666f, 0.666f, 0.666f },
                                                             new float[] { 8.5f, 0.333f, 0.333f, 0.333f },
                                                             new float[] { 9.5f, 0f, 0f, 1f },
                                                             new float[] { 10.5f, 0f, 1f, 0f },
                                                             new float[] { 11.5f, 0f, 1f, 1f },
                                                             new float[] { 12.5f, 1f, 0f, 0f },
                                                             new float[] { 13.5f, 1f, 0f, 1f },
                                                             new float[] { 14.5f, 1f, 1f, 0f },
                                                             new float[] { 15.5f, 1f, 1f, 1f }})
                {
                    float dist = (colorDef[1] - r) * (colorDef[1] - r) + (colorDef[2] - g) * (colorDef[2] - g) + (colorDef[3] - b) * (colorDef[3] - b);

                    if (dist < nearestDistance)
                    {
                        nearestDistance = dist;

                        color = (ConsoleColor)(int)colorDef[0];
                    }
                }

                return color;
            }
        }
    }
}
