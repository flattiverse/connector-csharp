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

Current protocol version:

```text
0
```

Examples:

```text
wss://www.flattiverse.com/galaxies/0/api?version=0&auth=<64-hex-api-key>&team=Blue
wss://www.flattiverse.com/galaxies/0/api?version=0&auth=<64-hex-api-key>
wss://www.flattiverse.com/galaxies/0/api?version=0&auth=0000000000000000000000000000000000000000000000000000000000000000
```

Important details:

* `auth` is always a 64-character lowercase/uppercase hex string representing 32 bytes.
* The all-zero API key is the special spectator login.
* The team-less form is currently used for spectator and admin logins.
* Normal players must currently provide a valid `team` query parameter.
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
1. `0x02` team snapshots
1. `0x06` cluster snapshots
1. existing `0x10` players
1. existing `0x20` controllable infos
1. `0x10` create packet for the newly connected player itself
1. visible `0x30` unit create packets
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

Each server-side `GameException` has a numeric id. On the wire, exceptions are transported in `command = 0xFF` packets.

Current codes:

* `0x01` `CantConnectGameException`
* `0x02` `InvalidProtocolVersionGameException`
* `0x03` `AuthFailedGameException`
* `0x04` `WrongAccountStateGameException`
* `0x05` `InvalidOrMissingTeamGameException`
* `0x08` `ServerFullOfPlayerKindGameException`
* `0x0C` `SessionsExhaustedGameException`
* `0x0D` `InvalidDataGameException`
* `0x0F` `ConnectionTerminatedGameException`
* `0x10` `SpecifiedElementNotFoundGameException`
* `0x11` `CantCallThisConcurrentGameException`
* `0x12` `InvalidArgumentGameException`
* `0x13` `PermissionFailedGameException`
* `0x14` `FloodcontrolTriggeredGameException`
* `0x15` `UnitConstraintViolationGameException`
* `0x16` `InvalidXmlNodeValueGameException`
* `0x20` `YouNeedToContinueFirstGameException`
* `0x21` `YouNeedToDieFirstGameException`
* `0x22` `AllStartLocationsAreOvercrowded`
* `0x30` `CanOnlyShootOncePerTickGameException`

Payload details for the important structured exceptions:

* `0x04`: `byte accountStatus`
* `0x08`: `byte playerKind`
* `0x12`: `byte invalidArgumentKind`, `string parameter`
* `0x16`: `byte invalidArgumentKind`, `string nodePath`, `string hint`

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
* `0x06`: create or update cluster
* `0x07`: deactivate cluster
* `0x10`: create player
* `0x11`: update player
* `0x1F`: deactivate player
* `0x20`: create controllable info
* `0x21`: mark controllable info alive
* `0x22`: mark controllable info dead with generic reason
* `0x23`: mark controllable info dead by collision
* `0x24`: mark controllable info dead by other player
* `0x2F`: deactivate controllable info
* `0x30`: create visible unit
* `0x31`: update visible unit state
* `0x3F`: deactivate visible unit
* `0x80`: create controllable
* `0x81`: controllable deceased
* `0x82`: controllable updated and alive
* `0x8F`: controllable deactivated
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
```

`maintenanceFlag` is `0x00` or `0x01`.

### `0x02` Team Snapshot

Payload order:

```text
byte   teamId
byte   red
byte   green
byte   blue
string name
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

### `0x10` Player Create

Payload order:

```text
byte   playerId
byte   playerKind
byte   teamId
string accountName
float  pingMilliseconds
```

### `0x11` Player Update

Payload order:

```text
byte  playerId
float pingMilliseconds
```

For unit and controllable payload details, inspect the current reference implementation types in `Flattiverse.Connector/Units` and `Flattiverse.Connector/GalaxyHierarchy`.

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
* XML semantics, whitelists, validation rules, and examples are intentionally documented in [MAPEDITORS.md](MAPEDITORS.md), not duplicated here.

### Player Commands

* `0x80`: register classic ship, payload `string name`, reply payload `byte controllableId`
* `0x84`: continue controllable, payload `byte controllableId`
* `0x85`: suicide controllable, payload `byte controllableId`
* `0x87`: move controllable, payload `byte controllableId`, `Vector movement`
* `0x88`: shoot, payload `byte controllableId`, `Vector movement`, `ushort ticks`, `float load`, `float damage`
* `0x8F`: unregister controllable, payload `byte controllableId`
* `0xC4`: send galaxy chat, payload `string message`
* `0xC5`: send team chat, payload `byte teamId`, `string message`
* `0xC6`: send private chat, payload `byte playerId`, `string message`

Notes:

* `0xC5` resolves the target team by id on the server side.
* `0xC6` resolves the target player by id on the server side.
* String validation for names and chat messages happens on the server even if a client already validated locally.

## Practical Compatibility Notes

If you write your own connector, these details matter in practice:

* Do not trust packet ordering beyond what is stated above.
* Always parse all packets contained in one WebSocket frame.
* Treat connection shutdown as failure of all open requests.
* Handle the synthetic spectators team locally or some player packets will reference an unknown team id.
* Do not assume that XML helper APIs from the reference connector correspond 1:1 to wire format. Region queries are the current example: wire is binary, helper API returns XML.
