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

`auth == null` connects as spectator. `team == null` lets the server auto-select the least populated non-spectator team for normal players unless an active tournament assigns the account to a configured team. `runtimeDisclosure` and `buildDisclosure` are optional extra arguments on `Galaxy.Connect(...)`; galaxies may require them. The server sends periodic ping challenges and disconnects sessions whose replies stay stale for too long. The reference connector replies to those ping challenges automatically, so long-lived clients do not need a separate heartbeat loop.

## Current Capabilities

- connect to a galaxy and keep a local mirror of galaxy settings, teams, clusters, players, controllables and visible units
- receive strongly typed events for settings changes, compile profile announcement, score changes, team and cluster lifecycle, player lifecycle, visible unit lifecycle, `ControllableInfo` lifecycle, owner-side subsystem runtime, player chat, objective system chat including mission-target hits, and connection termination
- receive owner-only `CollectedPowerUpEvent` packets with `PowerUpName`, `PowerUpKind`, configured `Amount`, and actual `AppliedAmount`
- inspect `Galaxy.Tournament` and receive `CreatedTournamentEvent`, `UpdatedTournamentEvent`, `RemovedTournamentEvent`, and `TournamentMessageEvent`
- observe respawning power-ups through the normal visible-unit create/delete lifecycle; the connector does not use a dedicated respawn packet
- inspect `Player.Score` and `Team.Score`, and react to `UpdatedPlayerScoreEvent` and `UpdatedTeamScoreEvent`
- create and control classic ships
- create and control modern ships
- create classic ships with up to three equipped crystal names
- create modern ships with up to three equipped crystal names
- request, create, rename and destroy account-wide crystals through `Galaxy.RequestCrystals()`, `Galaxy.ProduceCrystal(...)`, `Galaxy.RenameCrystal(...)` and `Galaxy.DestroyCrystal(...)`
- inspect and control the `ClassicShipControllable.Engine` subsystem
- inspect and control `ModernShipControllable.EngineN` .. `EngineNW`
- inspect owner-side integrity runtime via `Controllable.Hull` and `Controllable.Shield`
- inspect owner-side nebula cargo via `Controllable.Cargo.MaximumNebula`, `CurrentNebula` and `NebulaHue`
- inspect and control `ClassicShipControllable.NebulaCollector`
- inspect and control `ModernShipControllable.ScannerN` .. `ScannerNW`
- inspect and use `ModernShipControllable.ShotLauncherN` .. `ShotLauncherNW`, the matching magazines and shot fabricators
- inspect and use `ModernShipControllable.InterceptorLauncherE` / `InterceptorLauncherW`, the matching magazines and interceptor fabricators
- inspect and use `ModernShipControllable.RailgunN` .. `RailgunNW`
- inspect and use the `ClassicShipControllable.ShotLauncher`, `ClassicShipControllable.ShotMagazine`, and `ClassicShipControllable.ShotFabricator` subsystems
- inspect and use the `ClassicShipControllable.InterceptorLauncher`, `ClassicShipControllable.InterceptorMagazine`, and `ClassicShipControllable.InterceptorFabricator` subsystems
- inspect and use `ClassicShipControllable.Railgun` via `FireFront()` / `FireBack()`
- inspect owner-side controllable battery runtime via `Controllable.EnergyBattery`, `Controllable.IonBattery` and `Controllable.NeutrinoBattery`
- inspect controllable battery capacities and energy-cell efficiencies via the corresponding subsystem objects
- inspect per-tick battery consumption via `BatterySubsystem.ConsumedThisTick`
- inspect per-tick collected energy via `EnergyCellSubsystem.CollectedThisTick`
- inspect owner-side ClassicShip scanner runtime via `ClassicShipControllable.MainScanner` and `ClassicShipControllable.SecondaryScanner`
- send galaxy, team and private chat messages
- use admin functions to configure galaxy metadata, teams and clusters
- use admin functions to edit regions and editable map units
- use admin functions to query accounts and configure, commence, start, or cancel tournaments
- inspect `CurrentField` map units including directional flow and relative radial or tangential force configuration
- inspect `Storm` map units and the visible lifecycle of `StormCommencingWhirl` and `StormActiveWhirl`

The connector currently materializes these unit kinds as visible units or own controllables:

