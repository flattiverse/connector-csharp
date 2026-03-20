# Flattiverse.Connector

`Flattiverse.Connector` is the C# reference connector for Flattiverse galaxies. It mirrors the current galaxy state locally, exposes an event-driven API, and lets players or admins call commands against a galaxy endpoint.

If you want to implement your own connector, see [IMPLEMENTERS.md](IMPLEMENTERS.md).

If you want to build a map editor against the connector, see [MAPEDITORS.md](MAPEDITORS.md).

## Requirements

- `.NET 10`
- a Flattiverse API key for player or admin access
- a full galaxy endpoint URI such as `wss://www.flattiverse.com/galaxies/0/api`

## Getting Started

1. Register an account at the [Flattiverse homepage](https://www.flattiverse.com/).
2. Create an API key.
3. Add the [`Flattiverse.Connector`](https://www.nuget.org/packages/Flattiverse.Connector) NuGet package to your project.
4. Connect to a galaxy endpoint with `Galaxy.Connect(...)`.
5. Process events with `await galaxy.NextEvent()`.

Minimal example:

```csharp
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;

Galaxy galaxy = await Galaxy.Connect(
    "wss://www.flattiverse.com/galaxies/0/api",
    "<api-key>",
    "Pink");

await galaxy.Chat("Hello world.");

while (galaxy.Active)
{
    FlattiverseEvent @event = await galaxy.NextEvent();
    Console.WriteLine(@event);
}
```

`auth == null` connects as spectator. `team == null` lets the server auto-select the least populated non-spectator team for normal players. `runtimeDisclosure` and `buildDisclosure` are optional extra arguments on `Galaxy.Connect(...)`; galaxies may require them.

## Current Capabilities

- connect to a galaxy and keep a local mirror of galaxy settings, teams, clusters, players, controllables and visible units
- receive strongly typed events for settings changes, compile profile announcement, score changes, team and cluster lifecycle, player lifecycle, visible unit lifecycle, `ControllableInfo` lifecycle, owner-side subsystem runtime, chat and connection termination
- inspect `Player.Score` and `Team.Score`, and react to `PlayerScoreUpdatedEvent` and `TeamScoreUpdatedEvent`
- create and control classic ships
- inspect and control the `ClassicShipControllable.Engine` subsystem
- inspect and use the `ClassicShipControllable.Weapon` subsystem
- inspect owner-side controllable battery runtime via `Controllable.EnergyBattery`, `Controllable.IonBattery` and `Controllable.NeutrinoBattery`
- inspect controllable battery capacities and energy-cell efficiencies via the corresponding subsystem objects
- inspect per-tick battery consumption via `BatterySubsystem.ConsumedThisTick`
- inspect per-tick collected energy via `EnergyCellSubsystem.CollectedThisTick`
- inspect owner-side ClassicShip scanner runtime via `ClassicShipControllable.MainScanner` and `ClassicShipControllable.SecondaryScanner`
- send galaxy, team and private chat messages
- use admin functions to configure galaxy metadata, teams and clusters
- use admin functions to edit regions and editable map units

The connector currently materializes these unit kinds as visible units or own controllables:

- `Sun`
- `BlackHole`
- `Planet`
- `Moon`
- `Meteoroid`
- `Buoy`
- `MissionTarget`
- `Flag`
- `DominationPoint`
- `Shot`
- `ClassicShipPlayerUnit`
- `Explosion`

`NewShipPlayerUnit` already exists in `UnitKind` and `ControllableInfo`, but the connector currently has no visible-unit or owner-side runtime type for it.

## Core API

Connection and local state:

- [`Galaxy.Connect(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L98)
- [`Galaxy.NextEvent()`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L348)
- [`Galaxy.Teams`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L73)
- [`Galaxy.Clusters`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L78)
- [`Galaxy.Players`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L83)
- [`Galaxy.Controllables`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L88)
- [`Galaxy.CompiledWithMaxPlayersSupported`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L321)
- [`Galaxy.CompiledWithSymbol`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L326)

Player-facing commands:

- [`Galaxy.Chat(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L488)
- [`Team.Chat(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Team.cs#L48)
- [`Player.Chat(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Player.cs#L68)
- [`Player.DownloadSmallAvatar()`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Player.cs#L80)
- [`Player.DownloadBigAvatar()`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Player.cs#L100)
- [`Galaxy.CreateClassicShip(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L501)
- [`ClassicShipControllable.MainScanner`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/ClassicShipControllable.cs)
- [`ClassicShipControllable.SecondaryScanner`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/ClassicShipControllable.cs)
- [`ClassicShipControllable.Engine`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/ClassicShipControllable.cs)
- [`ClassicShipControllable.Weapon`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/ClassicShipControllable.cs)

Admin-facing commands:

- [`Galaxy.Configure(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L475)
- [`Cluster.SetRegion(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L120)
- [`Cluster.RemoveRegion(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L138)
- [`Cluster.QueryRegions()`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L157)
- [`Cluster.SetUnit(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L214)
- [`Cluster.RemoveUnit(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L231)
- [`Cluster.QueryUnitXml(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L249)

## Notes

- `NextEvent()` must not be awaited concurrently. Parallel calls fail with `CantCallThisConcurrentGameException`.
- The `Development` project is a regression and experimentation client, not a minimal beginner sample.
- The private scripted tooling (`CliShip`, `AdminRunner`) now lives in the sibling repository `../fv-cliship` and is intentionally no longer part of the public connector solution.
- Map-edit XML rules, examples, and server-side validation behavior are documented in [MAPEDITORS.md](MAPEDITORS.md).
- `ControllableInfo` lifecycle is event-driven. Owner-side `Controllable` objects are mirrored locally on `0x80` / `0x81` / `0x8F`, but the connector currently does not raise separate lifecycle events for them. `0x8F` is the final close, not the initial close request.
- Battery and energy-cell subsystem capabilities are initialized locally by controllable kind and are currently not transmitted on the wire.
- Scanner subsystems are server-authoritative runtime objects. `Set(...)`, `On()`, and `Off()` send player commands; `Current*`, `Target*`, and `Active` are mirrored back via `0x82`.
- Scanner angles are relative to the current ship facing. `scan:90x300x0` therefore points straight ahead.
- The current scanner cost preview is surface-based and matches the current server-side scanner formula.
- The owner's own ship must be read from `Controllable` / `0x80` / `0x82`, not from visible-unit events. The server does not echo a player's own controllables back through `NewUnit` / `UpdatedUnit` / `RemovedUnit`, even though those controllables still participate in scan masking and related geometry.
