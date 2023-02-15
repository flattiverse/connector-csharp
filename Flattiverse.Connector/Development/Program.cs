using Flattiverse.Connector;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.Units;

internal class Program
{
    private static async Task Main(string[] args)
    {
        //Unit
        //string unitSun = "{\"name\":\"Schnappi\",\"setPosition\":{\"x\":200,\"y\":100},\"setRadius\":50,\"gravity\":500,\"kind\":\"sun\"}";

        int ticks = 0;

        using (UniverseGroup universeGroup = new UniverseGroup("ws://127.0.0.1", "0000000000DAD1DAD1DAD1DAD100000000789634278596032409875325734585"))
        //using (UniverseGroup universeGroup = new UniverseGroup("wss://www.flattiverse.com/api/universes/mission1.ws", "0000000000DAD1DAD1DAD1DAD100000000789634278596032409875325734585"))
        {
            Thread.Sleep(1000);

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

                Controllable c = await universeGroup.NewShip("huihui");

            ThreadPool.QueueUserWorkItem(async delegate
            {
                

                Thread.Sleep(1000);

                while (true)
                {
                    await c.Continue();

                    ticks = 0;

                    while (c.IsAlive)
                    {
                        Console.Write($" -> {c.Position}\r");
                        await Task.Delay(10);
                    }

                    Console.WriteLine($" => {ticks} till death.");
                }

                //while (true)
                //{
                //    Thread.Sleep(5000);

                //    //await c.SetThruster(0.15);

                //    Console.WriteLine("Scan now: 270°.");

                //    await c.SetScanner(270, 300, 60, true);

                //    Thread.Sleep(5000);

                //    Console.WriteLine("Scan now: 0°.");

                //    await c.SetScanner(0, 300, 60, true);

                //    Thread.Sleep(5000);

                //    Console.WriteLine("Scan now: 90°.");

                //    await c.SetScanner(90, 300, 60, true);

                //    Thread.Sleep(5000);

                //    Console.WriteLine("Scan now: 180°.");

                //    await c.SetScanner(180, 300, 60, true);
                //}
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
                        Console.WriteLine($"AddedUnitEvent Event: [{addedUnitEvent.Unit.Kind}] {addedUnitEvent.Unit.Name}");
                        break;
                    case UpdatedUnitEvent updatedUnitEvent:
                        //if (updatedUnitEvent.Unit is PlayerUnit)
                        //    Console.WriteLine($"  -> {updatedUnitEvent.Unit.Name} now at: {updatedUnitEvent.Unit.Position} with movement: {updatedUnitEvent.Unit.Movement}: energy={((PlayerUnit)updatedUnitEvent.Unit).BatteryEnergy.Value}, thrust={((PlayerUnit)updatedUnitEvent.Unit).Thruster.Value}, direction={((PlayerUnit)updatedUnitEvent.Unit).Direction}");
                        //else
                        //    Console.WriteLine($"UpdatedUnitEvent Event: {updatedUnitEvent.Unit.Name} now at: {updatedUnitEvent.Unit.Position} with movement: {updatedUnitEvent.Unit.Movement}");
                        break;
                    case TickProcessedEvent tickProcessedEvent:
                        //Console.WriteLine($"Tick: {tickProcessedEvent.ProcessingTime}.");
                        //Console.WriteLine($" * BAT={c.BatteryEnergy.Value}/{((PlayerUnitRegularSystem)c.BatteryEnergy).MaxValue} Alive: {c.IsAlive}");
                        ticks++;
                        break;
                    case DeathControllableEvent deathControllableEvent:
                        Console.WriteLine($"DeathControllableEvent Event: {deathControllableEvent.CauserKind}: \"{deathControllableEvent.CauserName}\"");
                        break;
                    case UnregisteredControllableEvent unregisteredControllableEvent:
                        Console.WriteLine($"DeathControllableEvent Event: {unregisteredControllableEvent.Controllable}.");
                        break;
                    case ChatUnicastEvent chatUnicastEvent:
                        Console.WriteLine($"ChatUnicastEvent Event: {chatUnicastEvent.Source.Name}: \"{chatUnicastEvent.Message}\"");
                        break;
                    case ChatTeamcastEvent chatTeamcastEvent:
                        Console.WriteLine($"ChatTeamcastEvent Event: {chatTeamcastEvent.Source.Name}: \"{chatTeamcastEvent.Message}\"");
                        break;
                    case ChatMulticastEvent chatMulticastEvent:
                        Console.WriteLine($"ChatMulticastEvent Event: {chatMulticastEvent.Source.Name}: \"{chatMulticastEvent.Message}\"");
                        break;
                }
        }
    }
}