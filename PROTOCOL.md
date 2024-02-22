# Flattiverse Binary WebSocket Protocol.

Yes, we did it again: A complete rewrite of flattiverse with new design
paradigms and we are sorry about that.

## Things you should know before you start.

This document is part of the reference connector project. Therefore we
reference many things directly from the source code, like lists which are
written there. One example is the list of errors which you can find
[here](Flattiverse.Connector/Flattiverse.Connector/GameException.cs).

## Basic Game Topology.

If you would draw a system diagram of flattiverse you could go like this:

1. Universe: This is the whole flattiverse server hosting various galaxies.
2. Galaxy: This is a map where you can register to play on.
3. Cluster: This is a sub map of the Galaxy which is usually used to create 
   logical groups of game units within galaxies to optimize unit movement 
   and collision processing.
4. Region: This is a defined area in a cluster which then can be configured
   for special game rules like a starting location or a region with a special
   name which will be shown to the player via `GameMessage`.
5. Units: All units in galaxies must have unique names. Further naming rules
   can be derived form this: If units of one kind can change the cluster,
   then their names must be unique over the galaxy. If not their name must
   be unique within the cluster and within the set of units which can change
   the cluster.

## Basic network mechanics.

The game is based on a binary web socket protocol that sends packets around.
A packet has the following structure:

* Header
  * `byte` `Command`: A command identifier which selects the command to call.
  * `byte` `Session`: The session to which this packet belongs. 
  * `byte` `Id0`: An id which is used to address various things.
  * `byte` `Id1`: An id which is used to address various things.
  * `byte` `Param0`: An general purpose parameter.
  * `byte` `Param1`: An general purpose parameter.
  * `ushort` `Size`: The size of the payload.
* Up to 1024 bytes of payload.

### Payload reading.

The payload is used to transfer various things, including floating point numbers.
However, floating point numbers will be transferred as integers, thus meaning that
all the floating point numbers aren't real floating point numbers but based on a
specific number of digits:

* A coordinate (`X` or `Y` of a `Vector`): `int` (4 bytes) with 5 decimal places
  representing values from `-21474.83648` to `21474.83647`.
* An angle (0° - 360°): `ushort` (2 bytes) with 2 decimal places from `0.00` to
  `359.99`.
* etc...

## Connecting to the game server.

You have to specify the following data when connecting to a galaxy:

