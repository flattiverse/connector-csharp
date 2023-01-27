# Flattiverse WebSocket JSON Protocol

Flattiverse is a simulated 2D universe intended for students and others interested in getting a practical introduction to object-oriented programming. This documentation describes the protocol used to interact with Flattiverse.

Communication is carried out by sending single text frames which must not exceed 16 KiB of data and must not be splitted, each frame containing one command at a time. The server responds with frames up to 256 KiB, and will also group commands together when sending bulks of events, for example when a server tick has been processed.

## Connection and socket behaviour

You can interact with a Flattiverse `UniverseGroup` by connecting to the specific URI with a websocket.

All commands are expected to be in JSON format. To execute a command on the server, simply send a request with the corresponding command as payload.

Unless specified otherwise, all text data which represents a name or id sent in commands must only contain any of the characters `0-9, a-z, A-Z` as well as ` `, `.`, `_`, `-`, and any `Latin Letters` (Unicode characters with the values 192-214, 216-246, and 248-687) and must be between 2 and 32 characters long.

### Example command:

A command consists of the following parts:

- `command` can be any of the commands listed in the **List of commands** section.
- `id` is an **optional** string that will be sent back with the reply to your command. If you omit this property, the command is treated as "fire and forget" and there will be no reply to it, even if an exception is thrown.
- further data depending on the command.

```json
{    
    "command": "command name",
    "id": "frame id",
    "payload": "some data"
}
```

### Example response or event:

A message from the server contains the following parts:

- `kind` as listed below.
- `id` is only sent in replies to commands.
- `result` containing the result in case of a command reply message.
- `events` containing an array with data in case of an events message.

```json
{
    "kind": "success",
    "id": "frame id",
    "result": "some data",
}
```

```json
{
    "kind": "events",
    "events": [
        
    ]
}
```

Possible values for 'kind' are:

- `success` means a command was executed.
- `failure` means a command could not be executed. This happens, for example, if you try to do something with a ship which has been destroyed.
- `events` means the message from the server is not a reply to a command sent by the client. This may, for example, be a list of scanned objects.

### Example socket closure:

In the case of an invalid command being sent to the server, the websocket is closed and you will receive a close status and a closing frame containing an error text:

- `closeStatus` is the reason for the socket termination.
- `message` contains further information about the error that occurred.

## List of general errors

### WebSocketCloseStatus.InvalidPayloadData

This closeStatus is sent when the command contained invalid data.

| Error Message | Reason |
| :--- | :--- |
| Messages must consist of valid JSON data containing a valid command. | Omitting the command or using invalid characters. |
| The specified command doesn't exist. | Specifying a command that does not exist. |

## List of commands

The `command` and `id` values are not shown in the examples in this section.

### UniverseGroup commands

### Player commands

#### `whoami`

Returns the number in the player list this player is. If it's 0-63, then it's a real player, if it's 64, then it's an admin/spectator account.

This command will only be answered by the server once all metadata has been transmitted to the player. As such, it should generally be used at the beginning.

| Result type | Possible values |
| :--- | ---: |
| integer | 0-64 |