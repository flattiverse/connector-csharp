# Flattiverse Reference Connector

This is the Flattiverse reference connector for the binary protocol. You can use this to derive own connectors in other (or even C#) programming languages.

# The current status

This is the implementation of the protocol. It sends you the configured universe groups and teams. See [here](https://documentation.flattiverse.com/display/FLAT/Command+IDs) for a command overview.

The Server runs on `gaglxy.flattiverse.com` port `80`. You need to use one of the following accounts with the password `Password`:

* `Anonymous` (Requires Opt-In.)
* `Player0`
* `Player1`
* `Player2`
* `Player3`

*Please see the most updated information about the server [here](https://documentation.flattiverse.com/display/FLAT/Connector+development). If the development of this connector progresses we will also update the server on `galaxy.flattiverse.com:80`.*