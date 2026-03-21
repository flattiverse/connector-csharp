# Flattiverse Protocol Notes For Connector Implementers

This document covers the wire protocol and the connector-relevant runtime behavior.

Use [README.md](README.md) for the normal connector entry point.
Use [MAPEDITORS.md](MAPEDITORS.md) for XML-based map editing, validation rules, and exhaustive XML examples.

## Endpoint And Login

The public endpoint shape is currently:

```text
wss://www.flattiverse.com/galaxies/{universeId}/api
```

The WebSocket upgrade request uses query parameters:

* `version`
* `auth`
* optional `team`
* optional `runtimeDisclosure`
* optional `buildDisclosure`

Current protocol version:

```text
11
```

Examples:

```text
wss://www.flattiverse.com/galaxies/0/api?version=11&auth=<64-hex-api-key>&team=Blue
wss://www.flattiverse.com/galaxies/0/api?version=11&auth=<64-hex-api-key>
wss://www.flattiverse.com/galaxies/0/api?version=11&auth=<64-hex-api-key>&runtimeDisclosure=1234554321&buildDisclosure=543210123450
wss://www.flattiverse.com/galaxies/0/api?version=11&auth=0000000000000000000000000000000000000000000000000000000000000000
```

Important details:

* `auth` is always a 64-character lowercase/uppercase hex string representing 32 bytes.
* The all-zero API key is the special spectator login.
* The team-less form is used for spectator and admin logins.
* Normal players may also omit `team`; in that case the server auto-selects the non-spectator team with the fewest currently connected normal players. Ties are resolved by the smallest team id.
* `runtimeDisclosure` is a fixed 10-nibble hexadecimal string. Each nibble declares one runtime automation aspect in this order: `EngineControl`, `Navigation`, `ScannerControl`, `WeaponAiming`, `WeaponTargetSelection`, `ResourceControl`, `FleetControl`, `MissionControl`, `LoadoutControl`, `Chat`.
* Runtime disclosure nibble values: `0=Unsupported`, `1=Manual`, `2=Assisted`, `3=Automated`, `4=Autonomous`, `5=AiControlled`.
* `buildDisclosure` is a fixed 12-nibble hexadecimal string. Each nibble declares one build-assistance aspect in this order: `SoftwareDesign`, `UI`, `UniverseRendering`, `Input`, `EngineControl`, `Navigation`, `ScannerControl`, `WeaponSystems`, `ResourceControl`, `FleetControl`, `MissionControl`, `Chat`.
* Build disclosure nibble values: `0=None`, `1=SearchOnly`, `2=FreeLlm`, `3=PaidLlm`, `4=IntegratedLlm`, `5=AgenticTool`.
* If a galaxy has `RequiresSelfDisclosure=true`, regular player logins must provide both disclosure strings. Admin and spectator logins are currently exempt.
* A player account may have only one active galaxy session at a time. The same lock covers both `apiPlayer` and `apiAdmin` of that account, so an admin login also blocks a normal login and vice versa while a fresh session heartbeat exists.
* A plain HTTP request without WebSocket upgrade currently returns HTTP `426 Upgrade Required`.

## Initial Success And Failure Semantics

If the connection fails after the WebSocket was already accepted, the server sends exactly one protocol packet and then closes the socket:

* `command = 0xFF`
* `session = 0x01`
* payload starts with the exception code and may contain more payload depending on the exception type

If the connection succeeds, the server does **not** immediately answer with the session-1 reply first.
Instead it sends initial state packets first and only completes activation afterwards:

1. `0x00` ping challenge
1. `0x01` galaxy snapshot
1. repeated team blocks: `0x02` team snapshot, then `0x04` team score snapshot
1. `0x06` cluster snapshots
1. `0x0B` compiled-with message
1. repeated existing player blocks: `0x10` player create, `0x12` player score, then zero or more `0x20` controllable infos for that player
1. login-kind-specific self packets
1. for a normal player login: `0x10` create packet for the newly connected player itself, then `0x12` initial score packet
1. for a spectator or admin login: `0x10` create packet for the synthetic local spectator/admin player itself
1. initial visible units
1. for a normal player login: visible `0x30` unit create packets, with immediate `0x32` for units that are already full in that same tick
1. for a spectator or admin login: every initial unit currently arrives as `0x30` followed immediately by `0x32`
1. session reply with `session = 0x01` and payload `byte playerId`

