# Flattiverse Map Editors

This document is for software that connects as a Flattiverse admin and edits galaxy metadata, teams, clusters, regions or editable map units through `Flattiverse.Connector`.

Relevant API entry points:

- [`Galaxy.Configure(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L475)
- [`Cluster.SetRegion(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L120)
- [`Cluster.RemoveRegion(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L138)
- [`Cluster.QueryRegions()`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L157)
- [`Cluster.SetUnit(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L214)
- [`Cluster.RemoveUnit(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L231)
- [`Cluster.QueryUnitXml(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L249)

## General Habits

- All validation that exists in the client also exists on the server. Do not trust local validation alone.
- Unknown XML attributes are rejected.
- Unknown XML child elements are rejected. Plain text content is currently ignored by the server parser.
- Empty or malformed XML fails with `InvalidArgumentGameException` and `Reason == AmbiguousXmlData`.
- Semantically invalid XML fails with `InvalidXmlNodeValueGameException`. Use `Reason`, `NodePath` and `Hint` for UI feedback.
- `QueryUnitXml(...)` returns server-generated canonical XML.
- `QueryRegions()` reconstructs XML locally in the connector from the binary region query reply. Prefer querying back after edits instead of maintaining a second serializer in your editor.
- Region changes are not event-driven. Query them when your editor needs the current region set.
- `Galaxy.Configure(...)` does not reject target types based on `GameMode`. You may keep targets in the map while changing the galaxy game mode.
- Editable unit changes are event-driven. When an admin edits or removes an editable unit, the server sends `RemovedUnit` first and then `UnitAlteredByAdminEvent` to admins, spectators, and players that have seen that unit before during their current connection.

## Connection

Use an admin API key and connect to the full galaxy endpoint:

```csharp
Galaxy galaxy = await Galaxy.Connect(
    "wss://www.flattiverse.com/galaxies/1/api",
    "<admin-api-key>");
```

## Galaxy.Configure XML

`Galaxy.Configure(...)` edits galaxy metadata and defines the final set of teams and clusters.

Important behavior:

- `Team` and `Cluster` children define the final set.
- Missing team ids are removed.
- Missing cluster ids are removed.
- Existing teams and clusters may be partially updated. Missing attributes keep the old value.
- At least one resulting cluster must have `Start="true"`.
- Team id `12` is reserved for spectators and must not be configured.
- Team deletion is rejected if a region or editable unit still references that team.

Example:

```xml
<Galaxy
  Name="MapEditTest"
  Description="Map editing test universe."
  GameMode="Mission"
  MaxPlayers="8"
  MaxSpectators="64"
  GalaxyMaxTotalShips="8"
  GalaxyMaxClassicShips="8"
  GalaxyMaxNewShips="8"
  GalaxyMaxBases="0"
  TeamMaxTotalShips="8"
  TeamMaxClassicShips="8"
  TeamMaxNewShips="8"
  TeamMaxBases="0"
  PlayerMaxTotalShips="1"
  PlayerMaxClassicShips="1"
  PlayerMaxNewShips="1"
  PlayerMaxBases="0"
  Maintenance="false"
  RequiresSelfDisclosure="false">
  <Team Id="0" Name="Pink" ColorR="255" ColorG="0" ColorB="200" />
  <Team Id="1" Name="Green" ColorR="192" ColorG="255" ColorB="0" />
  <Cluster Id="0" Name="Playground" Start="true" Respawn="false" />
  <Cluster Id="1" Name="Arena" Start="false" Respawn="true" />
</Galaxy>
```

Allowed `Galaxy` attributes:

- `GameMode`: selects the galaxy-wide objective logic, for example `Mission`, `ShootTheFlag`, `Domination` or `Race`.
- `Name`: display name of the galaxy shown to clients.
- `Description`: longer descriptive text for UIs and selection screens.
- `MaxPlayers`: maximum number of regular player connections allowed at the same time.
- `MaxSpectators`: maximum number of spectator connections allowed at the same time.
- `GalaxyMaxTotalShips`: galaxy-wide cap for all player-owned controllables combined.
- `GalaxyMaxClassicShips`: galaxy-wide cap for `ClassicShipPlayerUnit`.
- `GalaxyMaxNewShips`: galaxy-wide cap for `NewShipPlayerUnit`.
- `GalaxyMaxBases`: galaxy-wide cap for future or custom base-type controllables.
- `TeamMaxTotalShips`: per-team cap for all player-owned controllables combined.
- `TeamMaxClassicShips`: per-team cap for `ClassicShipPlayerUnit`.
- `TeamMaxNewShips`: per-team cap for `NewShipPlayerUnit`.
- `TeamMaxBases`: per-team cap for future or custom base-type controllables.
- `PlayerMaxTotalShips`: per-player cap for all owned controllables combined.
- `PlayerMaxClassicShips`: per-player cap for `ClassicShipPlayerUnit`.
- `PlayerMaxNewShips`: per-player cap for `NewShipPlayerUnit`.
- `PlayerMaxBases`: per-player cap for future or custom base-type controllables.
- `Maintenance`: if `true`, the galaxy is marked as being under maintenance.
- `RequiresSelfDisclosure`: if `true`, regular player logins must provide both self-disclosure strings during connect.

Allowed `Team` attributes:

- `Id`: stable numeric team id referenced by regions and team-owned units.
- `Name`: display name of the team.
- `ColorR`: red channel of the team color in the range `0..255`.
- `ColorG`: green channel of the team color in the range `0..255`.
- `ColorB`: blue channel of the team color in the range `0..255`.

Allowed `Cluster` attributes:

- `Id`: stable numeric cluster id used by protocol packets and admin APIs.
- `Name`: display name of the cluster.
- `Start`: if `true`, this cluster is eligible as an initial spawn cluster.
- `Respawn`: if `true`, this cluster is preferred for respawning dead player units.

Limits and constraints:

- team ids: `0..11`
- cluster ids: `0..23`
- galaxy name: `1..32` characters
- description: `0..4096` characters
- team and cluster names use the strict server name constraint
  - effective length: `2..32` characters
  - no leading or trailing spaces
  - allowed ASCII characters: `A-Z`, `a-z`, `0-9`, space, `.`, `_`, `-`
  - additionally allowed are many Latin accented characters in the ranges `U+00C0..U+00D6`, `U+00D8..U+00F6`, `U+00F8..U+02AF`
  - characters outside those ranges are rejected
- galaxy names are currently looser than team and cluster names
  - length: `1..32` characters
  - no additional character whitelist is applied by the server today
  - leading or trailing spaces are therefore currently accepted
- cluster names and team names are limited to `32` characters

## Regions

Regions are edited per cluster. Region ids are local to the cluster.

Important behavior:

- region ids: `0..255`
- region names are optional
- region names may have up to `64` characters
- region names currently do not use the strict team/cluster name constraint
  - any string value is accepted
  - even an empty string is currently accepted if the `Name` attribute is present
- if a region name is stored as the empty string, `QueryRegions()` currently omits the `Name` attribute on roundtrip
- `Left < Right` must hold
- `Top < Bottom` must hold
- at least one `Team` child is required
- only `Team` child elements are allowed
- spectators (`Team Id="12"`) are forbidden
- duplicate `Team` children are rejected

Example:

```xml
<Region Id="66" Name="Spawn A" Left="-150" Top="-300" Right="150" Bottom="0">
  <Team Id="0" />
  <Team Id="1" />
</Region>
```

Allowed `Region` attributes:

- `Id`: stable numeric region id local to the cluster.
- `Name`: optional editor-facing or UI-facing label for the region.
- `Left`: left world-space boundary of the rectangular region.
- `Top`: top world-space boundary of the rectangular region.
- `Right`: right world-space boundary of the rectangular region.
- `Bottom`: bottom world-space boundary of the rectangular region.

Allowed `Region > Team` attributes:

- `Id`: team id that is allowed to use this region as a start location.

Typical usage:

```csharp
await cluster.SetRegion("""
<Region Id="66" Name="Spawn A" Left="-150" Top="-300" Right="150" Bottom="0">
  <Team Id="0" />
  <Team Id="1" />
</Region>
""");

string xml = await cluster.QueryRegions();
await cluster.RemoveRegion(66);
```

`QueryRegions()` returns:

```xml
<Regions>
  <Region Id="66" Name="Spawn A" Left="-150" Top="-300" Right="150" Bottom="0">
    <Team Id="0" />
    <Team Id="1" />
  </Region>
</Regions>
```

## Editable Units

Editable unit kinds are:

- `Sun`
- `BlackHole`
- `Planet`
- `Moon`
- `Meteoroid`
- `Buoy`
- `MissionTarget`
- `Flag`
- `DominationPoint`

Not editable:

- `Shot`
- `ClassicShipPlayerUnit`
- `NewShipPlayerUnit`
- `Explosion`

General behavior:

- unit XML root node must be the unit type itself, for example `<Sun ... />`
- unit names are required and may have up to `128` characters
- unit names currently do not use the strict team/cluster name constraint
  - effective length: `1..128` characters
  - no additional character whitelist is applied by the server today
  - leading or trailing spaces are currently accepted
- `Name`, `X`, `Y`, `Radius`, `Gravity` are common required attributes
- `Type` values use the enum member names from the connector, for example `OceanWorld`, `RockyMoon` or `MetallicSlug`
- `Radius > 0`
- `Gravity >= 0`
- changing the unit kind for an existing name is allowed only if the existing same-name unit is editable
- `QueryUnitXml(...)` returns the server-generated XML of exactly one unit
- `QueryUnitXml(...)` returns canonical configuration XML, not transient runtime state

Common editable unit attributes:

- `Name`: stable unique unit name within the cluster; updates with the same name replace the existing editable unit.
- `X`: unit center X coordinate in world space.
- `Y`: unit center Y coordinate in world space.
- `Radius`: physical radius of the unit used for collisions, masking and scan geometry.
- `Gravity`: gravitational pull emitted by the unit; `0` disables gravity while keeping the unit otherwise present.

Current server-side name inconsistency:

- `Galaxy.Name`: length-only validation, `1..32`
- `Team.Name` and `Cluster.Name`: strict server name constraint, effectively `2..32`
- `Region.Name`: optional, max `64`, no character whitelist
- editable `Unit.Name`: required, `1..128`, no character whitelist

If you build an editor UI, do not assume one shared "name rule" for all editable objects.

### Sun

Additional required attributes:

- `Energy`: raw energy output offered to receivers before their cell efficiency is applied.
- `Ions`: raw ion output offered to receivers before their cell efficiency is applied.
- `Neutrinos`: raw neutrino output offered to receivers before their cell efficiency is applied.
- `Heat`: radiation heat component emitted by the sun.
- `Drain`: radiation drain component emitted by the sun.

All resource values must be `>= 0`.

Example:

```xml
<Sun Name="Sol" X="0" Y="0" Radius="300" Gravity="2.5" Energy="120" Ions="10" Neutrinos="5" Heat="25" Drain="0" />
```

### BlackHole

Additional required attributes:

- `GravityWellRadius`: radius of the special black-hole gravity well effect around the core.
- `GravityWellForce`: strength of the gravity well effect inside `GravityWellRadius`.

Constraints:

- `GravityWellRadius > 0`
- `GravityWellForce >= 0`

Example:

```xml
<BlackHole Name="Void" X="2000" Y="-500" Radius="120" Gravity="6" GravityWellRadius="180" GravityWellForce="2.5" />
```

### Planet

Additional required attributes:

- `Type`: visual and semantic `PlanetType` of the planet, for example `OceanWorld`.
- `Metal`: configured metal resource amount associated with the planet.
- `Carbon`: configured carbon resource amount associated with the planet.
- `Hydrogen`: configured hydrogen resource amount associated with the planet.
- `Silicon`: configured silicon resource amount associated with the planet.

Constraints:

- `Type` must be a known `PlanetType`
- all resource values must be `>= 0`

Example:

```xml
<Planet Name="Gaia" X="800" Y="150" Radius="90" Gravity="0.25" Type="OceanWorld" Metal="20" Carbon="40" Hydrogen="15" Silicon="25" />
```

### Moon

Additional required attributes:

- `Type`: visual and semantic `MoonType` of the moon, for example `RockyMoon`.
- `Metal`: configured metal resource amount associated with the moon.
- `Carbon`: configured carbon resource amount associated with the moon.
- `Hydrogen`: configured hydrogen resource amount associated with the moon.
- `Silicon`: configured silicon resource amount associated with the moon.

Constraints:

- `Type` must be a known `MoonType`
- all resource values must be `>= 0`

Example:

```xml
<Moon Name="Luna" X="950" Y="170" Radius="30" Gravity="0.04" Type="RockyMoon" Metal="5" Carbon="1" Hydrogen="0" Silicon="4" />
```

### Meteoroid

Additional required attributes:

- `Type`: visual and semantic `MeteoroidType` of the meteoroid, for example `MetallicSlug`.
- `Metal`: configured metal resource amount associated with the meteoroid.
- `Carbon`: configured carbon resource amount associated with the meteoroid.
- `Hydrogen`: configured hydrogen resource amount associated with the meteoroid.
- `Silicon`: configured silicon resource amount associated with the meteoroid.

Constraints:

- `Type` must be a known `MeteoroidType`
- all resource values must be `>= 0`

Example:

```xml
<Meteoroid Name="M1" X="-400" Y="900" Radius="18" Gravity="0.005" Type="MetallicSlug" Metal="12" Carbon="0" Hydrogen="0" Silicon="3" />
```

### Buoy

Required attributes:

- no additional attributes beyond the common unit attributes

Optional attributes:

- `Message`: optional free-form editor text shown by the buoy.

Constraints:

- empty `Message` is normalized to `null`
- `Message` length is at most `384` characters

Examples:

```xml
<Buoy Name="Info" X="100" Y="200" Radius="12" Gravity="0" />
```

```xml
<Buoy Name="Warning" X="120" Y="220" Radius="12" Gravity="0" Message="Minefield ahead." />
```

### MissionTarget

Additional required attributes:

- `Team`: owning or assigned team id for the target.

Optional attributes:

- `SequenceNumber`: order index used by mission logic to sequence targets.

Allowed child elements:

- `Vector`: one path or waypoint node associated with the mission target.

Allowed `Vector` attributes:

- `X`: waypoint X coordinate in world space.
- `Y`: waypoint Y coordinate in world space.

Constraints:

- `Team` must exist
- `Team` must not be spectators
- `SequenceNumber` defaults to `0` if omitted; `QueryUnitXml(...)` writes it back explicitly
- `SequenceNumber >= 0`
- `Vector` children are optional
- only `Vector` child elements are allowed
- `Vector` child elements must not contain nested elements
- at most `65535` vectors

Example:

```xml
<MissionTarget Name="Alpha" X="400" Y="-250" Radius="20" Gravity="0" Team="0" SequenceNumber="3">
  <Vector X="500" Y="-250" />
  <Vector X="650" Y="-100" />
  <Vector X="700" Y="50" />
</MissionTarget>
```

### Flag

Additional required attributes:

- `Team`: team id that owns this flag.

Constraints:

- `Team` must exist
- `Team` must not be spectators
- child elements are rejected; plain text content is currently ignored

Example:

```xml
<Flag Name="BlueFlag" X="1200" Y="0" Radius="20" Gravity="0" Team="1" />
```

### DominationPoint

Additional required attributes:

- `Team`: configured initial team affiliation of the domination point.
- `DominationRadius`: radius around the point that counts for domination gameplay.

Constraints:

- `Team` must exist
- `Team` must not be spectators
- `DominationRadius > 0`
- child elements are rejected; plain text content is currently ignored
- `Domination` and `ScoreCountdown` are runtime-only state and are therefore not editable XML attributes
- `QueryUnitXml(...)` returns the configured team, not a transient runtime owner that may have changed during gameplay

Example:

```xml
<DominationPoint Name="Center" X="0" Y="0" Radius="24" Gravity="0" Team="0" DominationRadius="250" />
```

Typical usage:

```csharp
await cluster.SetUnit("""<Buoy Name="Info" X="100" Y="200" Radius="12" Gravity="0" Message="Hello." />""");

await cluster.SetUnit("""
<MissionTarget Name="Alpha" X="400" Y="-250" Radius="20" Gravity="0" Team="0" SequenceNumber="3">
  <Vector X="500" Y="-250" />
  <Vector X="650" Y="-100" />
</MissionTarget>
""");

await cluster.SetUnit("""<Flag Name="BlueFlag" X="1200" Y="0" Radius="20" Gravity="0" Team="1" />""");

await cluster.SetUnit("""<DominationPoint Name="Center" X="0" Y="0" Radius="24" Gravity="0" Team="0" DominationRadius="250" />""");

string xml = await cluster.QueryUnitXml("Alpha");
await cluster.RemoveUnit("Alpha");
```

## Failure Model

Protocol-level XML problems:

- exception: `InvalidArgumentGameException`
- typical reason: `AmbiguousXmlData`
- examples:
  - empty string
  - unreadable payload
  - malformed XML

Local argument validation before the request is sent:

- `RemoveUnit("")` and `QueryUnitXml("")` fail in the connector with `InvalidArgumentGameException`, `Reason == TooSmall`, parameter `name`

Semantic XML problems:

- exception: `InvalidXmlNodeValueGameException`
- fields:
  - `Reason`
  - `NodePath`
  - `Hint`

Examples:

- unknown attribute on `Region`:
  - `NodePath = "Region.UnknownAttribute"`
- missing `MissionTarget.Team`:
  - `NodePath = "MissionTarget.Team"`
- invalid `MissionTarget.SequenceNumber`:
  - `NodePath = "MissionTarget.SequenceNumber"`
- invalid team reference in a region:
  - `NodePath = "Region>Team.Id"`
- invalid numeric value:
  - `NodePath = "Galaxy>Team.ColorR"`
- invalid domination radius:
  - `NodePath = "DominationPoint.DominationRadius"`

Lookup failures:

- unknown unit name in `RemoveUnit(...)` or `QueryUnitXml(...)`:
  - exception: `InvalidArgumentGameException`
  - `Reason = EntityNotFound`
  - parameter = `unit`

For editor UX, show `Hint` directly and use `NodePath` to focus the failing field or tree node.