1. The URI you connect to. (The URI determines the galaxy you choose.)
2. The auth key you use to identify yourself.
3. The team you want to use to join.
4. The protocol version you speak. (You will be disconnected, if this
   doesn't match what the galaxy server expects.)

The Server may reply with the following HTTP status codes and the error they
result in in the reference connector (propably you should use the same messages):

1. <couldn't connect>: `[0xF1] Couldn't connect to the universe server: Are
   you online? Is flattiverse still online?`
2. `502` or `504`: `[0xF2] The reverse proxy of the flattiverse universe is
   online but the corresponding galaxy is offline. This may be due to maintenance
   reasons or the galaxy software version is being upgraded.`
3. `400`: `[0xF3] The call could not be processed. Either must didn't make a
   WebSocket call or the database is not available.`
4. `401`: `[0xF4] Authorization failed, possibly because one of these reasons:
   auth parameter missing, ambiguous or unknown, or no spectators allowed.`
5. `403`: `[0xF5] Forbidden. You are not allowed to perform this action (the
   way you tried).`
6. `409`: `[0xF6] The connector you are using is outdated.`
7. `412`: `[0xF7] Login failed because you're already online.`
8. `415`: `[0xF8] Specified team doesn't exist or can't be selected.`

If this documentation is not recent, you can find the assignments
[here](Flattiverse.Connector/Flattiverse.Connector/Network/Connection.cs).

## Packet Command List.

Flattiverse implements those commands sent from Server to Connector:

* `0x10`: A galaxy info you don't know yet. This can either be directly after login as an initial transmission or if one was created, changed or removed.
* `0x11`: A cluster info you don't know yet. This can either be directly after login as an initial transmission or if one was created, changed or removed.
* `0x12`: A region info you don't know yet. This can either be directly after login as an initial transmission or if one was created, changed or removed.
* `0x13`: A team info you don't know yet. This can either be directly after login as an initial transmission or if one was created, changed or removed.
* `0x14`: A ship info you don't know yet. This can either be directly after login as an initial transmission or if one was created, changed or removed.
* `0x15`: A upgrade info you don't know yet. This can either be directly after login as an initial transmission or if one was created, changed or removed.
* `0x16`: A player info you don't know yet. This can either be directly after login as an initial transmission or if one was created.
* `0x17`: A player was removed.
* `0x1C`: A new unit has been found.
  * `Param0`: `(byte)UnitKind` - specified the kind of the unit trasferred.
  * `Id0`: `Cluster.ID` - the ID of the cluster this unit is in.
  * `Payload`: The unit data to read.
* `0x1D`: A already found unit has been updated.
* `0x1E`: A unit which has been seen in the past is now not seen any more.
* `0x20`: Round committed.
* `0xFF` (in a session): An error occurred which processing the session.

And those commands from the client to the server:

* `0x40`: Configure galaxy
* `0x41`: Add new cluster
* `0x42`: Configure cluster
* `0x43`: Remove cluster
* `0x44`: Add new region
* `0x45`: Configure region
* `0x46`: Remove region
* `0x47`: Add new team
* `0x48`: Configure team
* `0x49`: Remove team
* `0x4A`: Add new ship design
* `0x4B`: Configure ship design
* `0x4C`: Remove ship design
* `0x4D`: Add new upgrade
* `0x4E`: Configure upgrade
* `0x4F`: Remove upgrade
* `0x50`: Get specific unit
* `0x51`: Add new unit
* `0x52`: Configure black hole
* `0x53`: Remove black hole
* `0x54`: Configure buoy
* `0x55`: Remove buoy
* `0x56`: Configure meteoroid
* `0x57`: Remove meteoroid
* `0x58`: Configure moon
* `0x59`: Remove moon
* `0x5A`: Configure planet
* `0x5B`: Remove planet
* `0x5C`: Configure sun
* `0x5D`: Remove sun


## Data Types


| Name | Size | Decimal Places | Comment/Todo | status |
| ---- | ---- | ---- | ---- | ---- |
| Angle | 2 byte unsigned | 2 |  |  |
| Radius | 3 byte unsigned | 3 |  |  |
| WeaponLoad |  |  | -> Radius |  |
| Size |  |  | -> Radius |  |
| Vector | 4 byte signed | 5 |  |  |
|  |  |  |  |  |
| costEnergy | 4 byte unsigned | 3 | Für Energie, Ions und Cargo |  |
| costIon |  |  |  |  |
| CostIron |  |  |  |  |
| CostTungsten |  |  |  |  |
| CostSilicon |  |  |  |  |
| CostTritium |  |  |  |  |
| EnergyMax |  |  |  |  |
| EnergyCells |  |  |  |  |
| EnergyReactor |  |  |  |  |
| EnergyTransfer |  |  |  |  |
| IonMax |  |  |  |  |
| IonCells |  |  |  |  |
| IonReactor |  |  |  |  |
| IonTransfer |  |  |  |  |
| weaponAmmo |  |  |  |  |
| weaponAmmoProduction |  |  |  |  |
| Cargo |  |  |  |  |
| Extractor |  |  |  |  |
|  |  |  |  |  |
| CostTime | 2 byte unsigned | /20 -> s |  |  |
| WeaponTime |  |  | -> Time, wie bei CostTime |  |
| Hull | 3 byte unsigned | 4 | Für Hüllenpunke und Schildpunkte. |  |
| WeaponDamage |  |  | -> Hull |  |
| HullRepair |  |  |  |  |
| Shields |  |  |  |  |
| ShieldsLoad |  |  |  |  |
| Weight | 2 byte signed | 4 | -> Gravitation |  |
| Thruster | 2 byte unsigned | 4 |  |  |
| Nozzle | 2 byte signed | 2 | -> Turnrate |  |
| Turnrate | 2 byte signed | 2 | -> Gleich wie Nozzle |  |
| Speed | 2 byte unsigned | 3 |  |  |
| WeaponSpeed |  |  | -> Speed |  |