Do not assume that "the first packet after connect" is the activation reply.

## Packet Framing

Every WebSocket message is binary. A single WebSocket frame may contain one or more protocol packets.

Each protocol packet starts with this header:

```text
byte   command
byte   session
ushort payloadSize
```

Then exactly `payloadSize` bytes of payload follow.

Important details:

* `payloadSize` is little-endian.
* All integer and floating-point values are little-endian.
* `float` is IEEE-754 single precision.
* A reader must keep advancing packet-by-packet inside one WebSocket frame until no more full packets remain.

### Primitive Encodings

Currently used scalar encodings:

* `byte`: 1 byte
* `ushort`: 2 bytes, little-endian
* `int`: 4 bytes, little-endian
* `uint`: 4 bytes, little-endian
* `float`: 4 bytes, little-endian IEEE-754
* `Vector`: two consecutive `float`s: `X`, then `Y`

### String Encoding

Strings are UTF-8 encoded and length-prefixed like this:

* empty string: one byte `0x00`
* non-empty string with UTF-8 byte length `< 255`: `byte length`, then payload
* non-empty string with UTF-8 byte length `>= 255`: `0xFF`, then `ushort length`, then payload

Notes:

* The protocol does not distinguish between `null` and empty string on the wire. Both serialize as `0x00`.
* The length is the UTF-8 byte length, not the character count.

## Session Semantics

`session` is used for request/reply correlation:

* `session = 0` means unsolicited packets or non-session commands
* `session > 0` means session-bound request/reply traffic

Normal session replies:

* `command = 0x00`
* `session = same session id as the request`
* payload may be empty

Exceptional session replies:

* `command = 0xFF`
* `session = same session id as the request`
* payload contains a serialized `GameException`

The reference connector also treats connection shutdown as a hard failure of all open sessions.
If the socket dies while requests are outstanding, all waiting requests should complete exceptionally rather than hanging forever.

## Reserved IDs And Current Limits

Current limits in the reference server and connector:

* team ids: `0..11`
* spectators pseudo-team id: `12`
* cluster ids: `0..23`
* region ids inside one cluster: `0..255`

Important spectator detail:

* The server does not send the spectators team as a normal `0x02` team-upsert packet.
* Nevertheless player packets may reference team id `12`.
* The reference connector therefore synthesizes a local team with id `12`, name `Spectators`, color `(128, 128, 128)`.

If you write your own connector, you should handle team id `12` explicitly.

## Exceptions

The reference implementation uses numeric exception ids across both server-side `GameException` packets and connector-local failures.
Server-side exceptions are transported on the wire in `command = 0xFF` packets.

Current server-side on-wire codes:

* `0x02` `InvalidProtocolVersionGameException`
* `0x03` `AuthFailedGameException`
* `0x04` `WrongAccountStateGameException`
* `0x05` `TeamSelectionFailedGameException`
* `0x06` `SelfDisclosureRequiredGameException`
* `0x07` `PersistenceUnavailableGameException`
* `0x08` `ServerFullOfPlayerKindGameException`
* `0x09` `AccountAlreadyLoggedInGameException`
* `0x0D` `InvalidDataGameException`
* `0x10` `SpecifiedElementNotFoundGameException`
* `0x12` `InvalidArgumentGameException`
* `0x13` `PermissionFailedGameException`
* `0x14` `FloodcontrolTriggeredGameException`
* `0x15` `UnitConstraintViolationGameException`
* `0x16` `InvalidXmlNodeValueGameException`
* `0x17` `ControllableIsClosingGameException`
* `0x18` `AvatarNotAvailableGameException`
* `0x20` `YouNeedToContinueFirstGameException`
* `0x21` `YouNeedToDieFirstGameException`
* `0x22` `AllStartLocationsAreOvercrowded`
* `0x30` `CanOnlyShootOncePerTickGameException`

Current connector-local-only codes:

* `0x01` `CantConnectGameException`
* `0x0C` `SessionsExhaustedException`
* `0x0F` `ConnectionTerminatedGameException`
* `0x11` `CantCallThisConcurrentGameException`

Payload details for the important structured exceptions:

* `0x04`: `byte accountStatus`
* `0x08`: `byte playerKind`
* `0x09`: no additional payload
* `0x12`: `byte invalidArgumentKind`, `string parameter`
* `0x16`: `byte invalidArgumentKind`, `string nodePath`, `string hint`

