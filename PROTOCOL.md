# Flattiverse WebSocket JSON Protocol: Universe Simulator

This is the documentation of the Flattiverse JSON protocol for the Universe Simulator. The Flattiverse Universe Simulator is a service which takes care of one universe group including their universes, units, players, etc.. You can't access an universe group when connected to another universe group.

You can connect to the Universe Simulator (to a specific universe group) by opening a WebSocket connection to it. You can send single text frames which must not exceed 16 KiB of data. Note that you must split up each command into one frame. The server, however can send you frames up to 256 KiB. Also the server will group commands together when sending bulks of events like when a server tick has been processed.

# Connecting

Simply connect with your websocket to the specific URI with port 80.

# Sending commands

Commands are expected to be in JSON format.
To execute a command on the server simply send a request with the corresponding command as payload.

```json
{
  "command": "command name",
  "id": "asdfgh",
  "data":
  {
    ...
  }
}

{
  "command": "updateunit",
  "id": "asdfgh",
  "data":
  {
    "universe": 0,
    "unit":
    {
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
}

```

You can identify the reply to your command by waiting for an reply to the `id` you did specify.
If a command fails with an exception a JSON with following structure will be send:

{  "fatal": Exception Message as Text }

Example JSON Response:

```json
{
  "kind": "success",
  "id": "exampleId",
  "result": see command result
}
```

# List of known and unknown commands

setunit:
  Creates or updates an unit, complete unit data is expected.
  returns: 0 if successful, -1 otherwise

```json
  {
    "command": "deleteunit",
    "id": "exampleId",
    "data":
    {
      "universe": number (short),
      "unit": unit data
    }
  }
```

deleteunit:
  Deletes an existing unit.
  returns: 0 if successful, -1 otherwise

```json
  {
    "command": "deleteunit",
    "id": "exampleId",
    "universe": number (short),
    "name": text of max. 32 characters size
  }
```

yadda, yadda...

# Unit Body Structure

Unit base:
```json
{
  "kind": "<unit type>",
  "name": "<unique unit name>",
  "position" :
    {
      "x" : 0.0,
     "y" : 0.0 
    },
  "radius" : 0.0,
  "gravity" : 0.0,
}
```
  
each kind of unit has additional fields to add to this json Body and a specific value that needs to be filled into the "kind" field.
  
Sun:
```json
{
  "kind" : "sun",
  ... general unit data...
  "corona" : 0.0
}
```

# Universe Updates
the initial universe update sent with the initial tick, and all units in the "created" object. Ticks without changes are not sent.
```json
{
  "tick": 0,
  "payload":
  {    
    [
      {
        "kind": "newunit",
        "universe": 0,
        "unit":
        {
          "kind": "sun",
          "name": "zirp",
          "position":
          {
            "x": 20,
            "y": 10
          }
        }
      },
      {
        "kind": "message",
        "message":
        {
          "sender": "tobi",
          "receiver": "broadcast",
          "timestamp": "yyyy-mm-ddTdd:hh:mm:ss.fffZ",
          "text": "Hallo Freunde"
        }
      }
    ]
  }
}
```
