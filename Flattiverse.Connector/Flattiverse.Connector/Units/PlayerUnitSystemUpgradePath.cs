using Flattiverse.Connector.Events;

namespace Flattiverse.Connector.Units
{
    public class PlayerUnitSystemUpgradePath
    {
        public PlayerUnitSystemIdentifier? RequiredComponent;

        public PlayerUnitSystemKind Kind;
        public int Level;

        public double Energy;
        public double Particles;

        public double Iron;
        public double Carbon;
        public double Silicon;
        public double Platinum;
        public double Gold;

        public int Time;

        public double Value0;
        public double Value1;
        public double Value2;
    }
}
