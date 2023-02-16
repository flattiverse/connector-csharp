using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    [UnitIdentifier("playerUnit")]
    public class PlayerUnit : MobileUnit
    {
        public Player Player;
        public Controllable? Controllable;

        public double TurnRate;
        public double ScanDirection;
        public double ScanWidth;
        public double ScanRange;
        public bool ScanActivated;

        public double Hull;
        public double HullMax;
        public double Nozzle;
        public double NozzleMax;
        public double Thruster;
        public double ThrusterMax;
        public double Armor;
        public double ArmorMax;
        public double Shield;
        public double ShieldMax;
        public double Energy;
        public double EnergyMax;
        public double Particles;
        public double ParticlesMax;
        public double CargoLoad;

        public Dictionary<PlayerUnitSystemKind, int> SystemLevels;


        public PlayerUnit()
        {
        }

        public PlayerUnit(string name) : base(name)
        {
        }

        public PlayerUnit(string name, Vector position) : base(name, position)
        {
        }

        public PlayerUnit(string name, Vector position, Vector movement) : base(name, position, movement)
        {
        }

        internal PlayerUnit(UniverseGroup group, JsonElement element) : base(group, element)
        {
            Utils.Traverse(element, out int playerId, "player");
            if (group.playersId[playerId] is null)
                group.connection.PushFailureEvent($"Tried to instantiate a PlayerUnit for a player that does not exist in the universe.");
            else
                Player = group.playersId[playerId];

            Utils.Traverse(element, out Direction, "direction");
            Utils.Traverse(element, out int controllableID, "controllable");
            if (Player?.ID == group.Player.ID)
                group.TryGetControllable(controllableID, out Controllable);
            Utils.Traverse(element, out TurnRate, "turnRate");
            Utils.Traverse(element, out ScanDirection, "scanDirection");
            Utils.Traverse(element, out ScanWidth, "scanWidth");
            Utils.Traverse(element, out ScanRange, "scanRange");
            Utils.Traverse(element, out ScanActivated, "scanActivated");

            Utils.Traverse(element, out Hull, "hull");
            Utils.Traverse(element, out HullMax, "hullMax");
            Utils.Traverse(element, out Nozzle, "nozzle");
            Utils.Traverse(element, out NozzleMax, "nozzleMax");
            Utils.Traverse(element, out Thruster, "thruster");
            Utils.Traverse(element, out ThrusterMax, "thrusterMax");
            Utils.Traverse(element, out Armor, "armor");
            Utils.Traverse(element, out ArmorMax, "armorMax");
            Utils.Traverse(element, out Shield, "shield");
            Utils.Traverse(element, out ShieldMax, "shieldMax");
            Utils.Traverse(element, out Energy, "energy");
            Utils.Traverse(element, out EnergyMax, "energyMax");
            Utils.Traverse(element, out Particles, "particles");
            Utils.Traverse(element, out ParticlesMax, "particlesMax");
            Utils.Traverse(element, out CargoLoad, "cargoLoad");

            Utils.Traverse(element, out JsonElement systems, "systemLevels");
            SystemLevels = new Dictionary<PlayerUnitSystemKind, int>();
            foreach (JsonProperty system in systems.EnumerateObject())
            {
                if (!Enum.TryParse(system.Name, true, out PlayerUnitSystemKind systemKind))
                    group.connection.PushFailureEvent($"Invalid system kind received.");
                else
                    SystemLevels.Add(systemKind, system.Value.GetInt32());
            }
        }

        public override UnitKind Kind => UnitKind.PlayerUnit;
    }
}