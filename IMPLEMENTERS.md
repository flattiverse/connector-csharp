# Flattiverse Protocol Notes For Connector Implementers

This document covers the wire protocol and the connector-relevant runtime behavior.

Use [README.md](README.md) for the normal connector entry point.
Use [MAPEDITORS.md](MAPEDITORS.md) for XML-based map editing, validation rules, and exhaustive XML examples.

## Connector Terminology

The connector intentionally models three different views that must not be conflated:

* `ControllableInfo`: public roster entry of one player-owned controllable. It survives deaths until the server finally closes that registration.
* `Controllable`: owner-only runtime handle of one registered controllable of the local player. Commands such as `Continue()`, `Suicide()`, scanner control, jump-drive usage, and subsystem inspection act on this channel.
* visible unit / `PlayerUnit`: cluster-side scan result for a currently visible runtime object in the world. This is what other players, spectators, and admins see through `0x30` / `0x31` / `0x32`.

In protocol terms, `0x20..0x2F` describe the public controllable roster, `0x80..0x8F` describe the owner's own controllables, and `0x30..0x3F` describe currently visible world units.

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
31
```

Examples:

```text
wss://www.flattiverse.com/galaxies/0/api?version=31&auth=<64-hex-api-key>&team=Blue
wss://www.flattiverse.com/galaxies/0/api?version=31&auth=<64-hex-api-key>
wss://www.flattiverse.com/galaxies/0/api?version=31&auth=<64-hex-api-key>&runtimeDisclosure=1234554321&buildDisclosure=543210123450
wss://www.flattiverse.com/galaxies/0/api?version=31&auth=0000000000000000000000000000000000000000000000000000000000000000
```

Important details:

* `auth` is always a 64-character lowercase/uppercase hex string representing 32 bytes.
* The all-zero API key is the special spectator login.
* The team-less form is used for spectator and admin logins.
* Normal players may also omit `team`; in that case the server auto-selects the non-spectator team with the fewest currently connected normal players. Ties are resolved by the smallest team id.
* If a tournament is in `Commencing` or `Running`, spectator logins may be rejected with `0x37`, non-participants may be rejected with `0x36`, and a player login without `team` is auto-assigned to the configured tournament team of that account instead of using least-populated-team selection.
* Galaxies may additionally maintain separate ACL whitelists for normal players and admins. If one of those lists is empty, that login kind stays open to all accounts. If it contains at least one account id, only listed accounts may connect with that login kind. ACL changes do not disconnect existing sessions.
* `runtimeDisclosure` is a fixed 10-nibble hexadecimal string. Each nibble declares one runtime automation aspect in this order: `EngineControl`, `Navigation`, `ScannerControl`, `WeaponAiming`, `WeaponTargetSelection`, `ResourceControl`, `FleetControl`, `MissionControl`, `LoadoutControl`, `Chat`.
* Runtime disclosure nibble values: `0=Unsupported`, `1=Manual`, `2=Assisted`, `3=Automated`, `4=Autonomous`, `5=AiControlled`.
* `buildDisclosure` is a fixed 12-nibble hexadecimal string. Each nibble declares one build-assistance aspect in this order: `SoftwareDesign`, `UI`, `UniverseRendering`, `Input`, `EngineControl`, `Navigation`, `ScannerControl`, `WeaponSystems`, `ResourceControl`, `FleetControl`, `MissionControl`, `Chat`.
* Build disclosure nibble values: `0=None`, `1=SearchOnly`, `2=FreeLlm`, `3=PaidLlm`, `4=IntegratedLlm`, `5=AgenticTool`.
* If a galaxy has `RequiresSelfDisclosure=true`, regular player logins must provide both disclosure strings. Admin and spectator logins are currently exempt.
* If a galaxy has `RequiredAchievement="KEY"`, regular player logins must already own that case-insensitive achievement key. Admin and spectator logins are currently exempt.
* A player account may have only one active regular player galaxy session at a time. Admin logins do not claim or refresh `sessionGalaxy` / `sessionTeam`, do not update `datePlayedStart` / `datePlayedEnd`, and therefore do not block normal player logins or other admin logins of the same account.
* The galaxy sends a new `0x00` ping request roughly once per second after the previous one was answered.
* The galaxy disconnects a client when a ping reply stays outstanding for more than `5s`.
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
1. optional tournament packets: `0xD0` tournament snapshot and `0xD2` tournament system message if the galaxy currently has a configured tournament
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
* `0x23` `MissingAchievementGameException`
* `0x24` `TeamNotPlayableGameException`
* `0x30` `CanOnlyShootOncePerTickGameException`
* `0x31` `TournamentNotConfiguredGameException`
* `0x32` `TournamentAlreadyConfiguredGameException`
* `0x33` `TournamentWrongStageGameException`
* `0x34` `TournamentMapEditingLockedGameException`
* `0x35` `TournamentRegistrationClosedGameException`
* `0x36` `TournamentParticipantRequiredGameException`
* `0x37` `TournamentSpectatingForbiddenGameException`
* `0x38` `TournamentTeamMismatchGameException`
* `0x39` `TournamentModeNotAllowedGameException`
* `0x3A` `PlayerAccessRestrictedGameException`
* `0x3B` `AdminAccessRestrictedGameException`
* `0x3C` `StaticMapRebuildInProgressGameException`
* `0x3D` `StaticMapRebuildLockedGameException`
* `0x3E` `BinaryChatAckRequiredGameException`
* `0x3F` `ControllableIsRebuildingGameException`

Current connector-local-only codes:

* `0x01` `CantConnectGameException`
* `0x0C` `SessionsExhaustedException`

Structured exception payloads currently used by the server:

* `0x12`: `byte reason`, `string parameter`
* `0x16`: `byte reason`, `string nodePath`, `string hint`
* `0x23`: `string achievementName`
* `0x24`: `string teamName`

## Static Map Rebuild And Tick Profiling

The server now maintains an explicit static-map rebuild step for expensive per-segment still-data.

Important connector-visible behavior:

* On galaxy start an initial static-map rebuild runs automatically. Regular logins are rejected with `0x3C` until that first rebuild has finished.
* Admins may trigger a manual rebuild through `Galaxy.RebuildStaticMap()`.
* Starting a rebuild while a tournament exists fails with `0x3D`.
* Starting or configuring a tournament while a rebuild is running fails with `0x3C`.
* A later rebuild request aborts the currently running rebuild job and restarts it from scratch.
* Live simulation keeps using the previous static-map state until the new rebuild finishes and is swapped in atomically.

`0xC0 GalaxyTickEvent` now carries per-tick float timings in milliseconds:

* `ScanMs`
* `SteadyMs`
* `GravityMs`
* `EnginesMs`
* `LimitMs`
* `MovementMs`
* `CollisionsMs`
* `ActionsMs`
* `VisibilityMs`
* `TotalMs`

It also carries `RemainingStaticSegments`, which is `0` while no rebuild is running and otherwise reports the currently remaining segment-cache work.

`0x22` controllable-death replies now also use `PlayerUnitDestroyedReason.LostInDeepSpace (0x02)` when a player-owned runtime leaves the activated map area.
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

`0x18` is currently used when a client requests `0xF1` / `0xF2` for a player that has no avatar available.

`0x31` / `0x32` / `0x33` are tournament admin-action failures: no configured tournament, tournament already exists, or wrong tournament stage.

`0x34` is used when an admin tries to edit galaxy metadata, regions, or editable units while any tournament is configured.

`0x35` is used when a player tries to register or continue a controllable while tournament registration is closed.

`0x36` is used when an account is not part of the configured tournament participant lists.

`0x37` is used when a spectator login is attempted in a tournament stage that forbids spectating.

`0x38` is used when a player login or controllable registration targets a team that differs from the configured tournament team of that account.

`0x39` is used when an admin tries to configure a tournament for a galaxy whose `GameMode == Mission`.

`0x3A` is used when a normal player login is denied by the galaxy player ACL.

`0x3B` is used when an admin login is denied by the galaxy admin ACL.

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

* The "must be alive" / "must be dead" / "must not be rebuilding" validation path is represented on the wire as dedicated exceptions `0x20`, `0x21`, and `0x3F`, not as additional `InvalidArgumentKind` values.

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
* `0x80`: create or refresh controllable
* `0x81`: controllable deceased
* `0x82`: controllable runtime update
* `0x8E`: owner-only power-up collected event
* `0x8F`: controllable finally closed
* `0xC0`: `GalaxyTickEvent`
* `0xC1`: flag-scored system chat message
* `0xC2`: domination-point-scored system chat message
* `0xC3`: own-flag-hit system chat message
* `0xC4`: public galaxy chat message
* `0xC5`: team chat message
* `0xC6`: private chat message
* `0xC7`: mission-target-hit system chat message
* `0xC8`: system message
* `0xC9`: flag-reactivated system chat message
* `0xCA`: gate-switched event
* `0xCB`: gate-restored event
* `0xCC`: private binary chat message
* `0xCE`: MOTD message
* `0xD0`: create or update tournament snapshot
* `0xD1`: remove tournament snapshot
* `0xD2`: tournament system chat message

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
ushort galaxyMaxModernShips
ushort teamMaxTotalShips
ushort teamMaxClassicShips
ushort teamMaxModernShips
byte   playerMaxTotalShips
byte   playerMaxClassicShips
byte   playerMaxModernShips
byte   maintenanceFlag
byte   requiresSelfDisclosureFlag
string requiredAchievement
```