`0x06` is used when the galaxy requires self-disclosure and a regular player login omitted `runtimeDisclosure` or `buildDisclosure`.

`0x07` is used when a player or admin login requires persistent account/session storage and that storage is currently unavailable.

`0x05` is currently used for both cases:

* an explicit `team` query parameter was specified but does not resolve to an existing team
* no `team` query parameter was specified and the server found no non-spectator team that could be auto-selected

`0x17` is currently used when a client sends `0x84 Continue` for a controllable that has already entered the closing phase.

`0x18` is currently used when a client requests `0xC7` / `0xC8` for a player that has no avatar available.

Current `InvalidArgumentKind` values:

* `0x00` `Unknown`
* `0x01` `TooSmall`
* `0x02` `TooLarge`
* `0x03` `NameConstraint`
* `0x04` `ChatConstraint`
* `0x05` `AmbiguousXmlData`
* `0xFB` `EntityNotFound`
* `0xFC` `NameInUse`
* `0xFD` `ContainedNaN`
* `0xFE` `ContainedInfinity`
* `0xFF` `Valid`

Note:

* The "must be alive" / "must be dead" validation path is represented on the wire as dedicated exceptions `0x20` and `0x21`, not as additional `InvalidArgumentKind` values.

## Server To Client Packets

The following packet commands are currently sent from the galaxy to the client:

* `0x00`: ping request, payload `ushort challenge`
* `0x01`: create or update galaxy snapshot
* `0x02`: create or update team
* `0x03`: deactivate team
* `0x04`: update team score
* `0x06`: create or update cluster
* `0x07`: deactivate cluster
* `0x0B`: compiled-with message
* `0x10`: create player
* `0x11`: update player
* `0x12`: update player score
* `0x1F`: deactivate player
* `0x20`: create controllable info
* `0x21`: mark controllable info alive
* `0x22`: mark controllable info dead with generic reason
* `0x23`: mark controllable info dead by collision
* `0x24`: mark controllable info dead by other player
* `0x25`: update controllable info score
* `0x2F`: final close of controllable info
* `0x30`: create visible unit
* `0x31`: update visible unit movement state
* `0x32`: update visible unit runtime state
* `0x3E`: admin altered a previously known unit
* `0x3F`: deactivate visible unit
* `0x80`: create controllable
* `0x81`: controllable deceased
* `0x82`: controllable runtime update and alive
* `0x8F`: controllable finally closed
* `0xC0`: `GalaxyTickEvent`
* `0xC4`: public galaxy chat message
* `0xC5`: team chat message
* `0xC6`: private chat message

Some important snapshot payloads:

### `0x01` Galaxy Snapshot

Payload order:

```text
byte   gameMode
string name
string description
byte   maxPlayers
ushort maxSpectators
ushort galaxyMaxTotalShips
ushort galaxyMaxClassicShips
ushort galaxyMaxNewShips
ushort galaxyMaxBases
ushort teamMaxTotalShips
ushort teamMaxClassicShips
ushort teamMaxNewShips
ushort teamMaxBases
byte   playerMaxTotalShips
byte   playerMaxClassicShips
byte   playerMaxNewShips
byte   playerMaxBases
byte   maintenanceFlag
byte   requiresSelfDisclosureFlag
```

`maintenanceFlag` and `requiresSelfDisclosureFlag` are `0x00` or `0x01`.

### `0x02` Team Snapshot

Payload order:

```text
byte   teamId
byte   red
byte   green
byte   blue
string name
```

### `0x04` Team Score Update

Payload order:

```text
byte teamId
uint playerKills
uint playerDeaths
uint friendlyKills
uint friendlyDeaths
uint npcKills
uint npcDeaths
uint neutralDeaths
uint mission
```

### `0x06` Cluster Snapshot

Payload order:

```text
byte   clusterId
string name
byte   flags
```

Cluster flags:

* bit `0x01`: start cluster
* bit `0x02`: respawn cluster

### `0x0B` Compiled-With Message

Payload order:

```text
byte   maxPlayersSupported
string compileSymbol
```

Current values:

* `16`, `PLAYERS_16`
* `64`, `PLAYERS_64`
* `192`, `PLAYERS_192`

### `0x10` Player Create

Payload order:

