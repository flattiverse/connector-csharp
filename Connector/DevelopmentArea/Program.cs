using Flattiverse;
using Flattiverse.Units;
using Flattiverse.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentArea
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Gray;

            using (Server server0 = new Server())
            using (Server server1 = new Server())
            {
                await server0.Login("Player0", "Password");
                await server1.Login("Player1", "Password");

                Universe universe0 = server0.Universes["Beginners Course"];
                Universe universe1 = server1.Universes["Beginners Course"];

                await universe0.Join();
                await universe1.Join();

                Controllable controllable0 = await universe0.NewShip("Bounty0");

                Console.WriteLine($" * Registered: {controllable0.Name} at position: {controllable0.Position} with a hull of {controllable0.Hull}.");

                await controllable0.Continue();

                Console.WriteLine($" * Registered: {controllable0.Name} at position: {controllable0.Position} with a hull of {controllable0.Hull}.");

                Controllable controllable1 = await universe1.NewShip("Bounty1");

                await controllable1.Continue();

                FlattiverseEvent @event;

                int heartbeat = 0;

                for (int eventsCounter = 0; heartbeat < 40; eventsCounter++)
                {
                    Queue<FlattiverseEvent> events = await server0.GatherEvents();

                    while (events.TryDequeue(out @event))
                    {
                        if (@event is HeartbeatEvent)
                        {
                            //Console.WriteLine($" =[{heartbeat.ToString("00")}]=> {controllable0.Name} THRUSTER={controllable0.Thruster} ROTATION={controllable0.Rotation} DIRECTION={controllable0.Direction}");

                            Console.WriteLine("\n *** " + heartbeat.ToString() + " E: " + controllable0.Energy + " cE: " + controllable0.EnergyOnLocation);

                            foreach (FlattiverseResourceKind kind in Enum.GetValues(typeof(FlattiverseResourceKind)))
                                if (kind != FlattiverseResourceKind.None)
                                    Console.WriteLine($" => {kind}: {controllable0.GetResource(kind).Current} / {controllable0.GetResource(kind).Max}");

                            heartbeat++;
                        }

                        //if (@event is UpdatedUnitEvent && ((UpdatedUnitEvent)@event).Unit is PlayerUnit && ((UpdatedUnitEvent)@event).Unit.Name == "Bounty0")
                        //    Console.WriteLine($" =[{heartbeat.ToString("00")}]=> {((UpdatedUnitEvent)@event).Unit.Name} THRUSTER={((PlayerUnit)((UpdatedUnitEvent)@event).Unit).Thruster} ROTATION={((PlayerUnit)((UpdatedUnitEvent)@event).Unit).Rotation} DIRECTION={((PlayerUnit)((UpdatedUnitEvent)@event).Unit).Direction}");

                        //if (@event is NewUnitEvent && ((NewUnitEvent)@event).Unit is Sun)
                        //    Console.WriteLine($" =[{heartbeat.ToString("00")}]=> {((NewUnitEvent)@event).Unit.Name} RESOURCE={((Sun)((NewUnitEvent)@event).Unit).Resource}");

                        switch (heartbeat)
                        {
                            case 10:
                                controllable0.SetThrusters(-1f);
                                break;
                            case 11:
                                controllable0.SetThrusters(0);
                                break;
                            case 15:
                                controllable0.SetThrusters(1f);
                                break;
                            case 16:
                                controllable0.SetThrusters(0f);
                                break;
                            case 18:
                                controllable0.SetThrusters(1f);
                                break;
                            case 19:
                                controllable0.SetThrusters(0f);
                                break;
                            case 22:
                                controllable0.SetThrusters(1f);
                                break;
                            case 24:
                                controllable0.SetThrusters(0f);
                                break;

                        }
                    }
                }
            }
        }
    }
}