`maintenanceFlag` and `requiresSelfDisclosureFlag` are `0x00` or `0x01`.
`requiredAchievement == ""` means there is no requirement configured.

### `0x02` Team Snapshot

Payload order:

```text
byte   teamId
byte   red
byte   green
byte   blue
byte   playableFlag
string name
```

`playableFlag` is `0x00` or `0x01`.

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
int mission
```

Mission-mode note:

* when `GameMode == Mission`, `Team.Score` stays static; mission-mode kills, deaths and objectives do not mutate team score values

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

### `0xD0` Tournament Snapshot

Payload order:

```text
byte stage
byte mode
uint durationTicks
byte teamCount

repeat teamCount times:
    byte teamId
    byte participantCount

    repeat participantCount times:
        int    accountId
        string accountName
        byte   adminFlag
        int    rank
        long   playerKills
        long   playerDeaths
        byte   hasAvatarFlag
        byte   hasTournamentEloFlag
        if hasTournamentEloFlag != 0:
            float tournamentElo

byte historyCount
repeat historyCount times:
    byte winningTeamId
```

Tournament stage values:

* `0`: `Preparation`
* `1`: `Commencing`
* `2`: `Running`

Tournament mode values:

* `0`: `Solo`
* `1`: `BestOf3`
* `2`: `BestOf5`
* `3`: `BestOf7`
* `4`: `BestOf9`
* `5`: `BestOf11`

The ordered history entries represent already finished games. If the history contains three entries, the current live game is match number four.

### `0xD1` Tournament Removed

Payload is empty.

### `0xD2` Tournament System Chat Message

Payload order:

```text
string message
```

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
byte   stateFlags
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

`stateFlags` uses:

* bit `0x01`: player connection already disconnected; cleanup and controllable closeout may still be pending

Packed disclosure bytes use high nibble first. Disclosure data is sent only in `0x10 Player Create`, not in `0x11 Player Update`.

### `0x11` Player Update

Payload order:

```text
byte  playerId
float pingMilliseconds
byte  adminFlag
byte  stateFlags
int   rank
long  playerKills
long  playerDeaths
long  friendlyKills
long  friendlyDeaths
long  npcKills
long  npcDeaths
long  neutralDeaths
```

`stateFlags` currently uses:

* bit `0x01`: player connection already disconnected; cleanup and controllable closeout may still be pending

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
int mission
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
int mission
```

This score belongs to the registered controllable identified by `(controllablePlayerId, controllableId)`.
It is session-local, just like `0x12 Player Score Update`.

Mission-mode combat note:

* when `GameMode == Mission`, combat between two different players always counts as enemy combat for score updates and death reasons, even if both players currently belong to the same team
* combat between two controllables of the same player is reported as `PlayerUnitDestroyedReason.Suicided`
* collisions never award kills, regardless of `GameMode`

### `0xC1` Flag Scored System Chat

Payload order:

```text
byte   playerId
byte   controllableId
byte   flagTeamId
string flagName
```

### `0xC2` Domination Point Scored System Chat

Payload order:

```text
byte   teamId
string dominationPointName
```

### `0xC3` Own Flag Hit System Chat

Payload order:

```text
byte   playerId
byte   controllableId
byte   flagTeamId
string flagName
```

### `0xC7` Mission Target Hit System Chat

Payload order:

```text
byte   playerId
byte   controllableId
ushort missionTargetSequence
```

### `0xC8` System Message

Payload order:

```text
string message
```

### `0xC9` Flag Reactivated System Chat

Payload order:

```text
byte   flagTeamId
string flagName
```

### `0xCE` MOTD Message

Payload order:

```text
string message
```

### `0xCA` Gate Switched

Payload order:

```text
byte   clusterId
byte   hasInvoker
[byte  playerId]
[byte  controllableId]
string switchName
ushort gateCount

repeat gateCount times:
    string gateName
    byte   closedFlag
```

Notes:

* `hasInvoker` is `0x00` or `0x01`
* when `hasInvoker == 0x00`, the optional player and controllable ids are omitted
* `closedFlag == 0x01` means the gate ended closed; `0x00` means open

### `0xCB` Gate Restored

Payload order:

```text
byte   clusterId
string gateName
byte   closedFlag
```

### `0x80` Controllable Create

Base payload order:

```text
byte   controllableKind
byte   controllableId
byte   clusterId
string controllableName
Vector position
Vector movement
float  angle
float  angularVelocity
```

This packet is the owner's authoritative identity channel for a controllable. A player's own controllables are intentionally modeled separately from the visible-unit stream and therefore must be tracked from `0x80` / `0x82`, not from `0x30` / `0x32`.

After the base fields, the packet continues with the owner-visible static subsystem capability block and the initial owner runtime snapshot for that controllable kind.
This data is intentionally richer than the visible-unit stream because it initializes the local `Controllable` mirror immediately.
The server may also resend `0x80` for an already known controllable id after a subsystem tier change; when the controllable kind stays the same, connectors should refresh the existing owner object in place instead of treating it as a death/recreate cycle.

Owner-side subsystem block rules:

* every subsystem block starts with `byte existsFlag`
* if `existsFlag == 0`, no more bytes for that subsystem follow in `0x80`
* if `existsFlag != 0`, owner-side `0x80` sends `byte tier` immediately after the flag and then that subsystem's static/runtime create data
* visible `0x32` player-unit packets never send subsystem tiers
* fabricator subsystem blocks no longer send `MinimumRate`; the connector treats the minimum as implicit `0`

### `0x81` Controllable Deceased

Payload order:

```text
byte controllableId
```

### `0x82` Controllable Runtime Update

This packet is owner-only. It is the runtime state channel for the player's own controllables.
It remains available even while scanners are off and is the correct source for the owner's own position, movement, and subsystem runtime.
Death is signaled separately via `0x81`; `0x82` is only sent for living controllables. A subsystem rebuild no longer emits `0x81`; during the rebuild the controllable stays alive and the affected subsystem reports `SubsystemStatus.Upgrading`.

Runtime block rules:

* runtime blocks are only present for subsystems whose `Exists` flag was `true` in the last owner create packet
* shared and specific subsystem runtime blocks both follow that rule
* `StructureOptimizer` has owner create data in `0x80`, but no dedicated runtime block in `0x82`

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
    0x18..0x1F integrity subsystems
    0x20..0x2F scanners
    0x30..0x3F engines
    0x40..0x4F dynamic shot subsystems
    0x50..0x5F resource subsystems
    0x60..0x7F static modern weapon subsystems
    0x80..0x8F modern railguns

