# Flattiverse WebSocket JSON Protocol: Universe Simulator

This is the documentation of the Flattiverse JSON protocol for the Universe Simulator. The Flattiverse Universe Simulator is a service which takes care of one universe group including their universes, units, players, etc.. You can't access an universe group when connected to another universe group.

You can connect to the Universe Simulator (to a specific universe group) by opening a WebSocket connection to it. You can send single text frames which must not exceed 16 KiB of data. Note that you must split up each command into one frame. The server, however can send you frames up to 256 KiB. Also the server will group commands together when sending bulks of events like when a server tick has been processed.

Commands are expected to be in JSON format.

# Sending commands

```json
{
  "command": "your command",
  "id": "asdfgh",
  "data":
  {
    ...
  }
}

{
  "command": "UpdateUnit",
  "id": "asdfgh",
  "data":
  {
    "universegroup": 0,
    "universe": 0,
    "name": "NiceUnit>9000",
    "position":
    {
      "x": 1,
      "y": 12
    },
    "radius": 10,
    "corona": 67,
    "gravity": 13
  }
}

```

You can identify the reply to your command by waiting for an reply to the `id` you did specify.
If a command fails with an exception a JSON with following structure will be send:

{
  "fatal": Exception Message as Text  
}

# List of known and unknown commands

CreateUnit
[
  returns: 0 if successful, -1 otherwise
  payload:
  {
    "universegroup": ushort,
    "universe": ushort,
    "name": text of max. 32 characters size,
    "position":
    {
      "x": double precision number,
      "y": double precision number
    },
    "radius": double precision number,
    "corona": double precision number,
    "gravity": double precision number
  }
]

UpdateUnit:
[
  returns: 0 if successful, -1 otherwise
  payload:
  {
    "universegroup": ushort,
    "universe": ushort,
    "name": text of max. 32 characters size,
    "position":
    {
      "x": double precision number,
      "y": double precision number
    },
    "radius": double precision number,
    "corona": double precision number,
    "gravity": double precision number
  }
]

DeleteUnit:
[
  returns: 0 if successful, -1 otherwise
  payload:
  {
    "universegroup": ushort,
    "universe": ushort,
    "name": text of max. 32 characters size
  }
]

yadda, yadda...

# Unit Body Structure

Unit base:
```json
{
  "kind": "<unit type>",
  "name": "<unique unit name>",
  "position" :
    { "x" : 0.0, "y" : 0.0 },
  "radius" : 0.0,
  "gravity" : 0.0,
}
```
  
each kind of unit has additional fields to add to this json Body and a specific value that needs to be filled into the "kind" field.
  
Sun:
```json
{
  "kind" : "Sun",
  "corona" : 0.0
}
```
