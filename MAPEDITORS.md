# Flattiverse Map Editors

This document is for software that connects as a Flattiverse admin and edits galaxy metadata, teams, clusters, regions or editable map units through `Flattiverse.Connector`.

Relevant API entry points:

- [`Galaxy.Configure(...)`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs:454)
- [`Cluster.SetRegion(...)`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs:110)
- [`Cluster.RemoveRegion(...)`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs:128)
- [`Cluster.QueryRegions()`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs:147)
- [`Cluster.SetUnit(...)`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs:204)
- [`Cluster.RemoveUnit(...)`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs:221)
- [`Cluster.QueryUnitXml(...)`](/mnt/d/projects/fv/fv-connector/Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs:239)

## General Habits

- All validation that exists in the client also exists on the server. Do not trust local validation alone.
- Unknown XML attributes are rejected.
- Unknown XML child nodes are rejected.
- Empty or malformed XML fails with `InvalidArgumentGameException` and `Reason == AmbiguousXmlData`.
- Semantically invalid XML fails with `InvalidXmlNodeValueGameException`. Use `Reason`, `NodePath` and `Hint` for UI feedback.
- `QueryRegions()` and `QueryUnitXml(...)` return server-generated canonical XML. Prefer querying back after edits instead of reconstructing the final state locally.
- Region changes are not event-driven. Query them when your editor needs the current region set.

## Connection

Use an admin API key and connect to the full galaxy endpoint:

```csharp
Galaxy galaxy = await Galaxy.Connect(
    "wss://www.flattiverse.com/galaxies/1/api",
    "<admin-api-key>",
    "Pink");
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
  Maintenance="false">
  <Team Id="0" Name="Pink" ColorR="255" ColorG="0" ColorB="200" />
  <Team Id="1" Name="Green" ColorR="192" ColorG="255" ColorB="0" />
  <Cluster Id="0" Name="Playground" Start="true" Respawn="false" />
  <Cluster Id="1" Name="Arena" Start="false" Respawn="true" />
</Galaxy>
```

Allowed `Galaxy` attributes:

- `GameMode`
- `Name`
- `Description`
- `MaxPlayers`
- `MaxSpectators`
- `GalaxyMaxTotalShips`
- `GalaxyMaxClassicShips`
- `GalaxyMaxNewShips`
- `GalaxyMaxBases`
- `TeamMaxTotalShips`
- `TeamMaxClassicShips`
- `TeamMaxNewShips`
- `TeamMaxBases`
- `PlayerMaxTotalShips`
- `PlayerMaxClassicShips`
- `PlayerMaxNewShips`
- `PlayerMaxBases`
- `Maintenance`

Allowed `Team` attributes:

- `Id`
- `Name`
- `ColorR`
- `ColorG`
- `ColorB`

Allowed `Cluster` attributes:

- `Id`
- `Name`
- `Start`
- `Respawn`

Limits and constraints:

- team ids: `0..11`
- cluster ids: `0..23`
- galaxy name: `1..32` characters
- description: `0..4096` characters
- team and cluster names must satisfy the server name constraint
  - effectively at least `2` characters
  - no leading or trailing spaces
  - restricted character set
- cluster names and team names are limited to `32` characters

## Regions

Regions are edited per cluster. Region ids are local to the cluster.

Important behavior:

- region ids: `0..255`
- region names are optional
- region names may have up to `64` characters
- `Left < Right` must hold
- `Top < Bottom` must hold
- at least one `Team` child is required
- only `Team` child nodes are allowed
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

- `Id`
- `Name`
- `Left`
- `Top`
- `Right`
- `Bottom`

Allowed `Region > Team` attributes:

- `Id`

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

Not editable:

- `Shot`
- `ClassicShipPlayerUnit`
- `NewShipPlayerUnit`
- `Explosion`

General behavior:

- unit XML root node must be the unit type itself, for example `<Sun ... />`
- unit names are required and may have up to `128` characters
- `Name`, `X`, `Y`, `Radius`, `Gravity` are common required attributes
- `Type` values use the enum member names from the connector, for example `OceanWorld`, `RockyMoon` or `MetallicSlug`
- `Radius > 0`
- `Gravity >= 0`
- changing the unit kind for an existing name is allowed
- `QueryUnitXml(...)` returns the server-generated XML of exactly one unit