Currently used slots:
    0x00 EnergyBattery
    0x01 IonBattery
    0x02 NeutrinoBattery
    0x10 EnergyCell
    0x11 IonCell
    0x12 NeutrinoCell
    0x18 Hull
    0x19 Shield
    0x1A Armor
    0x1B Repair
    0x20 PrimaryScanner
    0x21 SecondaryScanner
    0x23 ModernScannerN
    0x24 ModernScannerNE
    0x25 ModernScannerE
    0x26 ModernScannerSE
    0x27 ModernScannerS
    0x28 ModernScannerSW
    0x29 ModernScannerW
    0x2A ModernScannerNW
    0x30 PrimaryEngine
    0x33 JumpDrive
    0x34 ModernEngineN
    0x35 ModernEngineNE
    0x36 ModernEngineE
    0x37 ModernEngineSE
    0x38 ModernEngineS
    0x39 ModernEngineSW
    0x3A ModernEngineW
    0x3B ModernEngineNW
    0x40 DynamicShotLauncher
    0x41 DynamicShotMagazine
    0x42 DynamicShotFabricator
    0x43 DynamicInterceptorLauncher
    0x44 DynamicInterceptorMagazine
    0x45 DynamicInterceptorFabricator
    0x46 Railgun
    0x50 Cargo
    0x51 ResourceMiner
    0x52 NebulaCollector
    0x60..0x67 StaticShotLauncherN..NW
    0x68..0x6F StaticShotMagazineN..NW
    0x70..0x77 StaticShotFabricatorN..NW
    0x78 StaticInterceptorLauncherE
    0x79 StaticInterceptorLauncherW
    0x7A StaticInterceptorMagazineE
    0x7B StaticInterceptorMagazineW
    0x7C StaticInterceptorFabricatorE
    0x7D StaticInterceptorFabricatorW
    0x80..0x87 ModernRailgunN..NW
