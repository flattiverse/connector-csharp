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
        private float energyMax;
        private float engineMax;
        private float thrusterMax;
        private float hullMax;
        private float scannerBroadMax;
        private Galaxy galaxy;
        private byte[] systems;

        private Vector position;
        private Vector movement;
        private float hull;
        private float energy;
        private float direction;
        private float rotation;
        private float engine;
        private float thruster;
        private bool scannerBroadEnabled;
        private float scannerBroadDirection;
        private float scannerBroadDestination;

        internal Controllable(Server server, Universe universe, Packet packet)
        {
            this.server = server;
            ID = packet.SubAddress;

            active = true;

            BinaryMemoryReader reader = packet.Read();

            name = reader.ReadString();

            updateStructural(universe, ref reader);
        }

        internal void updateStructural(Universe universe, ref BinaryMemoryReader reader)
        {
            radius = reader.ReadSingle();
            energyMax = reader.ReadSingle();
            engineMax = reader.ReadSingle();
            thrusterMax = reader.ReadSingle();
            hullMax = reader.ReadSingle();
            scannerBroadMax = reader.ReadUInt16();
            galaxy = universe.galaxies[reader.ReadByte()];
            systems = new byte[13];

            reader.ReadBytes(systems, 0, 13);
        }

        internal void updateRegular(ref BinaryMemoryReader reader)
        {
            position = new Vector(ref reader);
            movement = new Vector(ref reader);
            hull = reader.ReadSingle();
            energy = reader.ReadSingle();
            direction = reader.ReadSingle();
            rotation = reader.ReadSingle();
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
        /// The size of the controllable.
        /// </summary>
        public float Radius => radius;

        /// <summary>
        /// The maximum amount of energy the vessel has.
        /// </summary>
        public float EnergyMax => energyMax;

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
        public float HullMax => hullMax;

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
        public float Hull => hull;

        /// <summary>
        /// The energy of the controllable.
        /// </summary>
        public float Energy => energy;

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
        /// The rotation speed of the unit. (Every heartbeat: Direction += Rotation.)
        /// </summary>
        public float Rotation => rotation;

        /// <summary>
        /// true, if the Scanner is enabled. false otherwise.
        /// </summary>
        public bool ScannerBroadEnabled => scannerBroadEnabled;

        /// <summary>
        /// The direction in which the scanner is currently looking.
        /// </summary>
        public float ScannerBroadDirection => scannerBroadDirection;

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

                await session.Wait().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Ends the current unit. This will take up to 2 seconds, if your unit is alive.
        /// </summary>
        public async Task Close()
        {
            using (Session session = server.connection.NewSession())
            {
                Packet packet = session.Request;

                packet.Command = 0xB1;
                packet.SubAddress = ID;

                server.connection.Send(packet);
                server.connection.Flush();

                await session.Wait().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sets the thrusters to the value. Use 0 here to stop accelerating the rotation into one specific direction. In general:
        /// * The rotation of the ship is changed every heartbeat by: rotation += thrusters.
        /// * The direction of the ship is changed every heartbeat by: direction += rotation.
        /// </summary>
        /// <param name="direction">The direction in which the thrusters should accelerate the ship.</param>
        /// <remarks>This method transmitts the request to the server without awaiting acknowledgement. The controllable will be updated with the next heartbeat.</remarks>
        public void SetThrusters(float direction)
        {
            if (float.IsNaN(direction) || float.IsInfinity(direction))
                throw new ArgumentException("direction needs to be a real number.", nameof(direction));

            Packet packet = new Packet();

            packet.Command = 0xB4;
            packet.SubAddress = ID;

            packet.Write().Write(direction);

            server.connection.Send(packet);
            server.connection.Flush();
        }

        /// <summary>
        /// Sets the engine output to the given value. Use 0 here to stop engine output. In general:
        /// * Movement += Vector of direction direction with length of engine every heartbeat.
        /// </summary>
        /// <param name="engine">The amount of acceleration which the engine should "produce".</param>
        /// <remarks>This method transmitts the request to the server without awaiting acknowledgement. The controllable will be updated with the next heartbeat.</remarks>
        public void SetEngine(float engine)
        {
            if (float.IsNaN(engine) || float.IsInfinity(engine) || engine < 0f)
                throw new ArgumentException("engine needs to be a positive real number.", nameof(engine));

            Packet packet = new Packet();

            packet.Command = 0xB5;
            packet.SubAddress = ID;

            packet.Write().Write(engine);

            server.connection.Send(packet);
            server.connection.Flush();
        }
    }
}
