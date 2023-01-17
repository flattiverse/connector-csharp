# Flattiverse WebSocket JSON Protocol: Universe Simulator

This is the documentation of the Flattiverse JSON protocol for the Universe Simulator. The Flattiverse Universe Simulator is a service which takes care of one universe group including their universes, units, players, etc.. You can't access an universe group when connected to another universe group.

You can connect to the Universe Simulator (to a specific universe group) by opening a WebSocket connection to it. You can send single text frames which must not exceed 16 KiB of data. Note that you must split up each command into one frame. The server, however can send you frames up to 256 KiB. Also the server will group commands together when sending bulks of events like when a server tick has been processed.

# Sending commands

```json
{
  "command": "your command",
  "id": "asdfgh"
}
```

You can identify the reply to your command by waiting for an reply to the `id` you did specify.

yadda, yadda...