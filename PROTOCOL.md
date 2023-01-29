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

### WebSocketCloseStatus.InvalidMessageType

### WebSocketCloseStatus.InvalidPayloadData

This closeStatus is sent when the command contained invalid data.

| Error Message | Reason |
| :--- | :--- |
| Messages must consist of valid JSON data containing a valid command. | Omitting the command or using invalid characters. |
| The specified command doesn't exist. | Specifying a command that does not exist. |

### WebSocketCloseStatus.InternalServerError

### WebSocketCloseStatus.MessageTooBig

### WebSocketCloseStatus.NormalClosure

### WebSocketCloseStatus.ProtocolError

## List of commands

The `command` and `id` values are not shown in the examples in this section.

### UniverseGroup commands

### Player commands

#### `whoami`

Returns the number in the player list this player is. If it's 0-63, then it's a real player, if it's 64, then it's an admin/spectator account.

This command will only be answered by the server once all metadata has been transmitted to the player. As such, it should generally be used at the beginning.

## List of units and their definition

Some units may have changed in this version of flattiverse. So it is generally a good idea to read this section carefully. Please note that optional properties can always be `null` or just not there. The server will always send you the most economical JSON he can generate (so not sending unused values). Those JSON examples contain the best approximation of what values could do. So, if you see a number without decimal point there, you can assume an integer. If you see a decimal point it's most likely a double, etc. If values can get negative there will be an example with negative values.

### Unit

The `Unit` as parent class has some properties which will be inherites by child unit kinds. In general we have 3 kinds of units:

1. `Still` units: Those units can't move at all. (Except for you move them in the map editor.)
2. `Steady` units: Those units move according to a predefined ruleset (`orbiting`). Orbiting units have an array of `distance`, `startAngle` and `rotationInterval`. Each entry extends the orbiting information(s). You also could generate complex paths according to fourier. :) In such a case the Position of the unit is used as center of the orbiting calculations and each entry results into another point. We will add a calculation example later.
3. `Mobile` units: Those units can move freely. Examples are player ships or shots.

A full unit definition:

```json
{
    "kind": "sun",
    "name": "Zirp",
    "team": 0,
    "position":
    {
        "x": -2.5,
        "y": 0.5
    },
    "setPosition":
    {
        "x": -2.5,
        "y": 0.5
    },
    "radius": 240.6,
    "setRadius": 240.6,
    "gravity": -0.7,
    "orbiting":
    [
        {
            "distance": 200.2,
            "angle": 82.7,
            "interval": 600
        }
    ]
}
```

There are more flags a unit can have like `masking` (in the C# connector the property `IsMasking`) or `solid` (in the C# connector the property `IsSolid`). Those flags are entirely calculated from the status of the unit itself. For instance a sun is always `masking` meaning you can't scan "through" a suns core.

* Names (`name`) must match the criteria for proper names. However, the server can create dynamic names which include forbidden characters like a `#` (hashtag).
* The `team` property specifies the team this unit belongs to. It is possible to have no team assigned to a unit. In that case `team` can be `null` or you just don't send the `team` property at all.
* The `position` is the current position of the unit in the game. This is a in game status and can't be set via the map editr. There can't be an unit without position. A position is always a `vector`.
* The `setPosition` is the position relevant for the map editor API and also is the center for the `orbiting` instruction. This values isn't there if you scan the unit in game.
* The `radius` is mandatory and every scannable unit has one. Don't use this value for the map editor, it will be ignored.
* The `setRadius` is mandatory and must be given for every unit specified via the map editor API. It's the configured radius for the unit. However, some units may change their radius due to in game activities and therefore `setRadius` and `radius` may differ.
* The `gravity` property can be omitted by sending null or leaving out the property at all.
* The `orbiting` array can also be omitted if oyu don't want to have a `steady` unit. All sub values are mandatory:
  * `distance` specifies the distance from the current position the orbiting operation will move the unit.
  * `angle` specifies the start-angle of the orbiting calculations.
  * `interval` specifies how many ticks it takes for one unit to move around it's orbitee position.

### Sun (`sun`)

The sun is one of the base units in this game. You can draw energy from the suns corona sections (or get energy drained). (This is a major change to versions before.) Suns now have corona sections instead of full circle coronas. Also corona sections can have a configurable dynamic, which leads to corona sections being randomly put on and off.

```json
{
    "corona":
    {
        "radius": 777.4,
        "energy": 60.3,
        "particles": 1.1
    },
    "sections":
    [
        {
            "angleStart": 17.3,
            "angleEnd": 68.2,
            "distanceStart": 500.2,
            "distanceEnd": 702.8,
            "energy": -45.2,
            "particles": 17.2,
            "activation":
            {
                "propability": 0.0025,
                "foreshadowing": 60,
                "upramp": 40,
                "time": 120,
                "fade": 80
            },
            "activationState":
            {
                "state": "upramp",
                "frame": 13
            }
        }
    ]
}
```

Suns have two modes of operation, which could also be combined:

1. A regular sun (like in the flattiverse versions before) suitable for beginners. Those suns only have one `corona` and is defined in the property `corona`.
2. A "more complex sun" (suitable for some advanced gameplay in the future). Those suns have "corona sections" and are defined in the array `sections`.

The meaning of the values are as follows:

* `corona` specifies the more simple corona of a sun and is optional:
  * `radius` specifies the radius of the corona and is also counted from the middle of the sun. A corona radius smaller than the sun doesn't make much sense from a gameplay stand of view.
  * `energy` is optional (`null` or property doesn't exist). If `energy` is set it will load (or unload in case of a negative value) the energy of a ship offset with the `solarCells` a ship has.
  * `particles` works like `energy` but for particles (a secondary form of energy).
* `sections` specify the more complex sun behavior.
  * `angleStart`, `angleEnd`, 'distanceStart' and `distanceEnd` specify the radial sun section. A sips center must be in this section for the loading process to work. The angle ist counted from start to end. 330 to 30 will give you a 60 degree section from -30 to +30 degrees.
  * `energy` and `particles` work like described in the `corona` object above.
  * `activation` is another property which specifies a more dynamic availability behavior: A section must be activated by `propability` (see next point). `activation` is optional if oyu don't want to use it.
    * `propability`: If a section is disabled a random number generator is checked each tick against this number. (RNG < `propability` starts the sequence.)
    * `foreshadowing`: If the random number generator has triggered then we wait this amount of ticks before we activate the section. But we show this to the player if he is scanning the unit actively. This is optional, if you want to not use this pahse.
    * `upramp`: Also optional. This upramps the effects of `energy` and `particles` (from 0 to the set values).
    * `time`: Not optional. The amount of ticks before this phase fades out again.
    * `fade`: Like `upramp` but the opposite: The effects of `energy` and `particles` fade out (to 0). Also optional.
  * `activationState` is a in game state and therefore not part of the map editor JSON. It specifies in thich state the corona section currently is:
    * `state`: The state is one of:
      * `inactive`: The corona section is currently inactive and waiting for the random number generator to kick in.
      * `foreshadowing`, `upramp` and `fade`: The corona segment is in the corresponding state. See `frame` to know when this state will end.
      * `active`: The corona segment is currently active. See `frame` to know when this state will end.
    * `frame`: The current tick in the current `state`.