```

Common payload order:

```text
byte   controllableId
byte   clusterId
Vector position
Vector movement
float  angle
float  angularVelocity
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
float  shieldCurrent
byte   shieldActive
float  shieldRate
byte   shieldStatus
float  shieldConsumedEnergyThisTick
float  shieldConsumedIonsThisTick
float  shieldConsumedNeutrinosThisTick
float  armorBlockedDirectDamageThisTick
float  armorBlockedRadiationDamageThisTick
byte   armorStatus
float  repairRate
byte   repairStatus
float  repairConsumedEnergyThisTick
float  repairConsumedIonsThisTick
float  repairConsumedNeutrinosThisTick
float  repairRepairedHullThisTick
float  cargoCurrentMetal
float  cargoCurrentCarbon
float  cargoCurrentHydrogen
float  cargoCurrentSilicon
float  cargoCurrentNebula
float  cargoNebulaHue
byte   cargoStatus
float  resourceMinerRate
byte   resourceMinerStatus
float  resourceMinerConsumedEnergyThisTick
float  resourceMinerConsumedIonsThisTick
float  resourceMinerConsumedNeutrinosThisTick
float  resourceMinerMinedMetalThisTick
float  resourceMinerMinedCarbonThisTick
float  resourceMinerMinedHydrogenThisTick
float  resourceMinerMinedSiliconThisTick
float  environmentHeatThisTick
float  environmentHeatEnergyCostThisTick
float  environmentHeatEnergyOverflowThisTick
float  environmentRadiationThisTick
float  environmentRadiationDamageBeforeArmorThisTick
float  environmentArmorBlockedDamageThisTick
float  environmentHullDamageThisTick
```

The remaining payload depends on `controllableKind`.

Current classic-ship controllable addition:

```text
float  nebulaCollectorRate
byte   nebulaCollectorStatus
float  nebulaCollectorConsumedEnergyThisTick
float  nebulaCollectorConsumedIonsThisTick
float  nebulaCollectorConsumedNeutrinosThisTick
float  nebulaCollectorCollectedThisTick
float  nebulaCollectorCollectedHueThisTick
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
Vector shotLauncherRelativeMovement
ushort shotLauncherTicks
float  shotLauncherLoad
float  shotLauncherDamage
byte   shotLauncherStatus
float  shotLauncherConsumedEnergyThisTick
float  shotLauncherConsumedIonsThisTick
float  shotLauncherConsumedNeutrinosThisTick
float  shotMagazineCurrentShots
byte   shotMagazineStatus
byte   shotFabricatorActive
float  shotFabricatorRate
byte   shotFabricatorStatus
float  shotFabricatorConsumedEnergyThisTick
float  shotFabricatorConsumedIonsThisTick
float  shotFabricatorConsumedNeutrinosThisTick
Vector interceptorLauncherRelativeMovement
ushort interceptorLauncherTicks
float  interceptorLauncherLoad
float  interceptorLauncherDamage
byte   interceptorLauncherStatus
float  interceptorLauncherConsumedEnergyThisTick
float  interceptorLauncherConsumedIonsThisTick
float  interceptorLauncherConsumedNeutrinosThisTick
float  interceptorMagazineCurrentShots
byte   interceptorMagazineStatus
byte   interceptorFabricatorActive
float  interceptorFabricatorRate
byte   interceptorFabricatorStatus
float  interceptorFabricatorConsumedEnergyThisTick
float  interceptorFabricatorConsumedIonsThisTick
float  interceptorFabricatorConsumedNeutrinosThisTick
byte   railgunDirection
byte   railgunStatus
float  railgunConsumedEnergyThisTick
float  railgunConsumedIonsThisTick
float  railgunConsumedNeutrinosThisTick
```

Notes:

* Battery maxima, cell efficiencies, hull maxima, shield maxima, armor reduction, cargo capacities, and similar static owner-side capabilities are sent during `0x80 Controllable Create`.
* `*CellCollectedThisTick` is the post-efficiency amount that was actually loaded through that cell during the current server tick.
* `hullCurrent` is the current hull integrity after that tick's damage resolution. The current reference classic ship uses `hullMaximum = 50`.
* `shieldCurrent` is the current shield integrity after that tick's damage resolution. The current reference classic ship uses `shieldMaximum = 20`.
* `shieldActive` / `shieldRate` is the owner-visible shield loading configuration. Exact per-tier costs and caps are exposed through `ShieldSubsystem.TierInfo` / `ShieldSubsystem.TierInfos`.
* `armorBlocked*` reports what the fixed armor reduction absorbed during the current tick. The current reference classic ship uses `Reduction = 0.5`.
* `repairRate == 0` means the repair subsystem is off. Exact per-tier costs and caps are exposed through `RepairSubsystem.TierInfo` / `RepairSubsystem.TierInfos`. If ship movement reaches `>= 0.1`, the server clears the configured rate and reports `repairStatus == Failed` for that tick.
* `cargoCurrent*` is the owner-visible stored resource state. The current reference classic ship uses `cargoMaximumMetal = cargoMaximumCarbon = cargoMaximumHydrogen = cargoMaximumSilicon = 20`.
* `resourceMinerRate == 0` means the miner is off. Exact per-tier costs and caps are exposed through `ResourceMinerSubsystem.TierInfo` / `ResourceMinerSubsystem.TierInfos`. It mines all in-range `Planet` / `Moon` / `Meteoroid` body resources edge-to-edge within `25`. Those body-side resource values are currently non-depleting.
* If the miner is active and no mineable resources are in range, the server still pays that tick, reports `resourceMinerStatus == Worked` with zero mined output, and then clears `resourceMinerRate` to `0`.
* If ship movement reaches `>= 0.1`, the server clears `resourceMinerRate` and reports `resourceMinerStatus == Failed` for that tick.
* `environment*` is an aggregated owner-only view of passive sun effects after the passive scan at the end of `DoBeforeCalculations()`. Heat drains `15` energy per point; unpaid heat overflows into radiation; radiation damage is reduced by armor before reaching hull.
* `engineCurrent` is the currently applied engine impulse for this tick. `engineTarget` is the persisted requested impulse.
* `shotLauncher*` describes the shot request the server actually processed in this tick. If the launcher had no queued shot, the runtime values are zeroed.
* `shotMagazineCurrentShots` is the owner-visible stored shot resource in `[0; 5]`. `shotMagazineStatus == Failed` means the launcher tried to consume one full shot this tick and the magazine did not have enough stored charge.
* `shotFabricator*` is the owner-visible production state. While active, the fabricator still consumes its configured energy at a full magazine; only the stored amount is clamped.
* `interceptorLauncher*` mirrors the shot launcher runtime but produces `Interceptor` instead of `Shot`. If the launcher had no queued interceptor, the runtime values are zeroed.
* `interceptorMagazineCurrentShots` is the owner-visible stored interceptor resource in `[0; 5]`. `interceptorMagazineStatus == Failed` means the interceptor launcher tried to consume one full interceptor this tick and the magazine did not have enough stored charge.
* `interceptorFabricator*` mirrors the shot fabricator runtime and fills the interceptor magazine independently.
* `railgunDirection` uses `0x00 None`, `0x01 Front`, `0x02 Back`. A worked tick consumed `300` energy and `1` metal and produced one `Rail` with relative speed `4`, fixed lifetime `250`, and damage `3.5 * |shipMovement + railRelativeMovement|`. Front/back are derived from the ship movement angle and fall back to world angle `0` while standing still.
* `mainScannerStatus == Worked` / `secondaryScannerStatus == Worked` tells owner-side tools whether the server actually paid and executed that scan in the current tick.
* `SecondaryScanner` is currently a disabled subsystem on the reference ClassicShip. Its runtime block is still present and currently zeroed.
* `Current*` is the server-applied runtime state. `Target*` is the server-side target configuration.
* Exact per-tier subsystem metadata is available directly in the connector on owner-side subsystems:
  * `Subsystem.Tier`
  * `Subsystem.TargetTier`
  * `Subsystem.RemainingTierChangeTicks`
  * `Subsystem.TierInfo`
  * `Subsystem.TierInfos`
* `SubsystemTierInfo` provides:
  * `StructuralLoad`
  * `UpgradeCost`
  * `DowngradeCost`
  * numeric `Properties`
  * `ResourceUsages` formulas grouped by subsystem component
  * `Description`
* Use `SubsystemTierInfo.CalculateResourceUsage(...)` to evaluate the metadata formulas for a concrete runtime operating point and compare them with the current tick's `Consumed*ThisTick` values.
* The reference connector does not expose additional wire events for subsystem runtime. Instead it parses these owner-only runtime blocks and raises connector-local subsystem events:
  * `BatterySubsystemEvent`
  * `EnergyCellSubsystemEvent`
  * `HullSubsystemEvent`
  * `ShieldSubsystemEvent`
  * `ArmorSubsystemEvent`
  * `RepairSubsystemEvent`
  * `CargoSubsystemEvent`
  * `ResourceMinerSubsystemEvent`
  * `DynamicScannerSubsystemEvent`
  * `ClassicShipEngineSubsystemEvent`
  * `DynamicShotLauncherSubsystemEvent`
  * `DynamicShotMagazineSubsystemEvent`
  * `DynamicShotFabricatorSubsystemEvent`
  * `DynamicInterceptorLauncherSubsystemEvent`
  * `DynamicInterceptorMagazineSubsystemEvent`
  * `DynamicInterceptorFabricatorSubsystemEvent`
  * `ClassicRailgunSubsystemEvent`
  * `ModernRailgunSubsystemEvent`
  * `ModernShipEngineSubsystemEvent`
* `EnvironmentDamageEvent`
* `CollectedPowerUpEvent`
* Those connector-local events currently use `EventKind` values `0x80..0x94`. These enum values are connector-local API identifiers, not an additional wire packet range. `CollectedPowerUpEvent` is backed by the dedicated owner-only wire packet `0x8E`; the subsystem events remain connector-local projections of `0x82`.
* Power-up respawn does not have a dedicated wire event. After pickup or explosion the server uses the normal `0x3F` visible-unit delete and later reintroduces the same unit name through `0x30` plus `0x32` once the respawn conditions are met.

### `0x30` Visible Unit Create

For `PlayerKind.Player`, the unit visibility packets `0x30` / `0x31` / `0x32` / `0x3F` are driven by the server-side scan analysis of the player's active ships. Spectators and admins still receive the global unit feed, but the reference server now uses the same reduced/full packet split there as well and follows every newly visible unit with `0x32`.

The visible-unit stream is for world-visible runtime objects only. The owner never receives the player's own controllables back through visibility packets, even if those controllables still participate in masking and other geometry during scan analysis.

Current runtime rule:

* only active scanners with sufficient battery charge produce visibility
* scanner angles are interpreted as absolute world angles
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
* `CurrentField (0x02)`: `steady unit payload`; `ReducedVisibilityTicks = 20`
* `Nebula (0x03)`: `steady unit payload`; `ReducedVisibilityTicks = 100`
* `Storm (0x20)`: `steady unit payload`; `ReducedVisibilityTicks = 20`
* `StormCommencingWhirl (0x21)`: `Vector position`, `Vector movement`, `float radius`, `float gravity`; `ReducedVisibilityTicks = 4`
* `StormActiveWhirl (0x22)`: `Vector position`, `Vector movement`, `float radius`, `float gravity`; `ReducedVisibilityTicks = 4`
* `Planet (0x08)`: `steady unit payload`, `byte planetType`; `ReducedVisibilityTicks = 250`
* `Moon (0x09)`: `steady unit payload`, `byte moonType`; `ReducedVisibilityTicks = 250`
* `Meteoroid (0x0A)`: `steady unit payload`, `byte meteoroidType`; `ReducedVisibilityTicks = 250`
* `Buoy (0x10)`: `steady unit payload`; `ReducedVisibilityTicks = 20`
* `WormHole (0x11)`: `steady unit payload`; `ReducedVisibilityTicks = 250`
* `MissionTarget (0x14)`: `steady unit payload`, `byte teamId`; `ReducedVisibilityTicks = 20`
* `Flag (0x15)`: `steady unit payload`, `byte teamId`; `ReducedVisibilityTicks = 0`
* `DominationPoint (0x16)`: `steady unit payload`, `byte teamId`, `float dominationRadius`; `ReducedVisibilityTicks = 50`
* `EnergyChargePowerUp (0x70)` / `IonChargePowerUp (0x71)` / `NeutrinoChargePowerUp (0x72)` / `MetalCargoPowerUp (0x73)` / `CarbonCargoPowerUp (0x74)` / `HydrogenCargoPowerUp (0x75)` / `SiliconCargoPowerUp (0x76)` / `ShieldChargePowerUp (0x77)` / `HullRepairPowerUp (0x78)` / `ShotChargePowerUp (0x79)`: `steady unit payload`; `ReducedVisibilityTicks = 0`
* `Switch (0x60)`: `steady unit payload`, `byte teamId`, `ushort linkId`, `float range`, `ushort cooldownTicks`, `byte mode`; `ReducedVisibilityTicks = 20`
* `Gate (0x61)`: `steady unit payload`, `ushort linkId`, `byte defaultClosedFlag`, `byte hasRestoreTicksFlag`, `[ushort restoreTicks if hasRestoreTicksFlag != 0]`; `ReducedVisibilityTicks = 20`
* `SpaceJellyFish (0x90)`: `mobile npc payload`; `ReducedVisibilityTicks = 20`
* `SpaceJellyFishSlime (0x91)`: `byte ownerPlayerId`, `byte ownerControllableId`, `ushort ticks`, `Vector position`, `Vector movement`, `float angle`, `float angularVelocity`; `ReducedVisibilityTicks = 20`
* `AiBase (0x92)`: `byte teamId`, `Vector position`, `float radius`; `ReducedVisibilityTicks = 20`
* `AiTurret (0x93)`: `byte teamId`, `Vector position`, `float radius`; `ReducedVisibilityTicks = 20`
* `AiFreighter (0x94)`: `mobile npc payload`; `ReducedVisibilityTicks = 20`
* `AiShip (0x95)`: `mobile npc payload`; `ReducedVisibilityTicks = 20`
* `AiProbe (0x96)`: `mobile npc payload`; `ReducedVisibilityTicks = 20`
* `Shot (0xE0)`: `byte ownerPlayerId`, `byte ownerControllableId`, `ushort ticks`, `Vector position`, `Vector movement`, `float angle`, `float angularVelocity`; `ReducedVisibilityTicks = 20`
* `Interceptor (0xE1)`: `byte ownerPlayerId`, `byte ownerControllableId`, `ushort ticks`, `Vector position`, `Vector movement`, `float angle`, `float angularVelocity`; `ReducedVisibilityTicks = 20`
* `Rail (0xE2)`: `byte ownerPlayerId`, `byte ownerControllableId`, `ushort ticks`, `Vector position`, `Vector movement`, `float angle`, `float angularVelocity`; `ReducedVisibilityTicks = 20`
* `ClassicShipPlayerUnit (0xF0)`: `byte playerId`, `byte controllableId`, `Vector position`, `Vector movement`, `float angle`, `float angularVelocity`; `ReducedVisibilityTicks = 100`
* `ModernShipPlayerUnit (0xF1)`: `byte playerId`, `byte controllableId`, `Vector position`, `Vector movement`, `float angle`, `float angularVelocity`; `ReducedVisibilityTicks = 100`
* `InterceptorExplosion (0xFE)`: `byte ownerPlayerId`, `byte ownerControllableId`, `float load`, `float damage`, `Vector position`; `ReducedVisibilityTicks = 0`
* `Explosion (0xFF)`: `byte ownerPlayerId`, `byte ownerControllableId`, `float load`, `float damage`, `Vector position`; `ReducedVisibilityTicks = 0`

Notes:

* `steady unit payload` means `Vector position`, `float radius`, `float gravity`
* `mobile npc payload` means `byte teamId`, `Vector position`, `Vector movement`, `float angle`, `float angularVelocity`, `float radius`
* for orbiting steady units, `position` in `0x30` is the current runtime position, not the configured map-editor center
* `planetType` / `moonType` / `meteoroidType` are intentionally already part of reduced visibility
* `Switch.mode` currently uses `0x00 Toggle`, `0x01 Open`, `0x02 Close`
* `Gate.hasRestoreTicksFlag == 0` means that no automatic restore is configured
* For projectile and explosion kinds, `ownerPlayerId = 0xFF` and `ownerControllableId = 0xFF` mean "no player owner". NPC-owned projectiles currently use the same ownerless marker on the wire.
* `Explosion`, `InterceptorExplosion`, and `Flag` are effectively full immediately, but still participate in the same `0x30` + optional `0x32` visibility protocol

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

* steady units (`Sun`, `BlackHole`, `CurrentField`, `Nebula`, `Planet`, `Moon`, `Meteoroid`, `Buoy`, `WormHole`, `MissionTarget`, `Flag`, `DominationPoint`, all visible power-up kinds, `Switch`, `Gate`):
  ```text
  Vector position
  Vector movement
  ```
  The reference server currently emits these `0x31` packets only for orbiting steady units.
* `Shot (0xE0)`:
  ```text
  ushort ticks
  Vector position
  Vector movement
  float  angle
  float  angularVelocity
  ```
* `Interceptor (0xE1)`:
  ```text
  ushort ticks
  Vector position
  Vector movement
  float  angle
  float  angularVelocity
  ```
* `Rail (0xE2)`:
  ```text
  ushort ticks
  Vector position
  Vector movement
  float  angle
  float  angularVelocity
  ```
* `StormCommencingWhirl (0x21)` / `StormActiveWhirl (0x22)`:
  ```text
  Vector position
  Vector movement
  ```
* `SpaceJellyFish (0x90)` / `AiFreighter (0x94)` / `AiShip (0x95)` / `AiProbe (0x96)`:
  ```text
  Vector position
  Vector movement
  float  angle
  float  angularVelocity
  ```
  `angularVelocity` is currently always `0`.
* `SpaceJellyFishSlime (0x91)`:
  ```text
  ushort ticks
  Vector position
  Vector movement
  float  angle
  float  angularVelocity
  ```
  `angularVelocity` is currently always `0`.
* `ClassicShipPlayerUnit (0xF0)`:
  ```text
  Vector position
  Vector movement
  float  angle
  float  angularVelocity
  ```
* `ModernShipPlayerUnit (0xF1)`:
  ```text
  Vector position
  Vector movement
  float  angle
  float  angularVelocity
  ```
* `InterceptorExplosion (0xFE)`: no additional payload; the reference connector ignores `0x31` for explosions
* `Explosion (0xFF)`: no additional payload; the reference connector ignores `0x31` for explosions

### `0x32` Visible Unit Runtime State Update

All `0x32` packets currently start with:

```text
byte   clusterId
string unitName
```

The remaining payload depends on the unit type.

Player-unit subsystem block rules:

* every visible player-unit subsystem block starts with `byte existsFlag`
* if `existsFlag == 0`, no more bytes for that subsystem follow in `0x32`
* visible player-unit packets do not send subsystem tiers
* visible fabricator blocks no longer send `MinimumRate`; treat it as implicit `0`

Current `0x32` payloads:

* all steady units (`Sun`, `BlackHole`, `CurrentField`, `Nebula`, `Planet`, `Moon`, `Meteoroid`, `Buoy`, `WormHole`, `MissionTarget`, `Flag`, `DominationPoint`, all power-up kinds, `Switch`, `Gate`) start with:
  ```text
  Vector configuredPosition
  byte   orbitCount
  orbitCount * (
    float distance
    float startAngle
    int   rotationTicks
  )
  ```
  `configuredPosition` is the canonical map-editor center returned by `QueryUnitXml(...)`.
  For non-orbiting steady units, `orbitCount == 0` and `configuredPosition == runtime position`.
  Clients can combine this payload with `0xC0` galaxy tick updates to recompute the runtime position deterministically.
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
* `CurrentField (0x02)`:
  ```text
  byte  mode
  Vector flow
  float radialForce
  float tangentialForce
  ```
  `mode` currently uses `0x00 Directional`, `0x01 Relative`.
* `Nebula (0x03)`:
  ```text
  float hue
  ```
* `Storm (0x20)`:
  ```text
  float  spawnChancePerTick
  ushort minAnnouncementTicks
  ushort maxAnnouncementTicks
  ushort minActiveTicks
  ushort maxActiveTicks
  float  minWhirlRadius
  float  maxWhirlRadius
  float  minWhirlSpeed
  float  maxWhirlSpeed
  float  minWhirlGravity
  float  maxWhirlGravity
  float  damage
  ```
* `StormCommencingWhirl (0x21)`:
  ```text
  ushort remainingAnnouncementTicks
  ```
* `StormActiveWhirl (0x22)`:
  ```text
  ushort remainingActiveTicks
  float  damage
  ```
* `Planet (0x08)` / `Moon (0x09)` / `Meteoroid (0x0A)`:
  ```text
  float metal
  float carbon
  float hydrogen
  float silicon
  ```
  These values are also the current non-depleting mining yields for those bodies.
* `Buoy (0x10)`:
  ```text
  string? message
  ```
* `WormHole (0x11)`:
  ```text
  byte  targetClusterId
  float targetLeft
  float targetTop
  float targetRight
  float targetBottom
  ```
  This payload is intentionally only present in full visibility. Reduced worm-hole sightings therefore do not disclose the destination yet.
* `MissionTarget (0x14)`:
  ```text
  ushort sequenceNumber
  ushort vectorCount
  vectorCount * Vector
  ```
  `Achievement` is configuration-only metadata and is not mirrored on the wire.
* `Flag (0x15)`:
  ```text
  ushort graceTicks
  byte activeFlag
  ```
* `DominationPoint (0x16)`:
  ```text
  byte teamId
  int  domination
  int  scoreCountdown
  ```
* `EnergyChargePowerUp (0x70)` / `IonChargePowerUp (0x71)` / `NeutrinoChargePowerUp (0x72)` / `MetalCargoPowerUp (0x73)` / `CarbonCargoPowerUp (0x74)` / `HydrogenCargoPowerUp (0x75)` / `SiliconCargoPowerUp (0x76)` / `ShieldChargePowerUp (0x77)` / `HullRepairPowerUp (0x78)` / `ShotChargePowerUp (0x79)`:
  ```text
  float amount
  ```
* `Switch (0x60)`:
  ```text
  ushort cooldownRemainingTicks
  byte switchedFlag
  ```
* `Gate (0x61)`:
  ```text
  byte closedFlag
  byte hasRestoreRemainingTicksFlag
  [ushort restoreRemainingTicks if hasRestoreRemainingTicksFlag != 0]
  ```
  `hasRestoreRemainingTicksFlag == 0` means that no auto-restore timer is currently armed.
* `SpaceJellyFish (0x90)` / `AiBase (0x92)` / `AiTurret (0x93)` / `AiShip (0x95)` / `AiProbe (0x96)`:
  ```text
  float hullCurrent
  float hullMaximum
  ```
* `SpaceJellyFishSlime (0x91)`:
  ```text
  float load
  float damage
  byte  targetClusterIdOrFF
  string targetUnitName
  byte  targetUnitKindOrFF
  ```
  `targetClusterIdOrFF == 0xFF` and `targetUnitKindOrFF == 0xFF` mean that the slime currently has no target. If the target vanished, the last known target reference may stay on the wire while the slime continues straight.
* `AiFreighter (0x94)`:
  ```text
  float hullCurrent
  float hullMaximum
  float lootMetal
  float lootCarbon
  float lootHydrogen
  float lootSilicon
  ```
* `Shot (0xE0)`:
  ```text
  float load
  float damage
  ```
* `Interceptor (0xE1)`:
  ```text
  float load
  float damage
  ```
* `Rail (0xE2)`:
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
  byte  shieldExists
  float shieldMaximum
  float shieldCurrent
  byte  shieldActive
  float shieldRate
  byte  shieldStatus
  float shieldConsumedEnergyThisTick
  float shieldConsumedIonsThisTick
  float shieldConsumedNeutrinosThisTick
  byte  armorExists
  float armorReduction
  byte  armorStatus
  float armorBlockedDirectDamageThisTick
  float armorBlockedRadiationDamageThisTick
  byte  repairExists
  float repairMinimumRate
  float repairMaximumRate
  float repairRate
  byte  repairStatus
  float repairConsumedEnergyThisTick
  float repairConsumedIonsThisTick
  float repairConsumedNeutrinosThisTick
  float repairRepairedHullThisTick
  byte  cargoExists
  float cargoMaximumMetal
  float cargoMaximumCarbon
  float cargoMaximumHydrogen
  float cargoMaximumSilicon
  float cargoCurrentMetal
  float cargoCurrentCarbon
  float cargoCurrentHydrogen
  float cargoCurrentSilicon
  byte  cargoStatus
  byte  resourceMinerExists
  float resourceMinerMinimumRate
  float resourceMinerMaximumRate
  float resourceMinerRate
  byte  resourceMinerStatus
  float resourceMinerConsumedEnergyThisTick
  float resourceMinerConsumedIonsThisTick
  float resourceMinerConsumedNeutrinosThisTick
  float resourceMinerMinedMetalThisTick
  float resourceMinerMinedCarbonThisTick
  float resourceMinerMinedHydrogenThisTick
  float resourceMinerMinedSiliconThisTick
  float effectiveStructureLoad
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
  byte  shotLauncherExists
  float shotLauncherMinimumRelativeMovement
  float shotLauncherMaximumRelativeMovement
  ushort shotLauncherMinimumTicks
  ushort shotLauncherMaximumTicks
  float shotLauncherMinimumLoad
  float shotLauncherMaximumLoad
  float shotLauncherMinimumDamage
  float shotLauncherMaximumDamage
  Vector shotLauncherRelativeMovement
  ushort shotLauncherTicks
  float shotLauncherLoad
  float shotLauncherDamage
  byte  shotLauncherStatus
  float shotLauncherConsumedEnergyThisTick
  float shotLauncherConsumedIonsThisTick
  float shotLauncherConsumedNeutrinosThisTick
  byte  shotMagazineExists
  float shotMagazineMaximumShots
  float shotMagazineCurrentShots
  byte  shotMagazineStatus
  byte  shotFabricatorExists
  float shotFabricatorMaximumRate
  byte  shotFabricatorActive
  float shotFabricatorRate
  byte  shotFabricatorStatus
  float shotFabricatorConsumedEnergyThisTick
  float shotFabricatorConsumedIonsThisTick
  float shotFabricatorConsumedNeutrinosThisTick
  byte  interceptorLauncherExists
  float interceptorLauncherMinimumRelativeMovement
  float interceptorLauncherMaximumRelativeMovement
  ushort interceptorLauncherMinimumTicks
  ushort interceptorLauncherMaximumTicks
  float interceptorLauncherMinimumLoad
  float interceptorLauncherMaximumLoad
  float interceptorLauncherMinimumDamage
  float interceptorLauncherMaximumDamage
  Vector interceptorLauncherRelativeMovement
  ushort interceptorLauncherTicks
  float interceptorLauncherLoad
  float interceptorLauncherDamage
  byte  interceptorLauncherStatus
  float interceptorLauncherConsumedEnergyThisTick
  float interceptorLauncherConsumedIonsThisTick
  float interceptorLauncherConsumedNeutrinosThisTick
  byte  interceptorMagazineExists
  float interceptorMagazineMaximumShots
  float interceptorMagazineCurrentShots
  byte  interceptorMagazineStatus
  byte  interceptorFabricatorExists
  float interceptorFabricatorMaximumRate
  byte  interceptorFabricatorActive
  float interceptorFabricatorRate
  byte  interceptorFabricatorStatus
  float interceptorFabricatorConsumedEnergyThisTick
  float interceptorFabricatorConsumedIonsThisTick
  float interceptorFabricatorConsumedNeutrinosThisTick
  byte  railgunExists
  float railgunEnergyCost
  float railgunMetalCost
  byte  railgunDirection
  byte  railgunStatus
  float railgunConsumedEnergyThisTick
  float railgunConsumedIonsThisTick
  float railgunConsumedNeutrinosThisTick
  byte  jumpDriveExists
  float jumpDriveEnergyCost
  ```
