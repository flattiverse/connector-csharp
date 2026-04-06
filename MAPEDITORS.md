# Flattiverse Map Editors

This document is for software that connects as a Flattiverse admin and edits galaxy metadata, teams, clusters, regions or editable map units through `Flattiverse.Connector`.

Relevant API entry points:

- [`Galaxy.Configure(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.cs#L475)
- [`Galaxy.QueryAclAccounts(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.AccessControl.cs)
- [`Galaxy.AddAclAccount(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.AccessControl.cs)
- [`Galaxy.RemoveAclAccount(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Galaxy.AccessControl.cs)
- [`Cluster.SetRegion(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L120)
- [`Cluster.RemoveRegion(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L138)
- [`Cluster.QueryRegions()`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L157)
- [`Cluster.QueryEditableUnits(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L178)
- [`Cluster.SetUnit(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L201)
- [`Cluster.RemoveUnit(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L218)
- [`Cluster.QueryUnitXml(...)`](Flattiverse.Connector/Flattiverse.Connector/GalaxyHierarchy/Cluster.cs#L236)

## Terminology

- This document edits persistent galaxy configuration and persistent world units.
- Player-owned `Controllable` and `ControllableInfo` data are runtime/session objects and are not edited through map XML.
- `QueryUnitXml(...)` always returns canonical configuration of one world unit, never owner-side runtime or transient live state.

## General Habits

- All validation that exists in the client also exists on the server. Do not trust local validation alone.
- Unknown XML attributes are rejected.
- Unknown XML child elements are rejected. Plain text content is currently ignored by the server parser.
- Empty or malformed XML fails with `InvalidArgumentGameException` and `Reason == AmbiguousXmlData`.
- Semantically invalid XML fails with `InvalidXmlNodeValueGameException`. Use `Reason`, `NodePath` and `Hint` for UI feedback.
- `QueryUnitXml(...)` returns server-generated canonical XML.
- `QueryEditableUnits(...)` is the discovery endpoint for editable units. Use it when you do not already know the unit name.
- `QueryEditableUnits(...)` includes editable units that are currently invisible in the live cluster view, for example inactive power-ups waiting for respawn.
- `QueryRegions()` reconstructs XML locally in the connector from the binary region query reply. Prefer querying back after edits instead of maintaining a second serializer in your editor.
- Region changes are not event-driven. Query them when your editor needs the current region set.
- `Galaxy.Configure(...)` does not reject target types based on `GameMode`. You may keep targets in the map while changing the galaxy game mode.
- Galaxy ACLs are persistent galaxy metadata, even though they are managed through dedicated admin commands instead of `Galaxy.Configure(...)` XML.
- If `Galaxy.Tournament is not null`, all map-editing commands are rejected with `TournamentMapEditingLockedGameException`.
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
  GalaxyMaxModernShips="8"
  TeamMaxTotalShips="8"
  TeamMaxClassicShips="8"
  TeamMaxModernShips="8"
  PlayerMaxTotalShips="1"
  PlayerMaxClassicShips="1"
  PlayerMaxModernShips="1"
  Maintenance="false"
  RequiresSelfDisclosure="false">
  <Team Id="0" Name="Pink" ColorR="255" ColorG="0" ColorB="200" />
  <Team Id="1" Name="Green" ColorR="192" ColorG="255" ColorB="0" />
  <Cluster Id="0" Name="Playground" Start="true" Respawn="false" />
  <Cluster Id="1" Name="Arena" Start="false" Respawn="true" />
</Galaxy>
```

Allowed `Galaxy` attributes:

- `GameMode`: selects the galaxy-wide objective logic, for example `Mission`, `ShootTheFlag`, or `Domination`.
- `Name`: display name of the galaxy shown to clients.
- `Description`: longer descriptive text for UIs and selection screens.
- `MaxPlayers`: maximum number of regular player connections allowed at the same time.
- `MaxSpectators`: maximum number of spectator connections allowed at the same time.
- `GalaxyMaxTotalShips`: galaxy-wide cap for all player-owned controllables combined.
- `GalaxyMaxClassicShips`: galaxy-wide cap for classic-ship controllable runtimes (`ClassicShipPlayerUnit` on the wire).
- `GalaxyMaxModernShips`: galaxy-wide cap for new-ship controllable runtimes (`ModernShipPlayerUnit` on the wire).
- `TeamMaxTotalShips`: per-team cap for all player-owned controllables combined.
- `TeamMaxClassicShips`: per-team cap for classic-ship controllable runtimes (`ClassicShipPlayerUnit` on the wire).
- `TeamMaxModernShips`: per-team cap for new-ship controllable runtimes (`ModernShipPlayerUnit` on the wire).
- `PlayerMaxTotalShips`: per-player cap for all owned controllables combined.
- `PlayerMaxClassicShips`: per-player cap for classic-ship controllable runtimes (`ClassicShipPlayerUnit` on the wire).
- `PlayerMaxModernShips`: per-player cap for new-ship controllable runtimes (`ModernShipPlayerUnit` on the wire).
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
- `Respawn`: if `true`, this cluster is preferred when dead player-owned controllable runtimes are continued.

## Galaxy ACLs

Galaxy ACLs restrict which persistent account ids may enter one galaxy as a normal player or as an admin.

Important behavior:

- The player ACL and admin ACL are completely separate.
- If one ACL list is empty, that login kind is open to all accounts.
- If one ACL list contains at least one account id, that login kind becomes a whitelist.
- Spectator logins are not affected.
- ACL changes are persistent and survive galaxy restarts because they are stored in the database.
- ACL changes do not disconnect already connected sessions. The new rules apply only to later login attempts.
- `AddAclAccount(...)` is idempotent for already listed accounts.
- `RemoveAclAccount(...)` is idempotent for missing accounts.
- `AddAclAccount(...)` fails with `SpecifiedElementNotFoundGameException` if the account id does not exist.

Typical usage:

```csharp
Flattiverse.Connector.Account.Account[] playerAcl = await galaxy.QueryAclAccounts(GalaxyAclKind.Player);
await galaxy.AddAclAccount(GalaxyAclKind.Player, 1234);
await galaxy.AddAclAccount(GalaxyAclKind.Admin, 42);
await galaxy.RemoveAclAccount(GalaxyAclKind.Player, 1234);
```

Typical workflow:

- Use `Galaxy.QueryAccounts(...)` if your editor/tool first needs a searchable account catalog.
- Use `Galaxy.QueryAclAccounts(...)` to display the current whitelist of one login kind.
- Use `Galaxy.AddAclAccount(...)` and `Galaxy.RemoveAclAccount(...)` to persist changes immediately.
- Expect denied future logins to fail with `PlayerAccessRestrictedGameException` or `AdminAccessRestrictedGameException`, depending on the attempted login kind.

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
- `CurrentField`
- `Nebula`
- `Storm`
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

Not editable:

- owner-side controllables and public controllable roster entries are never map XML
- `Shot`
- `Interceptor`
- `Rail`
- `StormCommencingWhirl`
- `StormActiveWhirl`
- `ClassicShipPlayerUnit`
- `ModernShipPlayerUnit`
- `InterceptorExplosion`
- `Explosion`

General behavior:

- unit XML root node must be the unit type itself, for example `<Sun ... />`
- unit names are required and may have up to `128` characters
- unit names currently do not use the strict team/cluster name constraint
  - effective length: `1..128` characters
  - no additional character whitelist is applied by the server today
  - leading or trailing spaces are currently accepted
- `Name`, `X`, `Y`, `Radius`, `Gravity` are common required attributes
- all power-up unit kinds additionally require `Amount > 0` and `RespawnTicks > 0`
- `Type` values use the enum member names from the connector, for example `OceanWorld`, `RockyMoon` or `MetallicSlug`
- `Radius > 0`
- `Gravity >= 0`
- changing the unit kind for an existing name is allowed only if the existing same-name unit is editable
- `QueryUnitXml(...)` returns the server-generated XML of exactly one unit
- `QueryUnitXml(...)` returns canonical configuration XML, not transient runtime state
- for orbiting units, `QueryUnitXml(...)` returns the configured orbit center in `X` / `Y`, not the current runtime position
- `Switch.Team` is required and may also be the spectators team id `12`
- omitting `Gate.RestoreTicks` disables automatic restore

Common editable unit attributes:

- `Name`: stable unique unit name within the cluster; updates with the same name replace the existing editable unit.
- `X`: unit center X coordinate in world space.
- `Y`: unit center Y coordinate in world space.
- `Radius`: physical radius of the unit used for collisions, masking and scan geometry.
- `Gravity`: gravitational pull emitted by the unit; `0` disables gravity while keeping the unit otherwise present.

## Orbiting

All current editable unit kinds may contain `Orbit` child elements. The server keeps the child order, and the order is semantically relevant because each orbit segment starts at the configured center or at the result of the previous orbit segment.

Allowed `Orbit` attributes:

- `Distance`: orbit radius contributed by that segment. Must be `>= 0`.
- `StartAngle`: angle in degrees at galaxy tick `0`. Any finite float is accepted.
- `RotationTicks`: full rotation period in ticks. Must be non-zero. Positive values rotate in the positive direction, negative values reverse the direction.

Important behavior:

- a unit without `Orbit` children is static; its runtime position is exactly `X` / `Y`
- a unit with at least one `Orbit` child is a runtime `Steady` unit and sends per-tick runtime position updates
- for orbiting units, `X` / `Y` is the configured center returned by `QueryUnitXml(...)`
- the runtime position that players scan is not stored back into XML; it is derived from the current galaxy tick
- players only receive the orbit configuration once the unit reaches full visibility, because it is part of the full `0x32` state

Runtime position at galaxy tick `t`:

```text
phaseTick = t % abs(rotationTicks)
angle = startAngle + 360 * phaseTick / rotationTicks
angle = angle modulo 360, normalized into [0, 360)
offset = Vector.FromAngleLength(angle, distance)
```

For multiple orbit segments:

```text
position = configuredCenter

for each Orbit child in document order:
    position += orbitOffsetAtTick(t)
```

This means clients can preview or predict the position themselves as soon as they know both the current galaxy tick and the full orbit chain.

Example:

```xml
<Gate Name="OrbitGate" X="120" Y="0" Radius="12" Gravity="0" LinkId="17" DefaultClosed="true">
  <Orbit Distance="80" StartAngle="15" RotationTicks="240" />
  <Orbit Distance="12" StartAngle="-90" RotationTicks="-40" />
</Gate>
```

Shared power-up attributes:

- `Amount`: positive payload amount transferred by that specific power-up kind on pickup.
- `RespawnTicks`: required positive tick delay before the collected or exploded power-up may reappear.
- `RespawnPlayerDistance`: optional non-negative clear radius around the power-up before respawn is allowed. Default: `800`.

Shared power-up runtime rule:

- power-ups are never configured as permanently gone; after pickup or explosion they disappear from the active game and re-enter through the normal visible-unit lifecycle once `RespawnTicks` elapsed and no active player-owned controllable runtime is inside `RespawnPlayerDistance`.

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
- `Heat`: thermal radiation emitted by the sun. Each point drains `15` energy per tick from affected ships; unpaid heat overflows into radiation.
- `Drain`: ionizing radiation emitted by the sun. Together with heat overflow it deals `0.125` hull damage per tick per point after armor reduction.

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

### CurrentField

Additional required attributes:

- `Mode`: one of `Directional` or `Relative`.
- `FlowX`: X component of the fixed directional flow vector.
- `FlowY`: Y component of the fixed directional flow vector.
- `RadialForce`: radial movement component relative to the field center.
- `TangentialForce`: tangential movement component relative to the field center.

Constraints and behavior:

- all five attributes are required, even if one mode ignores part of them
- unused components should normally be set to `0`
- `Mode="Directional"` uses `FlowX` and `FlowY`
- `Mode="Relative"` uses `RadialForce` and `TangentialForce`
- `RadialForce < 0` pulls inward, `RadialForce > 0` pushes outward
- `TangentialForce != 0` adds circular motion around the center
- current fields are non-solid, non-masking, and do not block spawning
- the configured `Radius` is also the effect radius

Examples:

```xml
<CurrentField Name="EastJet" X="80" Y="-20" Radius="40" Gravity="0" Mode="Directional" FlowX="0.18" FlowY="0" RadialForce="0" TangentialForce="0" />
```

```xml
<CurrentField Name="Whirlpool" X="-120" Y="60" Radius="55" Gravity="0" Mode="Relative" FlowX="0" FlowY="0" RadialForce="-0.08" TangentialForce="0.16" />
```

### Nebula

Additional required attributes:

- `Hue`: color angle in degrees. The server normalizes it into `[0; 360)`.

Constraints and behavior:

- `Hue` may be any finite float; values outside `[0; 360)` are wrapped by the server
- nebulas are non-solid, non-masking, visible map units
- the configured `Radius` is the collection radius used by the current `NebulaCollector`

Example:

```xml
<Nebula Name="AmberCloud" X="120" Y="-40" Radius="24" Gravity="0" Hue="30" />
```

### Storm

Additional required attributes:

- `SpawnChancePerTick`: probability in `[0; 1]` that this storm spawns one announcing whirl during a tick.
- `MinAnnouncementTicks`: minimum duration of the announcing whirl phase.
- `MaxAnnouncementTicks`: maximum duration of the announcing whirl phase.
- `MinActiveTicks`: minimum duration of the active whirl phase.
- `MaxActiveTicks`: maximum duration of the active whirl phase.
- `MinWhirlRadius`: minimum radius of generated whirls.
- `MaxWhirlRadius`: maximum radius of generated whirls.
- `MinWhirlSpeed`: minimum initial speed of generated whirls.
- `MaxWhirlSpeed`: maximum initial speed of generated whirls.
- `MinWhirlGravity`: minimum gravity of the active whirl phase.
- `MaxWhirlGravity`: maximum gravity of the active whirl phase.
- `Damage`: damage applied by each active-whirl hit.

Constraints and behavior:

- `SpawnChancePerTick` must stay within `[0; 1]`
- `MaxAnnouncementTicks >= MinAnnouncementTicks`
- `MinActiveTicks > 0` and `MaxActiveTicks >= MinActiveTicks`
- `MinWhirlRadius > 0` and `MaxWhirlRadius >= MinWhirlRadius`
- `MinWhirlSpeed >= 0` and `MaxWhirlSpeed >= MinWhirlSpeed`
- `MinWhirlGravity >= 0` and `MaxWhirlGravity >= MinWhirlGravity`
- `Damage >= 0`
- storms are non-solid, non-masking, editable map units
- storms do not block spawning
- a successful spawn creates one runtime-only `StormCommencingWhirl`
- `StormCommencingWhirl` is non-masking, non-solid, reaches full visibility after `4` seen ticks, and exposes remaining announcement ticks in full visibility
- once the announcing lifetime ends, the server removes that unit and creates a same-name `StormActiveWhirl`
- `StormActiveWhirl` is masking, non-solid, reaches full visibility after `4` seen ticks, and exposes remaining active ticks plus `Damage`
- active whirls hit player-owned controllable runtimes through mobile collision, not through solid collision
- each active whirl may hit the same player-owned controllable runtime at most once every `10` ticks
- each hit applies `Damage` through the normal shield -> armor -> hull path and randomizes ship movement to a random direction of length `6`

Example:

```xml
<Storm Name="BlueStorm" X="-180" Y="60" Radius="48" Gravity="0" SpawnChancePerTick="0.03" MinAnnouncementTicks="12" MaxAnnouncementTicks="18" MinActiveTicks="18" MaxActiveTicks="28" MinWhirlRadius="7" MaxWhirlRadius="11" MinWhirlSpeed="0.14" MaxWhirlSpeed="0.24" MinWhirlGravity="0.003" MaxWhirlGravity="0.008" Damage="3.5" />
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
- `Metal`, `Carbon`, `Hydrogen`, and `Silicon` are the current non-depleting mining yields of this body

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
- `Metal`, `Carbon`, `Hydrogen`, and `Silicon` are the current non-depleting mining yields of this body

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
- `Metal`, `Carbon`, `Hydrogen`, and `Silicon` are the current non-depleting mining yields of this body

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

### WormHole

Additional required attributes:

- `TargetCluster`: numeric id of the destination cluster.
- `TargetLeft`, `TargetTop`, `TargetRight`, `TargetBottom`: destination spawn rectangle in the target cluster.

Validation notes:

- `TargetCluster` must point to an existing cluster in the same galaxy.
- `TargetRight > TargetLeft`
- `TargetBottom > TargetTop`
- worm holes are non-solid and non-masking; player-owned controllables only jump when they explicitly trigger their jump drive while touching exactly one worm hole
- destination data is only disclosed to players after the worm hole reaches full visibility

Example:

```xml
<WormHole Name="AlphaToBeta" X="-60" Y="0" Radius="15" Gravity="0" TargetCluster="1" TargetLeft="-210" TargetTop="-10" TargetRight="-190" TargetBottom="10" />
```

### MissionTarget

Additional required attributes:

- `Team`: owning or assigned team id for the target.

Optional attributes:

- `SequenceNumber`: order index used by mission logic to sequence targets.

Allowed child elements:

- `Orbit`: orbit segment applied before the mission target's own waypoint list.
- `Vector`: one path or waypoint node associated with the mission target.

Allowed `Vector` attributes:

- `X`: waypoint X coordinate in world space.
- `Y`: waypoint Y coordinate in world space.

Constraints:

- `Team` must exist
- `Team` must not be spectators
- `SequenceNumber` defaults to `0` if omitted; `QueryUnitXml(...)` writes it back explicitly
- `SequenceNumber` must be in `[0; 65535]`
- `Vector` children are optional
- `Orbit` and `Vector` child elements are allowed
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

Optional attributes:

- `GraceTicks`: number of ticks the flag stays inactive after a hit. Defaults to `0`.

Constraints:

- `Team` must exist
- `Team` must not be spectators
- `GraceTicks >= 0`
- only `Orbit` child elements are allowed; plain text content is currently ignored
- `Active` is runtime-only state, becomes `false` during grace time, can be scanned, and is therefore not an editable XML attribute

Example:

```xml
<Flag Name="BlueFlag" X="1200" Y="0" Radius="20" Gravity="0" Team="1" GraceTicks="120" />
```

### DominationPoint

Additional required attributes:

- `Team`: configured initial team affiliation of the domination point.
- `DominationRadius`: radius around the point that counts for domination gameplay.

Constraints:

- `Team` must exist
- `Team` must not be spectators
- `DominationRadius > 0`
- only `Orbit` child elements are allowed; plain text content is currently ignored
- `Domination` and `ScoreCountdown` are runtime-only state and are therefore not editable XML attributes
- `QueryUnitXml(...)` returns the configured team, not a transient runtime owner that may have changed during gameplay

Example:

```xml
<DominationPoint Name="Center" X="0" Y="0" Radius="24" Gravity="0" Team="0" DominationRadius="250" />
```

### Switch

Additional required attributes:

- `Team`: team id allowed to trigger the switch; `12` (`Spectators`) means every player shot may trigger it.
- `LinkId`: unsigned gate-group id in `0..65535`.
- `Range`: switch search radius in world units.
- `CooldownTicks`: minimum number of ticks between two successful triggers.
- `Mode`: one of `Toggle`, `Open`, `Close`.

Constraints:

- `Team` must reference an existing team and may be `12`
- `LinkId` must be in `0..65535`
- `Range >= 0`
- `CooldownTicks >= 0`
- `Mode` must be one of the listed enum values

Runtime behavior:

- switches are solid and masking
- only explosions created by player-owned controllables can trigger a switch
- `Team="12"` means any player-owned controllable may trigger the switch
- for all other teams, only a player-owned controllable of exactly that team may trigger the switch
- all gates in the same cluster whose center distance is `< Range + Gate.Radius` and whose `LinkId` matches are affected

Example:

```xml
<Switch Name="BlueSwitch" X="120" Y="-40" Radius="10" Gravity="0" Team="12" LinkId="17" Range="160" CooldownTicks="40" Mode="Toggle" />
```

### Gate

Additional required attributes:

- `LinkId`: unsigned gate-group id in `0..65535`.
- `DefaultClosed`: `true` or `false`; this is the gate's regular state.

Optional attributes:

- `RestoreTicks`: if present, the gate automatically returns to `DefaultClosed` after that many ticks.

Constraints:

- `LinkId` must be in `0..65535`
- `RestoreTicks`, when present, must be `> 0`

Runtime behavior:

- gates are solid, masking, and spawn-blocking only while closed
- omitting `RestoreTicks` means the gate stays in its switched state until another switch affects it
- `QueryUnitXml(...)` never includes current open/closed runtime state, only configuration

Examples:

```xml
<Gate Name="BlueGateA" X="260" Y="-40" Radius="14" Gravity="0" LinkId="17" DefaultClosed="true" />
```

```xml
<Gate Name="BlueGateB" X="320" Y="-40" Radius="14" Gravity="0" LinkId="17" DefaultClosed="true" RestoreTicks="80" />
```

Typical usage:

```csharp
await cluster.SetUnit("""<Buoy Name="Info" X="100" Y="200" Radius="12" Gravity="0" Message="Hello." />""");

await cluster.SetUnit("""<CurrentField Name="EastJet" X="80" Y="-20" Radius="40" Gravity="0" Mode="Directional" FlowX="0.18" FlowY="0" RadialForce="0" TangentialForce="0" />""");

await cluster.SetUnit("""<Storm Name="BlueStorm" X="-180" Y="60" Radius="48" Gravity="0" SpawnChancePerTick="0.03" MinAnnouncementTicks="12" MaxAnnouncementTicks="18" MinActiveTicks="18" MaxActiveTicks="28" MinWhirlRadius="7" MaxWhirlRadius="11" MinWhirlSpeed="0.14" MaxWhirlSpeed="0.24" MinWhirlGravity="0.003" MaxWhirlGravity="0.008" Damage="3.5" />""");

await cluster.SetUnit("""
<MissionTarget Name="Alpha" X="400" Y="-250" Radius="20" Gravity="0" Team="0" SequenceNumber="3">
  <Vector X="500" Y="-250" />
  <Vector X="650" Y="-100" />
</MissionTarget>
""");

await cluster.SetUnit("""<Flag Name="BlueFlag" X="1200" Y="0" Radius="20" Gravity="0" Team="1" GraceTicks="120" />""");

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