```text
byte   playerId
byte   playerKind
byte   teamId
string accountName
float  pingMilliseconds
byte   adminFlag
int    rank
long   playerKills
long   playerDeaths
long   friendlyKills
long   friendlyDeaths
long   npcKills
long   npcDeaths
long   neutralDeaths
byte   hasAvatar
byte   disclosureFlags
[5 bytes packed runtimeDisclosure if disclosureFlags bit 0x01 is set]
[6 bytes packed buildDisclosure if disclosureFlags bit 0x02 is set]
```

`disclosureFlags` uses:

* bit `0x01`: `runtimeDisclosure` present
* bit `0x02`: `buildDisclosure` present

Packed disclosure bytes use high nibble first. Disclosure data is sent only in `0x10 Player Create`, not in `0x11 Player Update`.

### `0x11` Player Update

Payload order:

```text
byte  playerId
float pingMilliseconds
byte  adminFlag
int   rank
long  playerKills
long  playerDeaths
long  friendlyKills
long  friendlyDeaths
long  npcKills
long  npcDeaths
long  neutralDeaths
```

### `0x12` Player Score Update

Payload order:

```text
byte playerId
uint playerKills
uint playerDeaths
uint friendlyKills
uint friendlyDeaths
uint npcKills
uint npcDeaths
uint neutralDeaths
uint mission
```

### `0x25` Controllable Info Score Update

Payload order:

```text
byte controllablePlayerId
byte controllableId
uint playerKills
uint playerDeaths
uint friendlyKills
uint friendlyDeaths
uint npcKills
uint npcDeaths
uint neutralDeaths
uint mission
```

This score belongs to the registered controllable identified by `(controllablePlayerId, controllableId)`.
It is session-local, just like `0x12 Player Score Update`.

### `0x80` Controllable Create

Payload order:

```text
byte   controllableKind
byte   controllableId
string controllableName
Vector position
Vector movement
```

This packet is the owner's authoritative identity channel for a controllable. A player's own controllables are intentionally modeled separately from the visible-unit stream and therefore must be tracked from `0x80` / `0x82`, not from `0x30` / `0x32`.

Static subsystem capabilities such as battery maxima, energy-cell efficiencies, or scanner limits are currently not transmitted here.
The reference connector initializes them locally by `controllableKind`.

### `0x81` Controllable Deceased

Payload order:

```text
byte controllableId
```

### `0x82` Controllable Runtime Update

This packet is owner-only. It is the runtime state channel for the player's own controllables.
It remains available even while scanners are off and is the correct source for the owner's own position, movement, and subsystem runtime.
Death is signaled separately via `0x81`; `0x82` is only sent for living controllables.

Current subsystem runtime enums:

```text
SubsystemStatus:
    0x00 Off
    0x01 Worked
    0x02 Failed
    0x03 Upgrading

SubsystemSlot ranges:
    0x00..0x0F batteries
    0x10..0x17 cells
    0x18..0x1F hulls
    0x20..0x2F scanners
    0x30..0x3F engines
    0x40..0x4F shot launchers

Currently used slots:
    0x00 EnergyBattery
    0x01 IonBattery
    0x02 NeutrinoBattery
    0x10 EnergyCell
    0x11 IonCell
    0x12 NeutrinoCell
    0x18 Hull
    0x20 PrimaryScanner
    0x21 SecondaryScanner
    0x30 PrimaryEngine
    0x40 FrontShotLauncher
```

Common payload order:

```text
byte   controllableId
Vector position
Vector movement
float  energyBatteryCurrent
float  energyBatteryConsumedThisTick
byte   energyBatteryStatus
float  ionBatteryCurrent
float  ionBatteryConsumedThisTick
byte   ionBatteryStatus
float  neutrinoBatteryCurrent
float  neutrinoBatteryConsumedThisTick
byte   neutrinoBatteryStatus
float  energyCellCollectedThisTick
byte   energyCellStatus
float  ionCellCollectedThisTick
byte   ionCellStatus
float  neutrinoCellCollectedThisTick
byte   neutrinoCellStatus
float  hullCurrent
byte   hullStatus
```

The remaining payload depends on `controllableKind`.

Current `ClassicShipPlayerUnit` addition:

