# Flattiverse WebSocket JSON Protocol

Flattiverse is a simulated 2D universe intended for students and others interested in getting a practical introduction to object-oriented programming. This documentation describes the protocol used to interact with Flattiverse.

Communication is carried out by sending single text frames which must not exceed 16 KiB of data and must not be splitted, each frame containing one command at a time. The server responds with frames up to 256 KiB, and will also group commands together when sending bulks of events, for example when a server tick has been processed.

## Connection and socket behaviour

You can interacht with a Flattiverse `UniverseGroup` by connecting to the specific URI with a websocket on port 80.

All commands are expected to be in JSON format. To execute a command on the server, simply send a request with the corresponding command as payload.

Unless specified otherwise, all text data sent in commands must only contain any of the characters `0-9, a-z, A-Z` as well as ` `, `.`, `_`, `-`, and any `Latin Letters`.

### Example request:

A command consists of the following parts:

- `command` can be any of the commands listed in the **List of commands** section.
- `id` is an **optional** string that will be sent back with the reply to your command. If you omit this property, the command is treated as "fire and forget" and there will be no reply to it.
- further data depending on the command.

```json
{    
    "command": "command name",
    "id": "frame id",
    "payload": "some data"
}
```

### Example Response:

A reply consists of the following parts:

- `kind` 

```json
{
    "kind": "success",
    "id": "frame id",    
    "result": "see command result"
}
```

Possible values for 'kind' are:

- `success` means the command was executed.
- `failure` means the command could not be executed. This happens, for example, if you try to do something with a ship which has been destroyed.

### Example Termination:

In the case of an invalid command being sent to the server, the websocket is closed and you will receive a closing frame containing a JSON with the following structure:

```json
{  
    "fatal": "Exception Message as Text"
}
```

## List of general errors

//Message toobig etc. wann?

## List of commands

### UniverseGroup commands

### Player commands