* `InterceptorExplosion (0xFE)`: no additional payload
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
The server sends a new challenge after roughly `1s` once the previous challenge was answered, and disconnects the session if a reply stays outstanding for more than `5s`.
The reference connector answers these ping requests automatically.

### Admin / Map Editing

* `0x04`: configure galaxy, payload `string xml`
* `0x24`: set or overwrite one region, payload `byte clusterId`, `string xml`
* `0x25`: remove one region, payload `byte clusterId`, `byte regionId`
* `0x26`: query all regions of one cluster, payload `byte clusterId`
* `0x27`: query editable units of one cluster, payload `byte clusterId`, `int offset`, `ushort maximumCount`
* `0x28`: set or overwrite one editable unit, payload `byte clusterId`, `string xml`
* `0x29`: remove one editable unit, payload `byte clusterId`, `string unitName`
* `0x2A`: query XML of one editable unit, payload `byte clusterId`, `string unitName`
* `0x60`: configure tournament, payload `string xml`
* `0x61`: commence tournament, empty payload
* `0x62`: start tournament, empty payload
* `0x63`: cancel tournament, empty payload
* `0x64`: query accounts for tournament tooling, payload `int offset`, `ushort maximumCount`
* `0x65`: query one galaxy ACL list, payload `byte kind`, `int offset`, `ushort maximumCount`
* `0x66`: add one account to one galaxy ACL list, payload `byte kind`, `int accountId`
* `0x67`: remove one account from one galaxy ACL list, payload `byte kind`, `int accountId`

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