```text
byte   mainScannerActive
float  mainScannerCurrentWidth
float  mainScannerCurrentLength
float  mainScannerCurrentAngle
float  mainScannerTargetWidth
float  mainScannerTargetLength
float  mainScannerTargetAngle
byte   mainScannerStatus
float  mainScannerConsumedEnergyThisTick
float  mainScannerConsumedIonsThisTick
float  mainScannerConsumedNeutrinosThisTick
byte   secondaryScannerActive
float  secondaryScannerCurrentWidth
float  secondaryScannerCurrentLength
float  secondaryScannerCurrentAngle
float  secondaryScannerTargetWidth
float  secondaryScannerTargetLength
float  secondaryScannerTargetAngle
byte   secondaryScannerStatus
float  secondaryScannerConsumedEnergyThisTick
float  secondaryScannerConsumedIonsThisTick
float  secondaryScannerConsumedNeutrinosThisTick
Vector engineCurrent
Vector engineTarget
byte   engineStatus
float  engineConsumedEnergyThisTick
float  engineConsumedIonsThisTick
float  engineConsumedNeutrinosThisTick
Vector weaponRelativeMovement
ushort weaponTicks
float  weaponLoad
float  weaponDamage
byte   weaponStatus
float  weaponConsumedEnergyThisTick
float  weaponConsumedIonsThisTick
float  weaponConsumedNeutrinosThisTick
```

Notes:

* Battery maxima and energy-cell efficiencies are currently initialized locally by controllable kind and are not sent on the wire.
* `*CellCollectedThisTick` is the post-efficiency amount that was actually loaded through that cell during the current server tick.
* `hullCurrent` is the current hull integrity after that tick's damage resolution. The reference `ClassicShip` currently initializes hull locally with a maximum of `50`.
* `engineCurrent` is the currently applied engine impulse for this tick. `engineTarget` is the persisted requested impulse.
* `weapon*` describes the shot request the server actually processed in this tick. If the weapon was off or did not have a queued shot, the runtime values are zeroed.
* `mainScannerStatus == Worked` / `secondaryScannerStatus == Worked` tells owner-side tools whether the server actually paid and executed that scan in the current tick.
* `SecondaryScanner` is currently a disabled subsystem on the reference ClassicShip. Its runtime block is still present and currently zeroed.
* `Current*` is the server-applied runtime state. `Target*` is the server-side target configuration.
* The current placeholder scanner energy cost is `PI * length^2 * width / 360 * 0.000282943`, so the reference `ClassicShip` maximum scan `90 x 300` costs about `20` energy per tick.
* The reference connector does not expose additional wire events for subsystem runtime. Instead it parses these owner-only runtime blocks and raises connector-local subsystem events:
  * `BatterySubsystemEvent`
  * `EnergyCellSubsystemEvent`
  * `HullSubsystemEvent`
  * `ScannerSubsystemEvent`
  * `ClassicShipEngineSubsystemEvent`
  * `ShotWeaponSubsystemEvent`
* Those connector-local events currently use `EventKind` values `0x80..0x85`, but no additional wire commands were introduced.

### `0x30` Visible Unit Create

For `PlayerKind.Player`, the unit visibility packets `0x30` / `0x31` / `0x32` / `0x3F` are driven by the server-side scan analysis of the player's active ships. Spectators and admins still receive the global unit feed, but the reference server now uses the same reduced/full packet split there as well and follows every newly visible unit with `0x32`.

The visible-unit stream is for other units only. The owner never receives the player's own controllables back through visibility packets, even if those controllables still participate in masking and other geometry during scan analysis.

Current runtime rule:

* only active scanners with sufficient battery charge produce visibility
* scanner angles are interpreted relative to the owning unit's current facing angle
* passive energy and radiation intake is processed independently from scanner visibility and therefore still works while scanners are off
* passive sun intake uses `maskingFactor * 1 / (1 + d/60)^sqrt(2)` with `d` as edge-to-edge distance; transfers below `1%` are ignored completely
* a player's own controllables are not reported back through `0x30` / `0x31` / `0x32` / `0x3F`; the owner must use the separate `0x80` / `0x82` controllable channel for them
* visibility promotion is unit-specific: the reference server keeps a unit in reduced visibility for `ReducedVisibilityTicks` consecutive seen ticks before promoting it to full visibility
* when a unit first appears, the server always sends `0x30`; if the unit is already full in that same tick, `0x32` follows immediately
* regular `0x32` updates are only sent to players that currently see the unit in full visibility

All unit create packets start with:

```text
byte   clusterId
string unitName
byte   unitKind
```

The remaining payload depends on `unitKind`.