### Sun

Required attributes:

- `Name`
- `X`
- `Y`
- `Radius`
- `Gravity`
- `Energy`
- `Ions`
- `Neutrinos`
- `Heat`
- `Drain`

All resource values must be `>= 0`.

Example:

```xml
<Sun Name="Sol" X="0" Y="0" Radius="300" Gravity="2500" Energy="120" Ions="10" Neutrinos="5" Heat="25" Drain="0" />
```

### BlackHole

Required attributes:

- `Name`
- `X`
- `Y`
- `Radius`
- `Gravity`
- `GravityWellRadius`
- `GravityWellForce`

Constraints:

- `GravityWellRadius > 0`
- `GravityWellForce >= 0`

Example:

```xml
<BlackHole Name="Void" X="2000" Y="-500" Radius="120" Gravity="6000" GravityWellRadius="900" GravityWellForce="1800" />
```

### Planet

Required attributes:

- `Name`
- `X`
- `Y`
- `Radius`
- `Gravity`
- `Type`
- `Metal`
- `Carbon`
- `Hydrogen`
- `Silicon`

Constraints:

- `Type` must be a known `PlanetType`
- all resource values must be `>= 0`

Example:

```xml
<Planet Name="Gaia" X="800" Y="150" Radius="90" Gravity="250" Type="OceanWorld" Metal="20" Carbon="40" Hydrogen="15" Silicon="25" />
```

### Moon

Required attributes:

- `Name`
- `X`
- `Y`
- `Radius`
- `Gravity`
- `Type`
- `Metal`
- `Carbon`
- `Hydrogen`
- `Silicon`

Constraints:

- `Type` must be a known `MoonType`
- all resource values must be `>= 0`

Example:

```xml
<Moon Name="Luna" X="950" Y="170" Radius="30" Gravity="40" Type="RockyMoon" Metal="5" Carbon="1" Hydrogen="0" Silicon="4" />
```

### Meteoroid

Required attributes:

- `Name`
- `X`
- `Y`
- `Radius`
- `Gravity`
- `Type`
- `Metal`
- `Carbon`
- `Hydrogen`
- `Silicon`

Constraints:

- `Type` must be a known `MeteoroidType`
- all resource values must be `>= 0`

Example:

```xml
<Meteoroid Name="M1" X="-400" Y="900" Radius="18" Gravity="4" Type="MetallicSlug" Metal="12" Carbon="0" Hydrogen="0" Silicon="3" />
```

### Buoy

Required attributes:

- `Name`
- `X`
- `Y`
- `Radius`
- `Gravity`

Optional attributes:

- `Message`

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

Required attributes:

- `Name`
- `X`
- `Y`
- `Radius`
- `Gravity`
- `Team`

Allowed child nodes:

- `Vector`

Allowed `Vector` attributes:

- `X`
- `Y`

Constraints:

- `Team` must exist
- `Team` must not be spectators
- `Vector` children are optional
- only `Vector` child nodes are allowed
- `Vector` child nodes must not contain nested elements
- at most `65535` vectors

Example:

```xml
<MissionTarget Name="Alpha" X="400" Y="-250" Radius="20" Gravity="0" Team="0">
  <Vector X="500" Y="-250" />
  <Vector X="650" Y="-100" />
  <Vector X="700" Y="50" />
</MissionTarget>
```

Typical usage:

```csharp
await cluster.SetUnit("""<Buoy Name="Info" X="100" Y="200" Radius="12" Gravity="0" Message="Hello." />""");

await cluster.SetUnit("""
<MissionTarget Name="Alpha" X="400" Y="-250" Radius="20" Gravity="0" Team="0">
  <Vector X="500" Y="-250" />
  <Vector X="650" Y="-100" />
</MissionTarget>
""");

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
- invalid team reference in a region:
  - `NodePath = "Region>Team.Id"`
- invalid numeric value:
  - `NodePath = "Galaxy>Team.ColorR"`

For editor UX, show `Hint` directly and use `NodePath` to focus the failing field or tree node.