Reply format of `0x27`:

```text
int    totalCount
int    offset
ushort returnedCount

repeat returnedCount times:
    byte   unitKind
    string unitName
```

Reply format of `0x64`:

```text
int    totalCount
int    offset
ushort returnedCount

repeat returnedCount times:
    int    accountId
    string accountName
    byte   adminFlag
    int    rank
    long   playerKills
    long   playerDeaths
    byte   hasAvatarFlag
    byte   hasTournamentEloFlag
    if hasTournamentEloFlag != 0:
        float tournamentElo
```

Reply format of `0x65`:

```text
int    totalCount
int    offset
ushort returnedCount

repeat returnedCount times:
    int    accountId
    string accountName
    byte   adminFlag
    int    rank
    long   playerKills
    long   playerDeaths
    byte   hasAvatarFlag
    byte   hasTournamentEloFlag
    if hasTournamentEloFlag != 0:
        float tournamentElo
```

Important notes:

* `0x26` is binary on the wire. The reference connector converts the binary reply into `<Regions>...</Regions>` XML only as a convenience API.
* `0x27` is admin-only and returns all `CanBeEdited = true` named units of the cluster, including currently invisible ones such as inactive power-ups.
* `0x29` and `0x2A` resolve the unit by `(clusterId, unitName)` on the server side.
* `0x28`, `0x29`, and `0x2A` only work for unit types with `CanBeEdited = true`.
* Empty or unreadable XML on protocol level is rejected as `0x12 InvalidArgumentGameException` with `InvalidArgumentKind.AmbiguousXmlData` and parameter name `xml`.
* `0x2A` rejects non-editable units with `0x16 InvalidXmlNodeValueGameException`.
* Editable target kinds currently include `CurrentField`, `Nebula`, `Storm`, `MissionTarget`, `Flag`, `DominationPoint`, `Switch`, `Gate`, `SpaceJellyFish`, `AiBase`, `AiTurret`, `AiFreighter`, `AiShip`, and `AiProbe`.
* `0x04` also rejects cluster removal while any remaining unit still references that cluster. The current example is a `WormHole` whose `TargetCluster` points to the cluster you are trying to remove.
* `0x60` accepts `<Tournament ...>` XML with `Mode`, `DurationTicks`, exactly two playable `<Team Id="...">` elements containing `<Account Id="..."/>` children, and optional `<Match WinnerTeamId="..."/>` elements that describe already finished games in order.
* `0x60` is admin-only and rejects `GameMode == Mission` with `0x39 TournamentModeNotAllowedGameException`.
* `0x61`, `0x62`, and `0x63` are admin-only tournament lifecycle actions.
* `0x64` is admin-only, filters server-side to account statuses `user` and `reoptin`, orders by `lower(name), id`, and returns connector-style account snapshots for tournament configuration UIs.
* `0x65`, `0x66`, and `0x67` are admin-only galaxy ACL actions.
* `0x65` accepts `kind = 0x01` for the player ACL and `kind = 0x04` for the admin ACL. Other values are rejected as invalid arguments.
* `0x65` returns connector-style account snapshots for the requested ACL list, ordered by `lower(name), id`.
* `0x66` rejects nonexistent accounts with `0x13 SpecifiedElementNotFoundGameException`.
* Empty ACL lists keep the corresponding login kind open to all accounts; non-empty lists turn that login kind into a whitelist.
* XML semantics, whitelists, validation rules, and examples are intentionally documented in [MAPEDITORS.md](MAPEDITORS.md), not duplicated here.