Current reduced `0x30` payloads and promotion thresholds:

* `Sun (0x00)`: `steady unit payload`; `ReducedVisibilityTicks = 100`
* `BlackHole (0x01)`: `steady unit payload`; `ReducedVisibilityTicks = 200`
* `Planet (0x08)`: `steady unit payload`, `byte planetType`; `ReducedVisibilityTicks = 250`
* `Moon (0x09)`: `steady unit payload`, `byte moonType`; `ReducedVisibilityTicks = 250`
* `Meteoroid (0x0A)`: `steady unit payload`, `byte meteoroidType`; `ReducedVisibilityTicks = 250`
* `Buoy (0x10)`: `steady unit payload`; `ReducedVisibilityTicks = 20`
* `MissionTarget (0x14)`: `steady unit payload`, `byte teamId`; `ReducedVisibilityTicks = 20`
* `Flag (0x15)`: `steady unit payload`, `byte teamId`; `ReducedVisibilityTicks = 0`
* `DominationPoint (0x16)`: `steady unit payload`, `byte teamId`, `float dominationRadius`; `ReducedVisibilityTicks = 50`
* `Shot (0xE0)`: `byte ownerPlayerId`, `byte ownerControllableId`, `ushort ticks`, `Vector position`, `Vector movement`; `ReducedVisibilityTicks = 20`
* `ClassicShipPlayerUnit (0xF0)`: `byte playerId`, `byte controllableId`, `Vector position`, `Vector movement`; `ReducedVisibilityTicks = 100`
* `Explosion (0xFF)`: `byte ownerPlayerId`, `byte ownerControllableId`, `float load`, `float damage`, `Vector position`; `ReducedVisibilityTicks = 0`

Notes:

* `steady unit payload` means `Vector position`, `float radius`, `float gravity`
* `planetType` / `moonType` / `meteoroidType` are intentionally already part of reduced visibility
* For `Shot` and `Explosion`, `ownerPlayerId = 0xFF` and `ownerControllableId = 0xFF` mean "no owner". The reference connector treats `ownerPlayerId >= 192` as ownerless.
* `Explosion` and `Flag` are effectively full immediately, but still participate in the same `0x30` + optional `0x32` visibility protocol

### `0x31` Visible Unit Movement State Update

All `0x31` packets currently start with:

```text
byte   clusterId
string unitName
```

Current runtime rule:

* `0x31` is sent while a unit remains visible at all, i.e. in reduced or full visibility
* `0x32` is only sent once a unit is in full visibility, or immediately when it is promoted to full in the same tick

Current `0x31` payloads:

* steady units (`Sun`, `BlackHole`, `Planet`, `Moon`, `Meteoroid`, `Buoy`, `MissionTarget`, `Flag`, `DominationPoint`): no additional payload
* `Shot (0xE0)`:
  ```text
  ushort ticks
  Vector position
  Vector movement
  ```
* `ClassicShipPlayerUnit (0xF0)`:
  ```text
  Vector position
  Vector movement
  ```
* `Explosion (0xFF)`: no additional payload; in the reference connector `0x31` only advances the local explosion phase

### `0x32` Visible Unit Runtime State Update

All `0x32` packets currently start with:

```text
byte   clusterId
string unitName
```

The remaining payload depends on the unit type.

Current `0x32` payloads:

* `Sun (0x00)`:
  ```text
  float energy
  float ions
  float neutrinos
  float heat
  float drain
  ```
* `BlackHole (0x01)`:
  ```text
  float gravityWellRadius
  float gravityWellForce
  ```
* `Planet (0x08)` / `Moon (0x09)` / `Meteoroid (0x0A)`:
  ```text
  float metal
  float carbon
  float hydrogen
  float silicon
  ```
* `Buoy (0x10)`:
  ```text
  string? message
  ```
* `MissionTarget (0x14)`:
  ```text
  int    sequenceNumber
  ushort vectorCount
  vectorCount * Vector
  ```
* `Flag (0x15)`: no additional payload
* `DominationPoint (0x16)`:
  ```text
  byte teamId
  int  domination
  int  scoreCountdown
  ```
* `Shot (0xE0)`:
  ```text
  float load
  float damage
  ```
