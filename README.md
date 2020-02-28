# Flattiverse Reference Connector

This is the Flattiverse reference connector for the binary protocol. You can use this to derive own connectors in other (or even the C#) programming language(s).

Sadley we were required to roll back the connector to .NET Standard 2.0 for regular .NET compatibility. :( We will change this when .NET 5.0 has been released.

# The current status

This is the implementation of the protocol. It sends you the configured universe groups, teams, accounts, privileges and join/part events. See [here](https://documentation.flattiverse.com/display/FLAT/Command+IDs) for a internal command overview.

The Server runs on `galaxy.flattiverse.com` port `80`. You need to use one of the following accounts with the password `Password`:

* `Anonymous` (Requires Opt-In.)
* `Player0`
* `Player1`
* `Player2`
* `Player3`

Join command works for universe `Development` but not for the universe `For The Noobs`. (Just so you can test the server replys.) Currently `Universe`s, `Team`s, `Galaxy`(ie)s and `UniverseSystem`s are transferred to the connector, other things could be queries by using various endpoints.

It's also possible to Query, Update and Delete Units and Regions in the galaxy. Additionally: You now can Live View Galaxies and we implemented `Sun`s, `Planet`s, `Moon`s, `Meteoroid`s, `Buoy`s and ofc Mission`Target`s. :D - Checkout the code for how it's done. :)

*Please see the most updated information about the server [here](https://documentation.flattiverse.com/display/FLAT/Connector+development). If the development of this connector progresses we will also update the server on `galaxy.flattiverse.com:80`.*