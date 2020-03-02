using Flattiverse.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse
{
    /// <summary>
    /// A controllable is the control class which enables the control of a player controllable PlayerUnit.
    /// </summary>
    public class Controllable : UniversalEnumerable
    {
        private readonly string name;

        private readonly byte ID;

        private readonly Server server;

        private bool active;

        private float radius;
        private int energyMax;
        private float engineMax;
        private float thrusterMax;
        private int hullMax;
        private float scannerBroadMax;
        private Galaxy galaxy;
        private byte[] systems;

        private Vector position;
        private Vector movement;
        private int hull;
        private int energy;
        private float direction;
        private float engine;
        private float thruster;
        private bool scannerBroadEnabled;
        private float scannerBroadDirection;
        private float scannerBroadDestination;

        internal Controllable(Server server, Universe universe, Packet packet)
        {
            this.server = server;
            ID = packet.SubAddress;

            BinaryMemoryReader reader = packet.Read();

            name = reader.ReadString();

            updateStructural(universe, ref reader);
        }

        internal void updateStructural(Universe universe, ref BinaryMemoryReader reader)
        {
            radius = reader.ReadSingle();
            energyMax = reader.ReadUInt16();
            engineMax = reader.ReadSingle();
            thrusterMax = reader.ReadSingle();
            hullMax = reader.ReadByte();
            scannerBroadMax = reader.ReadUInt16();
            galaxy = universe.galaxies[reader.ReadByte()];
            systems = new byte[13];

            reader.ReadBytes(systems, 0, 13);
        }

        internal void updateRegular(ref BinaryMemoryReader reader)
        {
            position = new Vector(ref reader);
            movement = new Vector(ref reader);
            hull = reader.ReadByte();
            energy = reader.ReadUInt16();
            direction = reader.ReadSingle();
            engine = reader.ReadSingle();
            thruster = reader.ReadSingle();
            scannerBroadEnabled = reader.ReadBoolean();
            scannerBroadDirection = reader.ReadSingle();
            scannerBroadDestination = reader.ReadSingle();
        }

        /// <summary>
        /// True, if this unit is still registered to the game. This will change, when you get disconnected or if the unit gets disposed.
        /// </summary>
        public bool Active => active;

        /// <summary>
        /// true, if the ship is currently alive.
        /// </summary>
        public bool Alive => hull > 0;

        /// <summary>
        /// The maximum amount of energy the vessel has.
        /// </summary>
        public int EnergyMax => energyMax;

        /// <summary>
        /// The maximum amount of movement the main engine can do.
        /// </summary>
        public float EngineMax => engineMax;

        /// <summary>
        /// The absolute maximum the thrusters can accelerate the ship turning.
        /// </summary>
        public float ThrusterMax => thrusterMax;

        /// <summary>
        /// The maximum hull the ship has.
        /// </summary>
        public int HullMax => hullMax;

        /// <summary>
        /// The maximum range the broad scanner has.
        /// </summary>
        public float ScannerBroadMax => scannerBroadMax;

        /// <summary>
        /// The galaxy this controllable currently is in.
        /// </summary>
        public Galaxy Galaxy => galaxy;

        /// <summary>
        /// Generates a dictionary of ship systems out of stored raw data and the current system level of the corresponding systems.
        /// </summary>
        public Dictionary<UniverseSystemKind, int> Systems
        {
            get
            {
                Dictionary<UniverseSystemKind, int> systems = new Dictionary<UniverseSystemKind, int>(13);

                for (int systemKind = 0; systemKind < 13; systemKind++)
                    systems.Add((UniverseSystemKind)systemKind, this.systems[systemKind]);

                return systems;
            }
        }

        /// <summary>
        /// The position of the controllable.
        /// </summary>
        public Vector Position => new Vector(position);

        /// <summary>
        /// The movement of the controllable.
        /// </summary>
        public Vector Movement => new Vector(movement);

        /// <summary>
        /// The hull the current ship has.
        /// </summary>
        public int Hull => hull;

        /// <summary>
        /// The energy of the controllable.
        /// </summary>
        public int Energy => energy;

        /// <summary>
        /// The direction this controllable is turned to in degree.
        /// </summary>
        public float Direction => direction;

        /// <summary>
        /// The output of the engine. If this is 0 the ship is drifting. If this is 0.01 the ship is accelerating into the direction of "Direction".
        /// </summary>
        public float Engine => engine;

        /// <summary>
        /// The output of the thruster. If this is 0 the speed of how fast the ship is turning doesn't change. If this is -0.5 the ship is turning faster to the left each internal tick.
        /// </summary>
        public float Thruster => thruster;

        /// <summary>
        /// true, if the Scanner is enabled. false otherwise.
        /// </summary>
        public bool ScannerBroadEnabled => scannerBroadEnabled;

        /// <summary>
        /// The direction in which the scanner is currently looking.
        /// </summary>
        public float ScannerBroadDirection => scannerBroadDestination;

        /// <summary>
        /// The destination the scanner is turning to.
        /// </summary>
        public float ScannerBroadDestination => scannerBroadDestination;

        /// <summary>
        /// The name of the corresponding PlayerUnit.
        /// </summary>
        public string Name => name;

        internal void deactivate()
        {
            active = false;
        }

        /// <summary>
        /// Call this, if you want your ship to spawn.
        /// </summary>
        public async Task Continue()
        {
            using (Session session = server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0xB2;
                packet.SubAddress = ID;

                server.connection.Send(packet);
                server.connection.Flush();

                packet = await session.Wait().ConfigureAwait(false);
            }
        }
    }
}
