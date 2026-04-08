# Flattiverse.Connector

`Flattiverse.Connector` is the C# connector for Flattiverse galaxy servers.

The package provides:

- connection management for Flattiverse galaxy endpoints
- a local mirror of the current galaxy state
- event-driven APIs for player and admin sessions
- command APIs for ships, map tooling and automation

Target framework:

- `.NET 8`

Example:

```csharp
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;

Galaxy galaxy = await Galaxy.Connect(
    "wss://www.flattiverse.com/galaxies/0/api",
    "<api-key>",
    "Pink");

while (galaxy.Active)
{
    FlattiverseEvent @event = await galaxy.NextEvent();
    Console.WriteLine(@event);
}
```