### Player Commands

* `0x80`: register classic ship, payload `string name`, `string crystal0Name`, `string crystal1Name`, `string crystal2Name`, reply payload `byte controllableId`
* `0x81`: register modern ship, payload `string name`, `string crystal0Name`, `string crystal1Name`, `string crystal2Name`, reply payload `byte controllableId`
* `0x84`: continue controllable, payload `byte controllableId`
* `0x85`: suicide controllable, payload `byte controllableId`
* `0x87`: move controllable, payload `byte controllableId`, `Vector movement`
* `0x88`: shoot, payload `byte controllableId`, `Vector movement`, `ushort ticks`, `float load`, `float damage`
* `0x89`: configure scanner, payload `byte controllableId`, `byte scannerId`, `float width`, `float length`, `float angle`
* `0x8A`: activate scanner, payload `byte controllableId`, `byte scannerId`
* `0x8B`: deactivate scanner, payload `byte controllableId`, `byte scannerId`
* `0x8C`: configure shot fabricator, payload `byte controllableId`, `float rate`
* `0x8D`: activate shot fabricator, payload `byte controllableId`
* `0x8E`: deactivate shot fabricator, payload `byte controllableId`
* `0x8F`: request close of controllable, payload `byte controllableId`
* `0x90`: configure shield loading, payload `byte controllableId`, `float rate`
* `0x91`: activate shield loading, payload `byte controllableId`
* `0x92`: deactivate shield loading, payload `byte controllableId`
* `0x93`: configure repair rate, payload `byte controllableId`, `float rate`
* `0x94`: configure resource miner rate, payload `byte controllableId`, `float rate`
* `0x95`: trigger worm-hole jump, payload `byte controllableId`
* `0x96`: shoot interceptor, payload `byte controllableId`, `Vector movement`, `ushort ticks`, `float load`, `float damage`
* `0x97`: configure interceptor fabricator, payload `byte controllableId`, `float rate`
* `0x98`: activate interceptor fabricator, payload `byte controllableId`
* `0x99`: deactivate interceptor fabricator, payload `byte controllableId`
* `0x9A`: fire railgun forward, payload `byte controllableId`
* `0x9B`: fire railgun backward, payload `byte controllableId`
* `0x9C`: configure nebula collector rate, payload `byte controllableId`, `float rate`
* `0x9D`: produce crystal, payload `byte controllableId`, `string name`, reply payload `byte produced`, `byte crystalCount`, repeated crystal snapshot entries
* `0x9E`: rename crystal, payload `string oldName`, `string newName`, reply payload `byte crystalCount`, repeated crystal snapshot entries
* `0x9F`: destroy crystal, payload `string name`, reply payload `byte crystalCount`, repeated crystal snapshot entries
* `0xA0`: request crystals, empty payload, reply payload `byte crystalCount`, repeated crystal snapshot entries
* `0xA1`: set modern engine thrust, payload `byte controllableId`, `byte slot`, `float thrust`
* `0xA2`: configure modern scanner, payload `byte controllableId`, `byte slot`, `float width`, `float length`, `float angleOffset`
* `0xA3`: activate modern scanner, payload `byte controllableId`, `byte slot`
* `0xA4`: deactivate modern scanner, payload `byte controllableId`, `byte slot`
* `0xA5`: shoot modern shot launcher, payload `byte controllableId`, `byte slot`, `float relativeSpeed`, `ushort ticks`, `float load`, `float damage`
* `0xA6`: configure modern shot fabricator, payload `byte controllableId`, `byte slot`, `float rate`
* `0xA7`: activate modern shot fabricator, payload `byte controllableId`, `byte slot`
* `0xA8`: deactivate modern shot fabricator, payload `byte controllableId`, `byte slot`
* `0xA9`: shoot modern interceptor launcher, payload `byte controllableId`, `byte slot`, `float relativeSpeed`, `float angleOffset`, `ushort ticks`, `float load`, `float damage`
* `0xAA`: configure modern interceptor fabricator, payload `byte controllableId`, `byte slot`, `float rate`
* `0xAB`: activate modern interceptor fabricator, payload `byte controllableId`, `byte slot`
* `0xAC`: deactivate modern interceptor fabricator, payload `byte controllableId`, `byte slot`
* `0xAD`: fire modern railgun, payload `byte controllableId`, `byte slot`
* `0xC4`: send galaxy chat, payload `string message`
* `0xC5`: send team chat, payload `byte teamId`, `string message`
* `0xC6`: send private chat, payload `byte playerId`, `string message`
* `0xCC`: send private binary chat, payload `byte playerId`, `ushort messageLength`, `byte[] message`
* `0xCD`: send private binary chat bulk, payload `byte playerId`, `byte messageCount`, repeated `ushort messageLength`, `byte[] message`
* `0xF1`: download small avatar chunk, payload `byte playerId`, `int offset`, `ushort maximumLength`
* `0xF2`: download big avatar chunk, payload `byte playerId`, `int offset`, `ushort maximumLength`
* `0xF3`: download small account avatar chunk, payload `int accountId`, `int offset`, `ushort maximumLength`
* `0xF4`: download big account avatar chunk, payload `int accountId`, `int offset`, `ushort maximumLength`

