using System.Collections.ObjectModel;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.UnitConfigurations
{
    public class BuoyConfiguration : CelestialBodyConfiguration
    {
        private string message;
        private List<Vector> beacons;

        private BuoyConfiguration() : base()
        {
            message = string.Empty;
            beacons = new List<Vector>(0);
        }

        internal BuoyConfiguration(PacketReader reader) : base(reader)
        {
            message = Utils.CheckMessageThrowInvalidValue(reader.ReadString());

            byte size = reader.ReadByte();
            
            beacons = new List<Vector>(size);
            
            for (int i = 0; i < size; i++)
                beacons.Add(new Vector(reader));
        }

        internal override void Write(PacketWriter writer)
        {
            base.Write(writer);

            writer.Write(message);
            writer.Write((byte)beacons.Count);
            foreach (Vector vector in beacons)
            {
                vector.Write(writer);
            }
        }

        public override UnitKind Kind => UnitKind.Buoy;
        internal static BuoyConfiguration Default => new BuoyConfiguration();
        
        /// <summary>
        /// The message of the buoy unit.
        /// </summary>
        /// <exception cref="GameException">GameException.InvalidValue may be thrown, if the name violates rules.</exception>
        public String Message
        {
            get => message;
            set => message = Utils.CheckedName64OrThrowInvalidValue(value);
        }

    
        /// <summary>
        /// Beacons of the Buoy unit. Beacons are locations relative to this Buoy for which the space in between is of
        /// interest. 
        /// </summary>
        public ReadOnlyCollection<Vector> Beacons =>
            new ReadOnlyCollection<Vector>(beacons);


        /// <summary>
        /// Adds a new beacon location to this Buoy unit.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="GameException">Thrown if the the Buoy unit is full and cannot house any more beacons.</exception>
        public Vector AddBeacon()
        {
            if (beacons.Count >= 16)
            {
                throw new GameException(0x32);
            }

            Vector vector = new Vector();
        
            beacons.Add(vector);
        
            return vector;
        }

        /// <summary>
        /// Removes all Beacons from this configuration.
        /// </summary>
        public void ClearBeacons()
        {
            beacons.Clear();
        }
    }
}
