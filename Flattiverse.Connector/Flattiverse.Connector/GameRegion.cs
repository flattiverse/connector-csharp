using System.Text.Json;

namespace Flattiverse.Connector
{
    public class GameRegion
    {
        public int ID;

        public ushort TeamMask;

        public string? Name;

        public bool StartLocation;
        public bool SafeZone;
        public bool SlowRestore;

        public Region Region;

        public GameRegion(JsonElement element)
        {
            Utils.Traverse(element, out ID, "regionId");
            Utils.Traverse(element, out int teamMask, "teams");
            TeamMask = (ushort)teamMask;
            Utils.Traverse(element, out Name, "name");
            Region = new Region(element);
            Utils.Traverse(element, out StartLocation, "startLocation");
            Utils.Traverse(element, out SafeZone, "safeZone");
            Utils.Traverse(element, out SlowRestore, "slowRestore");

        }

        public GameRegion(int iD, string? name, ushort teamMask, bool startLocation, bool safeZone, Region region)
        {
            ID = iD;
            Name = name;
            TeamMask = teamMask;
            StartLocation = startLocation;
            SafeZone = safeZone;
            Region = region;
        }
    }
}
