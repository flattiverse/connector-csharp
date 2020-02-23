# Flattiverse Reference Connector

This is the Flattiverse reference connector for the binary protocol. You can use this to derive own connectors in other (or even the C#) programming language(s).

# The current status

This is the implementation of the protocol. It sends you the configured universe groups, teams, accounts, privileges and join/part events. See [here](https://documentation.flattiverse.com/display/FLAT/Command+IDs) for a internal command overview.

The Server runs on `galaxy.flattiverse.com` port `80`. You need to use one of the following accounts with the password `Password`:

* `Anonymous` (Requires Opt-In.)
* `Player0`
* `Player1`
* `Player2`
* `Player3`

Join command works for universe `Development` but not for the universe `For The Noobs`. (Just so you can test the server replys.) Currently `Universe`s, `Team`s, `Galaxy`(ie)s and `UniverseSystem`s are transferred to the connector, other things could be queries by using the `async foreach` or other endpoints.

*Please see the most updated information about the server [here](https://documentation.flattiverse.com/display/FLAT/Connector+development). If the development of this connector progresses we will also update the server on `galaxy.flattiverse.com:80`.*