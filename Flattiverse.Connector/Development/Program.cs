using Flattiverse.Connector;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.Units;

internal class Program
{
    private static async Task Main(string[] args)
    {
        //Matz
        string apiUser = "0123456789112345678921234567893123456789412345678951234567896123";
        string apiAdmn = "9876342587963245879623458976234589762345ACBACBACABCEDFEDFDEFEDFE";

        //Togo
        string apiUser2 = "BABABABABABABABABBBBBBBBBBBBBBBBBBBBB319480573420958723458796345";
        string apiAdmn2 = "BABABABABABABABABCCCCCCCCCCCCCCCCCCCC319480573420958723458796345";

        //Harald
        string apiUser3 = "CDCDCDCDCDCDC34587652345CABCABACBACBA319480573420958723458796345";
        string apiAdmn3 = "FEFEFEFE2873654324876EFEFEFEF23454325319480573420958723458796345";

        //Micha
        string apiUser4 = "555555555555FEBFEBDASASDASD1356723423419480573420958723458796345";
        string apiAdmn4 = "555555555555FEBFEBDASASDASD1356234234719480573420958723458796345";

        string apiSpec = "0000000000000000000000000000000000000000000000000000000000000000";
        string apiShit = "000000000000000000000000000000000000000000000000000000000000D3AD";

        //Unit
        //string unitSun = "{\"name\":\"Schnappi\",\"setPosition\":{\"x\":200,\"y\":100},\"setRadius\":50,\"gravity\":500,\"kind\":\"sun\"}";

        using (UniverseGroup universeGroup = new UniverseGroup("ws://127.0.0.1", apiAdmn))
        //using (UniverseGroup universeGroup = new UniverseGroup("wss://www.flattiverse.com/api/universes/beginnersGround.ws", apiAdmn))
        {
            //foreach (GameRegion region in await universeGroup.GetUniverse("Training ground")!.GetRegions())
            //    Console.WriteLine($" * {region.ID}\\{region.Name ?? "<unnamed>"}");

            //await universeGroup.GetUniverse("Training ground")!.SetRegion(3, 0, null, 1000, 1000, 1100, 1100, false, false, false);
            //await universeGroup.GetUniverse("Training ground")!.SetRegion(4, 0, "Test", 2000, 2000, 2100, 2100, false, false, false);
            //await universeGroup.GetUniverse("Training ground")!.RemoveRegion(3);
            //await universeGroup.GetUniverse("Training ground")!.RemoveRegion(4);


            //await universeGroup.GetUniverse("Training ground")!.SetUnit(unitSun);
            //string unitJson = await universeGroup.GetUniverse("Training ground")!.GetUnitMapEditJson("Schnappi");
            //await universeGroup.GetUniverse("Training ground")!.RemoveUnit("Schnappi");
            //await universeGroup.GetUniverse("Training ground")!.SetUnit(unitSun);


            //PlayerUnitSystemUpgradepath? path = universeGroup.GetSystem(PlayerUnitSystemKind.Armor, 1);
            //foreach (KeyValuePair<PlayerUnitSystemIdentifier, PlayerUnitSystemUpgradepath> kvp in await universeGroup.GetSystems())
            //    Console.WriteLine($" * {kvp.Key.Kind}\\{kvp.Key.Level}");

            ThreadPool.QueueUserWorkItem(async delegate
            {
                Controllable c = await universeGroup.NewShip("huihui");

                while (true)
                    await Task.Delay(100);
            });

            while (true)
                switch (await universeGroup.NextEvent())
                {
                    case RawEvent rawEvent:
                        Console.WriteLine($"RAW Event: {rawEvent.RawData}");
                        break;
                    case FailureEvent failureEvent:
                        Console.WriteLine($"Failure Event: {failureEvent.Message}");
                        break;
                    case UniverseGroupInfoEvent universeGroupInfoEvent:
                        Console.WriteLine($"UniverseGroupInfo Event: {universeGroupInfoEvent.Name}");
                        break;
                    case FullUpdatePlayerEvent fullUpdatePlayerEvent:
                        Console.WriteLine($"FullUpdatePlayerEvent Event: #{fullUpdatePlayerEvent.ID}");
                        break;
                    case PartialUpdatePlayerEvent partialUpdatePlayerEvent:
                        Console.WriteLine($"PartialUpdatePlayerEvent Event: #{partialUpdatePlayerEvent.ID}");
                        break;
                    case RemovedPlayerEvent removedPlayerEvent:
                        Console.WriteLine($"RemovedPlayerEvent Event: #{removedPlayerEvent.ID}");
                        break;
                    case RemovedUnitEvent removedUnitEvent:
                        Console.WriteLine($"RemovedUnitEvent Event: {removedUnitEvent.Name}");
                        break;
                    case AddedUnitEvent addedUnitEvent:
                        Console.WriteLine($"AddedUnitEvent Event: {addedUnitEvent.Unit.Name}");
                        break;
                }
        }
    }
}