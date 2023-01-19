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

# List of known and unknown commands

CreateUnit:
  returns: 0 if successful, -1 otherwise

```json
  "data":
  {
    "universe": number (short),
    "unit": unit data
  }
```


UpdateUnit:
  returns: 0 if successful, -1 otherwise

```json
  "data":
  {
    "universe": number (short),
    "unit": unit data
  }
```

DeleteUnit:
  returns: 0 if successful, -1 otherwise

```json
  "data":
  {
    "universe": number (short),
    "name": text of max. 32 characters size
  }
```

yadda, yadda...

# Unit Body Structure

Unit base:
{
  "kind": "<unit type>",
  "name": "<unique unit name>",
  "position :
    { "x" : double, "y" : double },
  "radius" : double,
  "gravity" : double,
  "universe" : short,
  "universegroup" : short
}
  
each kind of unit has additional fields to add to this json Body and a specific value that needs to be filled into the "kind" field.
  
Sun:
{
  "kind" : "Sun",
  "corona" : double"
}
