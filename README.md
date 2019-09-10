# Flattiverse Reference Connector

This is the Flattiverse reference connector for the binary protocol. You can use this to derive own connectors in other (or even C#) programming languages.

# The current status

This is the implementation of the protocol. It sends 10 packets to the server which echoes those packets.

Additionally the server alters those packets: Session will be the current second.

The Server runs on `gaglxy.flattiverse.com` port `80`. You need to use username `Anonymous` and the password `Password`.

*Please see the most updated information about the server [here](https://documentation.flattiverse.com/display/FLAT/Connector+development). If the development of this connector progresses we will also update the server on `galaxy.flattiverse.com:80`.*