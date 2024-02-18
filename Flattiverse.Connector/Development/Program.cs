using System.Runtime.CompilerServices;
using Flattiverse.Connector;
using Flattiverse.Connector.Hierarchy;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // A new beginning and a test - again.

        Universe universe = new Universe();

        Console.WriteLine("Starting join request");

        // Admin key / local host
        //Galaxy galaxy = await universe.Join("ws://127.0.0.1:5000", "68B59217071450554F85DB121F89EC31C54E427D04A8EE16AC72C52AED806631", 0x00);

        // Player key / online
        //Galaxy galaxy = await universe.Join("ws://www.flattiverse.com/game/galaxies/0", "CE43AE41B96111DB66D75AB943A3042755B98F10E6A09AF0D4190B0FFEC13EE8", 0x00);
        Galaxy galaxy = await universe.Join("ws://127.0.0.1:5000", "68B59217071450554F85DB121F89EC31C54E427D04A8EE16AC72C52AED806631", 0x00);

        Console.WriteLine("Finished join request");

        //await galaxy.SendMessage(1, "This is a message.");

        try
        {
            Console.WriteLine($"Original galaxy {galaxy.Name} #{galaxy.ID}: {galaxy.Config.Description}");

            await galaxy.Configure(config =>
            {
                config.Description = "An updated description.";
                config.GameType = GameType.Domination;
                config.MaxPlayers = 1;
                config.MaxPlatformsUniverse = 2;
                config.MaxProbesUniverse = 3;
                config.MaxDronesUniverse = 4;
                config.MaxShipsUniverse = 5;
                config.MaxBasesUniverse = 6;
                config.MaxPlatformsTeam = 7;
                config.MaxProbesTeam = 8;
                config.MaxDronesTeam = 9;
                config.MaxShipsTeam = 10;
                config.MaxBasesTeam = 11;
                config.MaxPlatformsPlayer = 12;
                config.MaxProbesPlayer = 13;
                config.MaxDronesPlayer = 14;
                config.MaxShipsPlayer = 15;
                config.MaxBasesPlayer = 16;
            });

            Console.WriteLine($"Updated galaxy {galaxy.Name} #{galaxy.ID}: {galaxy.Config.Description}");

            Cluster cluster = await galaxy.CreateCluster(config =>
            {
                config.Name = "TestCluster";
            });

            Console.WriteLine($"Created cluster {cluster.Name} #{cluster.ID}");

            Region region = await cluster.CreateRegion(config =>
            {
                config.Name = "TestRegion";
                config.StartPropability = 1;
                config.RespawnPropability = 2;
                config.Protected = true;
            });

            Console.WriteLine($"Created region {region.Name} #{region.ID}");
            Console.WriteLine($"Cluster {cluster.Name} #{cluster.ID} now has {cluster.Regions.Count} regions");

            Team team = await galaxy.CreateTeam(config =>
            {
                config.Name = "TestTeam";
                config.Red = 1;
                config.Green = 2;
                config.Blue = 3;
            });

            Console.WriteLine($"Created team {team.Name} #{team.ID}");

            Ship ship = await galaxy.CreateShip(config =>
            {
                config.Name = "TestShip";
                config.CostEnergy = 1;
                config.CostIon = 0.2;
                config.CostIron = 3;
                config.CostTungsten = 0.4;
                config.CostSilicon = 5;
                config.CostTritium = 0.6;
                config.CostTime = 0.7;
                config.Hull = 0.8;
                config.HullRepair = 0.9;
                config.Shields = 1.0;
                config.ShieldsLoad = 0.11;
                config.Size = 1.2;
                config.Weight = 0.13;
                config.EnergyMax = 1.4;
                config.EnergyCells = 0.15;
                config.EnergyReactor = 0.16;
                config.EnergyTransfer = 0.17;
                config.IonMax = 0.18;
                config.IonCells = 0.19;
                config.IonReactor = 0.20;
                config.IonTransfer = 0.21;
                config.Thruster = 22;
                config.Nozzle = 0.23;
                config.Speed = 0.24;
                config.Turnrate = 0.25;
                config.Cargo = 0.26;
                config.Extractor = 0.27;
                config.WeaponSpeed = 2.8;
                config.WeaponTime = 0.29;
                config.WeaponLoad = 3.0;
                config.FreeSpawn = false;
            });

            Console.WriteLine($"Created ship {ship.Name} #{ship.ID}");

            Upgrade upgrade1 = await ship.CreateUpgrade(config =>
            {
                config.Name = "TestUpgrade1";
                config.CostEnergy = 1;
                config.CostIon = 0.2;
                config.CostIron = 3;
                config.CostTungsten = 0.4;
                config.CostSilicon = 5;
                config.CostTritium = 0.6;
                config.CostTime = 0.7;
                config.Hull = 0.8;
                config.HullRepair = 0.9;
                config.Shields = 1.0;
                config.ShieldsLoad = 0.11;
                config.Size = 1.2;
                config.Weight = 0.13;
                config.EnergyMax = 1.4;
                config.EnergyCells = 0.15;
                config.EnergyReactor = 0.16;
                config.EnergyTransfer = 0.17;
                config.IonMax = 0.18;
                config.IonCells = 0.19;
                config.IonReactor = 0.20;
                config.IonTransfer = 0.21;
                config.Thruster = 22;
                config.Nozzle = 0.23;
                config.Speed = 0.24;
                config.Turnrate = 0.25;
                config.Cargo = 0.26;
                config.Extractor = 0.27;
                config.WeaponSpeed = 2.8;
                config.WeaponTime = 0.29;
                config.WeaponLoad = 3.0;
                config.FreeSpawn = false;
            });

            Console.WriteLine($"Created upgrade {upgrade1.Name} #{upgrade1.ID} following upgrade {upgrade1.Config.PreviousUpgrade?.Name ?? "none"}");
            Console.WriteLine($"Ship {ship.Name} #{ship.ID} now has {ship.Upgrades.Count} upgrades");

            Upgrade upgrade2 = await ship.CreateUpgrade(config =>
            {
                config.Name = "TestUpgrade2";
                config.PreviousUpgrade = upgrade1;
                config.CostEnergy = 1;
                config.CostIon = 2;
                config.CostIron = 3;
                config.CostTungsten = 4;
                config.CostSilicon = 5;
                config.CostTritium = 6;
                config.CostTime = 7;
                config.Hull = 8;
                config.HullRepair = 9;
                config.Shields = 10;
                config.ShieldsLoad = 11;
                config.Size = 12;
                config.Weight = 13;
                config.EnergyMax = 14;
                config.EnergyCells = 15;
                config.EnergyReactor = 16;
                config.EnergyTransfer = 17;
                config.IonMax = 18;
                config.IonCells = 19;
                config.IonReactor = 20;
                config.IonTransfer = 21;
                config.Thruster = 22;
                config.Nozzle = 23;
                config.Speed = 24;
                config.Turnrate = 25;
                config.Cargo = 26;
                config.Extractor = 27;
                config.WeaponSpeed = 28;
                config.WeaponTime = 29;
                config.WeaponLoad = 30;
                config.FreeSpawn = false;
            });

            Console.WriteLine($"Created upgrade {upgrade2.Name} #{upgrade2.ID} following upgrade {upgrade2.Config.PreviousUpgrade?.Name ?? "none"}");
            Console.WriteLine($"Ship {ship.Name} #{ship.ID} now has {ship.Upgrades.Count} upgrades");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        await Task.Delay(60000);
    }
}