* `ClassicShipPlayerUnit (0xF0)` common player-unit detail block:
  ```text
  byte  energyBatteryExists
  float energyBatteryMaximum
  float energyBatteryCurrent
  float energyBatteryConsumedThisTick
  byte  energyBatteryStatus
  byte  ionBatteryExists
  float ionBatteryMaximum
  float ionBatteryCurrent
  float ionBatteryConsumedThisTick
  byte  ionBatteryStatus
  byte  neutrinoBatteryExists
  float neutrinoBatteryMaximum
  float neutrinoBatteryCurrent
  float neutrinoBatteryConsumedThisTick
  byte  neutrinoBatteryStatus
  byte  energyCellExists
  float energyCellEfficiency
  float energyCellCollectedThisTick
  byte  energyCellStatus
  byte  ionCellExists
  float ionCellEfficiency
  float ionCellCollectedThisTick
  byte  ionCellStatus
  byte  neutrinoCellExists
  float neutrinoCellEfficiency
  float neutrinoCellCollectedThisTick
  byte  neutrinoCellStatus
  byte  hullExists
  float hullMaximum
  float hullCurrent
  byte  hullStatus
  byte  mainScannerExists
  float mainScannerMaximumWidth
  float mainScannerMaximumLength
  float mainScannerWidthSpeed
  float mainScannerLengthSpeed
  float mainScannerAngleSpeed
  byte  mainScannerActive
  float mainScannerCurrentWidth
  float mainScannerCurrentLength
  float mainScannerCurrentAngle
  float mainScannerTargetWidth
  float mainScannerTargetLength
  float mainScannerTargetAngle
  byte  mainScannerStatus
  float mainScannerConsumedEnergyThisTick
  float mainScannerConsumedIonsThisTick
  float mainScannerConsumedNeutrinosThisTick
  byte  secondaryScannerExists
  float secondaryScannerMaximumWidth
  float secondaryScannerMaximumLength
  float secondaryScannerWidthSpeed
  float secondaryScannerLengthSpeed
  float secondaryScannerAngleSpeed
  byte  secondaryScannerActive
  float secondaryScannerCurrentWidth
  float secondaryScannerCurrentLength
  float secondaryScannerCurrentAngle
  float secondaryScannerTargetWidth
  float secondaryScannerTargetLength
  float secondaryScannerTargetAngle
  byte  secondaryScannerStatus
  float secondaryScannerConsumedEnergyThisTick
  float secondaryScannerConsumedIonsThisTick
  float secondaryScannerConsumedNeutrinosThisTick
  byte  engineExists
  float engineMaximum
  Vector engineCurrent
  Vector engineTarget
  byte  engineStatus
  float engineConsumedEnergyThisTick
  float engineConsumedIonsThisTick
  float engineConsumedNeutrinosThisTick
  byte  weaponExists
  float weaponMinimumRelativeMovement
  float weaponMaximumRelativeMovement
  ushort weaponMinimumTicks
  ushort weaponMaximumTicks
  float weaponMinimumLoad
  float weaponMaximumLoad
  float weaponMinimumDamage
  float weaponMaximumDamage
  Vector weaponRelativeMovement
  ushort weaponTicks
  float weaponLoad
  float weaponDamage
  byte  weaponStatus
  float weaponConsumedEnergyThisTick
  float weaponConsumedIonsThisTick
  float weaponConsumedNeutrinosThisTick
  ```
* `Explosion (0xFF)`: no additional payload

For unit and controllable payload details, inspect the current reference implementation types in `Flattiverse.Connector/Units` and `Flattiverse.Connector/GalaxyHierarchy`.

### `0x3E` Unit Altered By Admin

Payload order:

```text
byte   clusterId
string unitName
```

Current runtime rule:

* the server sends `0x3F` first and then `0x3E`
* recipients are all admins, all spectators, and all players that have seen the unit before during their current connection
* `0x3E` is intentionally only a cache invalidation hint and does not carry any further unit state

## Client To Server Commands

The following packet commands are currently used by clients to call server commands.

### General

* `0x00` for all player kinds: ping reply, payload `ushort challenge`

The client must echo the challenge value it most recently received in a `0x00` ping request.

### Admin / Map Editing

* `0x04`: configure galaxy, payload `string xml`
* `0x24`: set or overwrite one region, payload `byte clusterId`, `string xml`
* `0x25`: remove one region, payload `byte clusterId`, `byte regionId`
* `0x26`: query all regions of one cluster, payload `byte clusterId`
* `0x28`: set or overwrite one editable unit, payload `byte clusterId`, `string xml`
* `0x29`: remove one editable unit, payload `byte clusterId`, `string unitName`
* `0x2A`: query XML of one editable unit, payload `byte clusterId`, `string unitName`