Reply format of `0xF1` / `0xF2` / `0xF3` / `0xF4`:

```text
int    totalLength
int    offset
ushort chunkLength
byte[] chunkBytes
```

Notes:

* `0xC5` resolves the target team by id on the server side.
* `0xC6` resolves the target player by id on the server side.
* `0xCC` is both the client-side single binary-chat command and the server-originated private binary-chat event.
* `0xCD` is client-only. The server fans the bulk payload out into individual `0xCC` events for the receiver in the original order.
* Binary chat is player-to-player only.
* Each binary message must contain `1..1024` bytes.
* `0xCD` accepts `1..32` binary messages per request.
* Binary chat bypasses the normal text-chat flood control.
* Binary chat uses a session-local opt-in handshake:
  first `A -> B` single binary message is allowed,
  any further `A -> B` binary message before a binary reply from `B` fails with `0x3E`,
  and bulk `0xCD` is therefore only accepted after that binary acknowledgement from the target player.
* `0xF1` / `0xF2` resolve the target player by id on the server side and stream the avatar bytes cached on that server-side player at login time.
* `0xF3` / `0xF4` resolve the target account by id on the server side and stream the persisted account avatar bytes.
* `0xC1`, `0xC2`, and `0xC7` are server-originated objective system chat packets. They are not sent by clients.
* `0x10 Player Create` contains `hasAvatar`, so clients can avoid sending `0xF1` / `0xF2` for players without avatars.
* If `hasAvatar == 0`, the reference connector throws locally before sending `0xF1` / `0xF2`.
* The server still validates `0xF1` / `0xF2` and returns `0x18 AvatarNotAvailableGameException` if no avatar is available.
* `0xF3` / `0xF4` also return `0x18 AvatarNotAvailableGameException` if the target account has no stored avatar.
* Clients repeat the avatar request with increasing `offset` until `offset + chunkLength == totalLength`.
* `0x8F` is not an immediate removal. The server may keep a living controllable alive for 30 ticks before finally closing it. Dead controllables stay registered and can be `Continue()`d until the owner explicitly closes them.
* During that cleanup window the owning player may already have `stateFlags & 0x01 != 0`, meaning the connection is gone even though the player and controllable infos are still present.
* String validation for names and chat messages happens on the server even if a client already validated locally.
* `0x88` currently validates `movement.Length` in `[0.1; 3]`, `ticks` in `[2; 140]`, `load` in `[2.5; 25]`, and `damage` in `[1; 20]`. Firing additionally requires at least one full stored shot in `ShotMagazine`; otherwise the launcher runtime stays failed for that tick.
* `0x96` currently uses the same validation as `0x88`, but consumes from `InterceptorMagazine` and produces `Interceptor`.
* The current reference ClassicShip supports `scannerId = 0` for `MainScanner`. `scannerId = 1` addresses `SecondaryScanner`, which currently does not exist and therefore returns `0x10 SpecifiedElementNotFoundGameException`.
* `0x89` currently validates scanner `width` in `[5; 90]`, `length` in `[20; 300]`, and any finite `angle` as an absolute world angle.
* While a dynamic scanner is off, its current width, length, and angle are zero. On the next active tick after re-activation, width and length ramp from `2.5 x 10` and the angle starts again from `0` toward the configured target.
* `0x8C` currently validates fabricator `rate` in `[0; 0.025]`. `0x8D` / `0x8E` only toggle `Active`; they do not change the configured rate.
* `0x97` currently validates fabricator `rate` in `[0; 0.025]`. `0x98` / `0x99` only toggle `Active`; they do not change the configured rate.
* `0xA1` currently validates thrust against the addressed modern engine's installed `MaximumReverseThrust` / `MaximumForwardThrust`. The current reference values are `0.06` / `0.06` with `MaximumThrustChangePerTick = 0.02`.
* `0xA2` currently validates modern scanner `width` in `[5; 10]`, `length` in `[20; 350]`, and `angleOffset` in `[-22.5; +22.5]` relative to the scanner's local ship slot angle.
* `0xA5` currently uses the classic shot limits, but the produced modern shot leaves the launcher with `+0.5` projectile speed compared to the classic launcher.
* `0xA6` / `0xAA` currently validate modern fabricator `rate` in `[0; 0.025]`. `0xA7` / `0xA8` and `0xAB` / `0xAC` only toggle `Active`.
* `0xA9` currently uses the classic interceptor limits plus `angleOffset` in `[-45; +45]` relative to the addressed local `E` / `W` launcher direction.
* `0x90` currently validates shield `rate` in `[0; 0.125]`. `0x91` / `0x92` only toggle shield loading `Active`; they do not change the configured rate.
* `0x93` currently validates repair `rate` in `[0; 0.1]`. `rate == 0` means off. Repair only affects hull, costs `1600 * rate^2`, and the server clears the rate if the ship movement reaches `>= 0.1`.
* `0x94` currently validates miner `rate` in `[0; 0.01]`. `rate == 0` means off. The miner costs `160000 * rate^2`, mines non-depleting `Planet` / `Moon` / `Meteoroid` body resources within edge-to-edge distance `25`, clears the rate with `Failed` once the ship movement reaches `>= 0.1`, and also clears the rate after a paid zero-yield tick when no mineable resources are in range.
* `0x9C` currently validates collector `rate` in `[0; 0.1]`. `rate == 0` means off. The collector costs `1600 * rate^2`, requires ship movement `< 0.1`, collects from the first in-range `Nebula` within edge-to-edge distance `25`, and clears the configured rate after a paid zero-yield tick when no nebula is in range.
* `0x9D` always clears nebula cargo if `cargoCurrentNebula < 4`, returns `produced = 0`, and still replies with the full crystal snapshot. For valid creation attempts with `cargoCurrentNebula >= 4`, the account is capped at `64` crystals.
* `0x9E` / `0x9F` reject locked crystals with `0x13 PermissionFailedGameException`. Foreign-account access still surfaces as `0x12 InvalidArgumentGameException` with `EntityNotFound`.
* Crystal snapshot entries are ordered as `string name`, `float hue`, `byte grade`, `12` multiplier floats in the effect-axis order, then `byte locked`.
* `0x95` currently succeeds only while the controllable is alive, touches exactly one `WormHole`, has at least `1000` energy available, and the worm-hole target region in the target cluster has a free spawn position. Successful jumps always zero the ship movement.
* `0x9A` / `0x9B` fire a fixed rail shot. They currently require `300` energy and `1` metal, derive front/back from the controllable's movement angle, and fall back to world angle `0` at standstill.
* `InterceptorExplosion` only affects projectile kinds. Normal `Explosion` currently does not destroy `Rail`; `Rail` is only removed by solid collisions, lifetime expiry, or `InterceptorExplosion`.
* On revive, the reference `ClassicShip` restores both `ShotMagazine` and `InterceptorMagazine` to `2`, resets `ShotFabricator` and `InterceptorFabricator` to `Active=false`, `Rate=0`, and resets `Shield` to `Current=0`, `Active=false`, `Rate=0`.

## Practical Compatibility Notes

If you write your own connector, these details matter in practice:

* Do not trust packet ordering beyond what is stated above.
* Always parse all packets contained in one WebSocket frame.
* Treat connection shutdown as failure of all open requests.
* Handle the synthetic spectators team locally or some player packets will reference an unknown team id.
* Do not assume that XML helper APIs from the reference connector correspond 1:1 to wire format. Region queries are the current example: wire is binary, helper API returns XML.
