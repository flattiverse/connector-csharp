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

`auth == null` connects as spectator. `team == null` is currently intended for spectator and admin logins; normal player logins currently need an explicit team.

## Current Capabilities

- connect to a galaxy and keep a local mirror of galaxy settings, teams, clusters, players, controllables and visible units
- receive strongly typed events for settings changes, team and cluster lifecycle, player lifecycle, visible unit lifecycle, controllable lifecycle, chat and connection termination
- create and control classic ships
- send galaxy, team and private chat messages
- use admin functions to configure galaxy metadata, teams and clusters
- use admin functions to edit regions and editable map units

The connector currently knows these map-relevant unit kinds:

- `Sun`
- `BlackHole`
- `Planet`
- `Moon`
- `Meteoroid`
- `Buoy`
- `MissionTarget`
- `Shot`
- `ClassicShipPlayerUnit`
- `NewShipPlayerUnit`
- `Explosion`

## Core API

Connection and local state:

- [`Galaxy.Connect(...)`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs:94)
- [`Galaxy.NextEvent()`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs:332)
- [`Galaxy.Teams`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs:64)
- [`Galaxy.Clusters`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs:74)
- [`Galaxy.Players`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs:79)
- [`Galaxy.Controllables`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs:84)

Player-facing commands:

- [`Galaxy.Chat(...)`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs:467)
- [`Galaxy.CreateClassicShip(...)`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs:480)

Admin-facing commands:

- [`Galaxy.Configure(...)`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs:454)
- [`Cluster.SetRegion(...)`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs:110)
- [`Cluster.RemoveRegion(...)`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs:128)
- [`Cluster.QueryRegions()`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs:147)
- [`Cluster.SetUnit(...)`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs:204)
- [`Cluster.RemoveUnit(...)`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs:221)
- [`Cluster.QueryUnitXml(...)`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs:239)

## Notes

- `NextEvent()` must not be awaited concurrently. Parallel calls fail with `CantCallThisConcurrentGameException`.
- The `Development` project is a regression and experimentation client, not a minimal beginner sample.
- Map-edit XML rules, examples, and server-side validation behavior are documented in [MAPEDITORS.md](MAPEDITORS.md).
