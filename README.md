# Flattiverse.Connector
This is the flattiverse C# reference implementation of the connector. This library
can be used to connect to a flattiverse galaxy.
# Derive implementations from this one.
Here are some useful lists.
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
* `0x13`: Deactivate `Player`.
* `0xC0`: `GalaxyTickEvent`.
* `0xC4`: Message received which has been sent to the `Galaxy`.
* `0xC5`: Message received which has been sent to your `Team`.
* `0xC6`: Message received which has been sent ti you.

With those packet commands clients call commands in the galaxy:
* `0xC4` (players only): Send a message to the galaxy.
* `0xC5` (players only): Send a message to all members of a team.
* `0xC6` (players only): Send a message to a player.