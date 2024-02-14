# Flattiverse Binary WebSocket Protocol.

Yes, we did it again: A complete rewrite of flattiverse with new design paradigms and we are sorry about that.

## Things you should know before you start.

This document is part of the reference connector project. Therefore we reference many things directly from the source code, like lists which are written there. One example is the list of errors which you can find [here](Flattiverse.Connector/Flattiverse.Connector/GameException.cs).

## Basic Game Topology.

If you would draw a system diagram of flattiverse you could go like this:

1. Universe: This is the whole flattiverse server hosting various galaxies.
1. Galaxy: This is a map where you can register to play on.
1. Cluster: This is a sub map of the Galaxy which is usually used to create logical groups of game units within galaxies to optimize unit movement and collision processing.
1. Region: This is a defined area in a cluster which then can be configured for special game rules like a starting location or a region with a special name which will be shown to the player via `GameMessage`.
1. Units: All units in galaxies must have unique names. Further naming rules can be derived form this: If units of one kind can change the cluster, then their names must be unique over the galaxy. If not their name must be unique within the cluster and within the set of units which can change the cluster.

## Basic network yadda, yadda...

## Connecting to the game server.

You have to specify the following data when connecting to a galaxy:

1. The URI you connect to. (The URI determines the galaxy you choose.)
1. The auth key you use to identify yourself.
1. The team you want to use to join.
1. The protocol version you speak. (You will be disconnected, if this doesn't match what the galaxy server expects.)

The Server may reply with the following HTTP status codes and the error they result in in the reference connector (propably you should use the same messages):

1. <couldn't connect>: `[0xF1] Couldn't connect to the universe server: Are you online? Is flattiverse still online?`
1. `502` or `504`: `[0xF2] The reverse proxy of the flattiverse universe is online but the corresponding galaxy is offline. This may be due to maintenance reasons or the galaxy software version is being upgraded.`
1. `400`: `[0xF3] The call could not be processed. Either must didn't make a WebSocket call or the database is not available.`
1. `401`: `[0xF4] Authorization failed, possibly because one of these reasons: auth parameter missing, ambiguous or unknown, or no spectators allowed.`
1. `409`: `[0xF5] The connector you are using is outdated.`
1. `412`: `[0xF6] Login failed because you're already online.`
1. `415`: `[0xF7] Specified team doesn't exist or can't be selected.`

If this documentation is not recent, you can find the assignments [here](Flattiverse.Connector/Flattiverse.Connector/Network/Connection.cs).