# Flattiverse.Connector Reference Implementation
This project is here to help you implement your own connector. You may find the following information useful.
## GameExceptions
Each `Exception` possible in the game has a number. Here is a list:
* `0x01` (`CantConnectGameException`): Thrown, if the specified uri couldn't be reached.
* `0x02` (`InvalidProtocolVersionGameException`): Thrown, if you try to connect to an galaxy with an incompatible version.
* `0x03` (`AuthFailedGameException`): Thrown, if the authentication failed. (Missing or wrong API key.)
* `0x04` (`WrongAccountStateGameException`): Thrown, if the account isn't opted in, is banned or deleted.
* `0x05` (`InvalidOrMissingTeamGameException`): Thrown, if you didn't specify a team or if the specified team doesn't exist.
* `0x08` (`ServerFullOfPlayerKindGameException`): Thrown, if the specified PlayerKind doesn't have available slots on the server.
* `0x0C` (`SessionsExhaustedGameException`): Thrown, if you open more then 255 simultaneous requests.
* `0x0D` (`InvalidDataGameException`): Thrown, if the server did send some ambiguous data.
* `0x0F` (`ConnectionTerminatedGameException`): Thrown, if you try to do something after the connection has been closed.
* `0x10` (`SpecifiedElementNotFoundGameException`): Thrown if an element with specified index or name doesn't exist.
* `0x11` (`CantCallThisConcurrentGameException`): Thrown, if you try to access a method in parallel.
* `0x12` (`InvalidArgumentGameException`): Thrown if you did specify a wrong parameter.
* `0x13` (`PermissionFailedGameException`): Thrown if you tried to call a command to which your PlayerKind doesn't have access.
* `0x14` (`FloodcontrolTriggeredGameException`): Thrown, if you flood the chat somehow.
* `0x15` (`UnitConstraintViolationGameException`): Thrown, if you try to register too many ships.
* `0x16` (`InvalidXmlNodeValueGameException`): Thrown, if a specific XML node/attribute value is invalid. Includes reason, node path, and hint.
* `0x20` (`YouNeedToContinueFirstGameException`): Thrown, if the Controllable you want to control is dead.
* `0x21` (`YouNeedToDieFirstGameException`): Thrown, if you are not dead but try to continue.
* `0x22` (`AllStartLocationsAreOvercrowded`): Thrown, if theres too many traffic on the start locations.
* `0x30` (`CanOnlyShootOncePerTickGameException`): Thrown, if you tried to shoot multiple times a tick with the same unit.
## Packets
Those packet commands are used to transfer necessary data from the galaxy to the client:
* `0x00`: Ping request.
* `0x01`: Update `Galaxy` info.
* `0x02`: Create or update `Team`.
* `0x03`: Deactivate `Team`.
* `0x06`: Create or update `Cluster`.
* `0x07`: Deactivate `Cluster`.
* `0x10`: Create `Player`.
* `0x11`: Update `Player`.
* `0x1F`: Deactivate `Player`.
* `0x20`: Create `ControllableInfo`.
* `0x21`: Set `ControllableInfo` Alive. (Continued.)
* `0x22`: Set `ControllableInfo` dead with reason.
* `0x23`: Set `ControllableInfo` dead by Collision.
* `0x24`: Set `ControllableInfo` dead by OtherPlayer.
* `0x2F`: Deactivate `ControllableInfo`.
* `0x30`: You see a new `Unit` in the `Cluster`.
* `0x31`: `Position`, `Movement` and maybe `Direction` of the `Unit` has been updated.
* `0x3F`: You don't see the `Unit` anymore.
* `0x80`: There is a new `Controllable`.
* `0x81`: `Controllable` deceased.
* `0x82`: `Controllable` data has been updated and is alive.
* `0x8F`: `Controllable` has been deactivated.
* `0xC0`: `GalaxyTickEvent`.
* `0xC4`: Message received which has been sent to the `Galaxy`.
* `0xC5`: Message received which has been sent to your `Team`.
* `0xC6`: Message received which has been sent to you.

With those packet commands clients call commands in the Galaxy (server):

* `0x00` (all): Respond to a ping request with the given payload.
* `0x04` (admins only): Configure galaxy metadata/teams/clusters via XML.
* `0x24` (admins only): Set or update a region in a cluster via `<Region ...>` XML.
* `0x25` (admins only): Remove a region by id (`0..255`) from a cluster.
* `0x26` (admins only): Query all regions of a cluster (binary reply).
  Reply format: `ushort regionCount`, then per region: `byte id`, `string name`, `float left`, `float top`, `float right`, `float bottom`, `uint teamMask`.
* `0x28` (admins only): Set or update one editable map unit in a cluster via unit-root XML like `<Sun ... />`.
* `0x29` (admins only): Remove one editable map unit by name.
* `0x2A` (admins only): Query XML of one editable map unit by name.
* `0x80` (players only): Register a new classic ship with the specified name.
* `0x84` (players only): Continue (respawn) the specified controllable.
* `0x85` (players only): Commit suicide with the specified controllable.
* `0x87` (players only): Move the specified controllable with the given vector.
* `0x88` (players only): Shoot with the specified controllable using movement vector, ticks, load, and damage.
* `0x8F` (players only): Unregister (deactivate) the specified controllable.
* `0xC4` (players only): Send a message to the galaxy.
* `0xC5` (players only): Send a message to all members of a team.
* `0xC6` (players only): Send a message to a player.