Reply format of `0x26`:

```text
ushort regionCount

repeat regionCount times:
    byte   id
    string name
    float  left
    float  top
    float  right
    float  bottom
    uint   teamMask
```

Reply format of `0x2A`:

```text
string xml
```

Important notes:

* `0x26` is binary on the wire. The reference connector converts the binary reply into `<Regions>...</Regions>` XML only as a convenience API.
* `0x29` and `0x2A` resolve the unit by `(clusterId, unitName)` on the server side.
* `0x28`, `0x29`, and `0x2A` only work for unit types with `CanBeEdited = true`.
* Empty or unreadable XML on protocol level is rejected as `0x12 InvalidArgumentGameException` with `InvalidArgumentKind.AmbiguousXmlData` and parameter name `xml`.
* `0x2A` rejects non-editable units with `0x16 InvalidXmlNodeValueGameException`.
* Editable target kinds currently include `MissionTarget`, `Flag`, and `DominationPoint`.
* XML semantics, whitelists, validation rules, and examples are intentionally documented in [MAPEDITORS.md](MAPEDITORS.md), not duplicated here.

### Player Commands

* `0x80`: register classic ship, payload `string name`, reply payload `byte controllableId`
* `0x84`: continue controllable, payload `byte controllableId`
* `0x85`: suicide controllable, payload `byte controllableId`
* `0x87`: move controllable, payload `byte controllableId`, `Vector movement`
* `0x88`: shoot, payload `byte controllableId`, `Vector movement`, `ushort ticks`, `float load`, `float damage`
* `0x89`: configure scanner, payload `byte controllableId`, `byte scannerId`, `float width`, `float length`, `float angle`
* `0x8A`: activate scanner, payload `byte controllableId`, `byte scannerId`
* `0x8B`: deactivate scanner, payload `byte controllableId`, `byte scannerId`
* `0x8F`: request close of controllable, payload `byte controllableId`
* `0xC4`: send galaxy chat, payload `string message`
* `0xC5`: send team chat, payload `byte teamId`, `string message`
* `0xC6`: send private chat, payload `byte playerId`, `string message`
* `0xC7`: download small avatar, payload `byte playerId`, reply payload `byte[] avatarBytes`
* `0xC8`: download big avatar, payload `byte playerId`, reply payload `byte[] avatarBytes`

Notes:

* `0xC5` resolves the target team by id on the server side.
* `0xC6` resolves the target player by id on the server side.
* `0xC7` / `0xC8` resolve the target player by id on the server side and return the avatar bytes cached on that server-side player at login time.
* `0x10 Player Create` contains `hasAvatar`, so clients can avoid sending `0xC7` / `0xC8` for players without avatars.
* If `hasAvatar == 0`, the reference connector throws locally before sending `0xC7` / `0xC8`.
* The server still validates `0xC7` / `0xC8` and returns `0x18 AvatarNotAvailableGameException` if no avatar is available.
* `0x8F` is not an immediate removal. The server may keep a living controllable alive for 30 ticks before finally closing it, and dead controllables stay registered until no shot/explosion references remain.
* String validation for names and chat messages happens on the server even if a client already validated locally.
* `0x88` currently validates `movement.Length` in `[0.1; 3]`, `ticks` in `[2; 140]`, `load` in `[2.5; 25]`, and `damage` in `[1; 20]`.
* The current reference ClassicShip supports `scannerId = 0` for `MainScanner`. `scannerId = 1` addresses `SecondaryScanner`, which currently does not exist and therefore returns `0x10 SpecifiedElementNotFoundGameException`.
* The configured scanner angle is relative to the ship's current facing. `0` points straight ahead, positive values rotate counter-clockwise in world space.

## Practical Compatibility Notes

If you write your own connector, these details matter in practice:

* Do not trust packet ordering beyond what is stated above.
* Always parse all packets contained in one WebSocket frame.
* Treat connection shutdown as failure of all open requests.
* Handle the synthetic spectators team locally or some player packets will reference an unknown team id.
* Do not assume that XML helper APIs from the reference connector correspond 1:1 to wire format. Region queries are the current example: wire is binary, helper API returns XML.