- `Sun`
- `BlackHole`
- `CurrentField`
- `Nebula`
- `Storm`
- `StormCommencingWhirl`
- `StormActiveWhirl`
- `Planet`
- `Moon`
- `Meteoroid`
- `Buoy`
- `WormHole`
- `MissionTarget`
- `Flag`
- `DominationPoint`
- `EnergyChargePowerUp`
- `IonChargePowerUp`
- `NeutrinoChargePowerUp`
- `MetalCargoPowerUp`
- `CarbonCargoPowerUp`
- `HydrogenCargoPowerUp`
- `SiliconCargoPowerUp`
- `ShieldChargePowerUp`
- `HullRepairPowerUp`
- `ShotChargePowerUp`
- `Switch`
- `Gate`
- `Shot`
- `Interceptor`
- `Rail`
- `ClassicShipPlayerUnit`
- `ModernShipPlayerUnit`
- `InterceptorExplosion`
- `Explosion`

## Core API

Connection and local state:

- [`Galaxy.Connect(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L98)
- [`Galaxy.NextEvent()`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L348)
- [`Galaxy.Tournament`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.Tournament.cs#L12)
- [`Galaxy.Teams`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L73)
- [`Galaxy.Clusters`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L78)
- [`Galaxy.Players`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L83)
- [`Galaxy.Controllables`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L88)
- [`Galaxy.CompiledWithMaxPlayersSupported`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L321)
- [`Galaxy.CompiledWithSymbol`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L326)

Player-facing commands:

- [`Galaxy.Chat(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L488)
- [`Galaxy.CreateClassicShip(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L555)
- [`Galaxy.CreateModernShip(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L645)
- [`Galaxy.RequestCrystals()`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L581)
- [`Galaxy.ProduceCrystal(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L594)
- [`Galaxy.RenameCrystal(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L613)
- [`Galaxy.DestroyCrystal(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L627)
- [`Team.Chat(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Team.cs#L48)
- [`Player.Chat(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Player.cs#L68) for private text chat and private binary chat
- [`Player.DownloadSmallAvatar(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Player.cs#L117)
- [`Player.DownloadBigAvatar(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Player.cs#L134)
- [`Galaxy.CreateClassicShip(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L501)
- [`ClassicShipControllable.MainScanner`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/ClassicShipControllable.cs)
- [`ClassicShipControllable.SecondaryScanner`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/ClassicShipControllable.cs)
- [`ClassicShipControllable.Engine`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/ClassicShipControllable.cs)
- [`ModernShipControllable`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/ModernShipControllable.cs)
- [`Controllable.Hull`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Controllable.cs)
- [`Controllable.Shield`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Controllable.cs)
- [`ClassicShipControllable.ShotLauncher`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/ClassicShipControllable.cs)
- [`ClassicShipControllable.ShotMagazine`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/ClassicShipControllable.cs)
- [`ClassicShipControllable.ShotFabricator`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/ClassicShipControllable.cs)
- [`ClassicShipControllable.InterceptorLauncher`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/ClassicShipControllable.cs)
- [`ClassicShipControllable.InterceptorMagazine`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/ClassicShipControllable.cs)
- [`ClassicShipControllable.InterceptorFabricator`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/ClassicShipControllable.cs)
- [`ClassicShipControllable.Railgun`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/ClassicShipControllable.cs)

Admin-facing commands:

- [`Galaxy.Configure(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L475)
- [`Galaxy.QueryAccounts(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.Tournament.cs#L72)
- [`Galaxy.ConfigureTournament(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.Tournament.cs#L17)
- [`Galaxy.CommenceTournament()`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.Tournament.cs#L48)
- [`Galaxy.StartTournament()`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.Tournament.cs#L56)
- [`Galaxy.CancelTournament()`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.Tournament.cs#L64)
- [`Cluster.SetRegion(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L120)
- [`Cluster.RemoveRegion(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L138)
- [`Cluster.QueryRegions()`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L157)
- [`Cluster.QueryEditableUnits(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L178)
- [`Cluster.SetUnit(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L201)
- [`Cluster.RemoveUnit(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L218)
- [`Cluster.QueryUnitXml(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L236)

## Notes

- `NextEvent()` must not be awaited concurrently. Parallel calls fail with `CantCallThisConcurrentGameException`.
- The `Development` project is a regression and experimentation client, not a minimal beginner sample.
- The private scripted tooling (`CliShip`, `AdminRunner`) now lives in the sibling repository `../fv-cliship` and is intentionally no longer part of the public connector solution.
- Map-edit XML rules, examples, and server-side validation behavior are documented in [MAPEDITORS.md](MAPEDITORS.md).
- The server keeps sessions alive with periodic ping challenges. The reference connector answers them automatically, so client code only needs to keep consuming events and issuing its normal commands.
- `Galaxy.Tournament` is `null` if no tournament is configured. Tournament state changes are mirrored through tournament events and can additionally surface as `TournamentMessageEvent` system-chat packets.
- `ControllableInfo` lifecycle is event-driven. Owner-side `Controllable` objects are mirrored locally on `0x80` / `0x81` / `0x8F`, but the connector currently does not raise separate lifecycle events for them. `0x8F` is the final close, not the initial close request.
- `Player.HasAvatar` tells you whether `DownloadSmallAvatar(...)` / `DownloadBigAvatar(...)` are valid for that player. The reference connector throws immediately if no avatar is available.
- `DownloadSmallAvatar(...)` / `DownloadBigAvatar(...)` and the matching account avatar download methods accept an optional `ProgressState` that can be polled while the connector fetches internal chunks.
- `Galaxy.QueryAccounts(...)` returns all `user` / `reoptin` connector `Account` snapshots for tournament tooling in one logical result and accepts an optional `ProgressState` for chunked transfer progress.
- `Cluster.QueryEditableUnits(...)` returns `(Name, Kind)` summaries for all editable units of the cluster, including currently invisible ones such as inactive power-ups.
- Battery maxima, cell efficiencies, hull and shield maxima, cargo capacities, and similar owner-side static subsystem capabilities are initialized from `0x80 Controllable Create`.
- Scanner subsystems are server-authoritative runtime objects. `Set(...)`, `On()`, and `Off()` send player commands; `Current*`, `Target*`, and `Active` are mirrored back via `0x82`.
- `Controllable.Angle` / `AngularVelocity` and visible `PlayerUnit.Angle` / `AngularVelocity` are authoritative wire values. They are no longer derived from movement.
- Worm holes are visible units. Their destination is intentionally absent in reduced visibility and only appears once the unit reaches full visibility. Triggering a jump is an explicit owner command through `JumpDrive.Jump()`, not an automatic collision effect.
- `CurrentField` is a non-solid visible map unit. `Mode=Directional` exposes a fixed world-space `Flow`, while `Mode=Relative` exposes `RadialForce` and `TangentialForce`.
- `Storm` is an editable non-solid map unit that spawns mobile storm whirls. `StormCommencingWhirl` is visible early and only exposes its remaining announcement ticks in full visibility, while `StormActiveWhirl` is masking and additionally exposes `Damage`.
- Scanner angles are absolute world angles. `scan:90x300x0` therefore points east on the world axis, independent of ship movement.
- The current scanner cost preview is surface-based and matches the current server-side scanner formula.
- `Shield.Set(...)` / `On()` / `Off()` manage shield loading. The reference `ClassicShip` shield has `Maximum=20`, `MaximumRate=0.125`, and tick cost `1600 * rate^2`, so the maximum rate costs `25` energy per tick.
- `Armor` is a passive flat reduction before hull damage. `Repair.Set(rate)` manages hull-only repair with `rate in [0; 0.1]` and tick cost `1600 * rate^2`; `rate == 0` means off, and the server clears the rate when ship movement reaches `>= 0.1`.
- Passive sun effects now also surface as owner-only `EnvironmentDamageEvent`: heat drains `15` energy per point, unpaid heat overflows into radiation, and radiation damage is reduced by armor before reaching hull.
- `ShotLauncher.Shoot(...)` and `InterceptorLauncher.Shoot(...)` use the same configurable projectile launch profile. `ShotFabricator.Set(...)` / `On()` / `Off()` and `InterceptorFabricator.Set(...)` / `On()` / `Off()` manage the two magazines separately.
- `Railgun.FireFront()` / `FireBack()` request a fixed rail shot with `EnergyCost=300`, `MetalCost=1`, projectile speed `4`, and lifetime `250`. At zero ship movement the rail direction falls back to world angle `0`.
- `ShotMagazine.CurrentShots` and `InterceptorMagazine.CurrentShots` are owner-visible through `0x82` and visible on foreign ships through their richer `UnitUpdated` snapshots.
- The owner's own ship must be read from `Controllable` / `0x80` / `0x82`, not from visible-unit events. Those owner packets now also carry the authoritative `clusterId`, so cluster jumps must be tracked there instead of inferred from visible scans.
