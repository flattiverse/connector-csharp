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

## List of HTTP error codes

The following HTTP error codes are sent by the server if connecting to the universeGroup fails:

### `HTTP/400`

- The endpoint was called with anything other than a websocket.
- There was a failure in the database.

### `HTTP/401`

- No auth parameter was given, or a malformed or nonexisting auth key was given. A proper auth parameter consists of string of 64 characters representing hex values.
- A connection as a spectator was attempted, but the UniverseGroup does not allow spectators.

### `HTTP/412`

- A connection as a player or admin was attempted, but the associated account is still online with another connection. As disconnecting players will linger for a while, a connection may not be possible for a short time even if a previous connection has been closed or severed.

### `HTTP/417`

- The UniverseGroup is currently at capacity and no further connections are possible.

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

## Vectors

There are two kinds of vector information which are used in the commands:

- x/y coordinates which are always absolute values. That means, the position of a scanned object is not relative to you, but absolute in the universe; also, movement vectors of scanned ships are also absolute and not relative to you.
- angles indicating the rotation of an object, like the direction a ship is pointing to.

```
+------------------------------- (+X)
|         , - ~ ~ ~ - ,
|     , '      |(0°)    ' ,
|   ,          |            ,
|  ,           |             ,
| ,            |              ,
| ,            X--------(90°) ,
| ,                           ,
|  ,                         ,
|   ,                       ,
|     ,                  , '
|       ' - , _ _ _ ,  '
(+Y)
```

JSON structure of a vector:

```json
{
    "x": -1.5,
    "y": 1.5
}
```

## List of commands

The `command` and `id` values are not shown in the examples in this section.

Possible value types are:

- `string` is a text value.
- `integer` is an integer number value like 5.
- `double` is a number value like 7.5 or 10.
- `vector` is a json object containing an x and y value.
- `json` is a general json object.
- `playerUnit` is a json object containing the values of a playerUnit.
- `universe` is a json object containing the values of a universe.

### UniverseGroup commands

### Player commands

#### `whoami`

Returns the number in the player list this player is. If it's 0-63, then it's a real player, if it's 64, then it's an admin/spectator account.

This command will only be answered by the server once all metadata has been transmitted to the player. As such, it should generally be used at the beginning before giving the control to the player.

Returns: `integer`.

#### `unitSet`

Sets the unit to the specified settings. You need to specify the parameters `universe` (integer, id) and `unit` (string).

The unit will be created or replaced (if possible). Replacing a non editable unit (name colission with a playership, etc.) will result in the corresponding `GameException`.

Returns: Nothing (empty confirmation) or `GameException`.

#### `unitRemove`

Removes the unit from the universe. You need to specify the parameters `universe` (integer, id) and `unit` (string).

Trying to remove a non editable or non existing unit will result in the corresponding `GameException`.

Returns: Nothing (empty confirmation) or `GameException`.

#### `unitGet`

Retrieves a map-editable json of the unit from the universe. You need to specify the parameters `universe` (integer, id) and `unit` (string).

Trying to retrieve a non editable or non existing unit will result in the corresponding `GameException`.

Returns: The map-editable json data or `GameException`.

#### `regionSet`

Sets the `Region` to the specified settings in the universe. You need to specify the parameters `universe` (integer, id), `regionId` (integer, id, 0-255), `teams` (integer, bitmask for 16 teams), `name` (string, can be empty), `left` `top` `right` `bottom` (double), and `startLocation` `safeZone` `slowRestore` (int, 1 or 0).

The region will be created or replaced.

Returns: Nothing (empty confirmation) or `GameException`.

#### `regionRemove`

Removes the `Region` from the universe. You need to specify the parameters `universe` (integer, id), and `regionId` (integer, id, 0-255).

Returns: Nothing (empty confirmation) or `GameException`.

#### `regionRemove`

Removes the `Region` from the universe. You need to specify the parameters `universe` (integer, id), and `regionId` (integer, id, 0-255).

Returns: Nothing (empty confirmation) or `GameException`.

#### `systemSet` and `systemSetRequired`

tbd

#### `systemRemove`

tbd

#### `systemList`

tbd


## List of units and their definition

Some units may have changed in this version of flattiverse. So it is generally a good idea to read this section carefully. Please note that optional properties may be `null` or omitted. The server will always send you the most economical JSON it can generate (skipping unnecessary parts). The following JSON examples contain the best approximation of what values could do. So, if you see a number without decimal point there, you can assume an integer. If you see a decimal point it's most likely a double, and if values can get negative there will be an example with negative values.

### Unit

The `Unit` as parent class has some properties which will be inherited by child unit kinds. In general we have 3 kinds of units:

- `Still` units: Those units can't move at all (except for when moved in the map editor).
- `Steady` units: Those units move according to a predefined ruleset (`orbiting`). Orbiting units have an array of orbiting informations containing `distance`, `startAngle` and `rotationInterval`. Each entry extends the orbiting information(s). You also could generate complex paths according to fourier :). In such a case the Position of the unit is used as center of the orbiting calculations and each entry results into another point. We will add a calculation example later.
- `Mobile` units: Those units can move freely. Examples are player ships or shots.

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
    "movement":
    {
        "x": -1.5,
        "y": 1.5
    },
    "mobility": "still"
    "radius": 240.6,
    "setRadius": 240.6,
    "direction": 71.2,
    "gravity": -0.7,
}
```

Additional parameters for (`still` and) `steady` units - (a `still` unit becomes a `steady` unit when those parameters appear):

```json
{
    "orbitingCenter":
    {
        "x": 222.2,
        "y": -160.2
    },
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

- Names (`name`) must match the criteria for proper names. However, the server can create dynamic names which include forbidden characters like a `#` (hashtag).
- The `team` property specifies the team this unit belongs to. It is possible to have no team assigned to a unit. In that case `team` can be `null` or you just don't send the `team` property at all.
- The `position` is the current position of the unit in the game. This is a in game status and can't be set via the map editr. There can't be an unit without position. A position is always a `vector`.
- The `setPosition` is the position relevant for the map editor API and also is the center for the `orbiting` instruction. This values isn't there if you scan the unit in game.
- The `movement` is the movement vector of the unit. With each tick the `movement` is added onto the `position`.
- The `mobility` specifies if the unit is `still` (sitting on the same point all day long), `steady` (orbiting) or `mobile` (player ship, etc.).
- The `radius` is mandatory and every scannable unit has one. Don't use this value for the map editor, it will be ignored.
- The `setRadius` is mandatory and must be given for every unit specified via the map editor API. It's the configured radius for the unit. However, some units may change their radius due to in game activities and therefore `setRadius` and `radius` may differ.
- The `gravity` property can be omitted by sending null or leaving out the property at all.
- The `orbitingCenter` returns the `setPosition` in the case of orbiting units.
- The `orbiting` array can also be omitted if oyu don't want to have a `steady` unit. All sub values are mandatory:
  - `distance` specifies the distance from the current position the orbiting operation will move the unit.
  - `angle` specifies the start-angle of the orbiting calculations.
  - `interval` specifies how many ticks it takes for one unit to move around it's orbitee position.

### Reduced Unit

Reduced units will be sent to you when you just use the regular scan. You will only get detailed unit informations if you directly scan the target unit for at least some ticks. Reduced units give the following infomrations:

```json
{
    "kind": "reduced",
    "probableKind": "planet"
    "name": "Zorp",
    "team": "enemy",
    "position":
    {
        "x": -200.5,
        "y": 100.5
    },
    "movement":
    {
        "x": -1.5,
        "y": 1.5
    },
    "radius": 240.6,
    "gravity": 0.7,
    "energyOutput": 18.2
}
```

The `probableKind` information reduces units which are looking quite like the same into the same kinds. We have those groups, in which the first item will always be returned even if the unit is actually one of the other kinds:
- `sun` and `blackhole`.
- `planet`, `moon`, `meteoroid`, and `comet`.
- `asteroid` and `trash`.
- `playerUnit` and `aiUnit`.
- `shot`.
- `explosion`.
- `resource`, `powerup`, `missionTarget`, and `buoy`.

The `team` flag will only tell you if a unit has the `same` team, an `enemy` team or `none` (no) team. The `energyOutput` will specify the energy output of the unit. For suns this will be quite high, for example for ships without enabled afterburners it will be low.

### Sun (`sun`)

The sun is one of the base units in this game. You can draw energy from the suns corona sections, or get energy drained. A major change to versions before, suns now have corona sections instead of full circle coronas. Also, corona sections can have a configurable dynamic, which leads to corona sections being randomly put on and off.

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
                "probability": 0.0025,
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

- A regular sun (like in the flattiverse versions before) suitable for beginners. Those suns only have one `corona` and is defined in the property `corona`.
- A "more complex sun" (suitable for some advanced gameplay in the future). Those suns have "corona sections" and are defined in the array `sections`.

The meaning of the values are as follows:

- `corona` specifies the more simple corona of a sun and is optional:
  - `radius` specifies the radius of the corona and is also counted from the middle of the sun. A corona radius smaller than the sun doesn't make much sense from a gameplay stand of view.
  - `energy` is optional (`null` or property doesn't exist). If `energy` is set it will load (or unload in case of a negative value) the energy of a ship offset with the `solarCells` a ship has.
  - `particles` works like `energy` but for particles (a secondary form of energy).
- `sections` specify the more complex sun behavior.
  - `angleStart`, `angleEnd`, 'distanceStart' and `distanceEnd` specify the radial sun section. A ships center must be in this section for the loading process to work. The angle ist counted from start to end. 330 to 30 will give you a 60 degree section from -30 to +30 degrees.
  - `energy` and `particles` work like described in the `corona` object above.
  - `activation` is another property which specifies a more dynamic availability behavior: A section must be activated by `probability` (see next point). `activation` is optional if you don't want to use it. If used, at least probability and time must be set, the other values are optional.
    - `probability`: If a section is disabled a random number generator is checked each tick against this number. (RNG < `probability` starts the sequence.)
    - `foreshadowing`: If the random number generator has triggered then we wait this amount of ticks before we activate the section. But we show this to the player if he is scanning the unit actively. This is optional, if you want to not use this pahse.
    - `rampup`: Also optional. This ramps up the effects of `energy` and `particles` (from 0 to the set values).
    - `time`: Not optional. The amount of ticks before this phase fades out again.
    - `fade`: Like `upramp` but the opposite: The effects of `energy` and `particles` fade out (to 0). Also optional.
  - `activationState` is a in game state and therefore not part of the map editor JSON. It specifies in thich state the corona section currently is:
    - `state`: The state is one of:
      - `inactive`: The corona section is currently inactive and waiting for the random number generator to kick in.
      - `foreshadowing`, `upramp` and `fade`: The corona segment is in the corresponding state. See `frame` to know when this state will end.
      - `active`: The corona segment is currently active. See `frame` to know when this state will end.
    - `frame`: The current tick in the current `state`.

### Black hole (`blackhole`)

The black hole is also one of the first units ever in this game. A black hole usually moves you towards the center and has a high gravity. Like with the sun it can have a "corona" (which in this case is a "gravity well") or "gravity sections" like the "corona sections".

```json
{
    "gravityWell":
    {
        "radius": 777.4,
        "force": 0.02
    },
    "sections":
    [
        {
            "angleStart": 17.3,
            "angleEnd": 68.2,
            "distanceStart": 500.2,
            "distanceEnd": 702.8,
            "force": 0.02,
            "activation":
            {
                "probability": 0.0025,
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

Black holes have two modes of operation, which could also be combined:

- A regular black hole (like in the flattiverse versions before) suitable for beginners. Those black holes only have one `gravityWell` and are defined in the property `gravityWell`.
- A "more complex black hole" (suitable for some advanced gameplay in the future). Those black holes have "gravity sections" and are defined in the array `sections`.

The meaning of the values are as follows:

- `gravityWell` specifies the more simple gravityWell of a black hole and is optional:
  - `radius` specifies the radius of the gravityWell and is also counted from the middle of the black hole. A gravity well radius smaller than the black hole doesn't make much sense from a gameplay stand of view.
  - `force` specifies the movement vector imparted on mobile units which remain inside the gravity well's radius with their center point.
- `sections` specify the more complex black hole behavior.
  - `angleStart`, `angleEnd`, 'distanceStart' and `distanceEnd` specify the radial black hole section. A ships center must be in this section to be affected by the gravitational effect. The angle ist counted from start to end. 330 to 30 will give you a 60 degree section from -30 to +30 degrees.
  - `force` works like described in the `gravityWell` object above.
  - `activation` is another property which specifies a more dynamic availability behavior: A section must be activated by `probability` (see next point). `activation` is optional if you don't want to use it. If used, at least probability and time must be set, the other values are optional.
    - `probability`: If a section is disabled a random number generator is checked each tick against this number. (RNG < `probability` starts the sequence.)
    - `foreshadowing`: If the random number generator has triggered then we wait this amount of ticks before we activate the section. But we show this to the player if he is scanning the unit actively. This is optional, if you want to not use this pahse.
    - `rampup`: Also optional. This ramps up the effects of `force` (from 0 to the set values).
    - `time`: Not optional. The amount of ticks before this phase fades out again.
    - `fade`: Like `upramp` but the opposite: The effects of `force` fade out (to 0). Also optional.
  - `activationState` is a in game state and therefore not part of the map editor JSON. It specifies in thich state the gravity well section currently is:
    - `state`: The state is one of:
      - `inactive`: The gravity well section is currently inactive and waiting for the random number generator to kick in.
      - `foreshadowing`, `upramp` and `fade`: The gravity well segment is in the corresponding state. See `frame` to know when this state will end.
      - `active`: The gravity well segment is currently active. See `frame` to know when this state will end.
    - `frame`: The current tick in the current `state`.


### Planet (`planet`), Moon (`moon`), Meteoroid (`meteoroid`) and Comet (`comet`)

Planets, moons, meteoroids and comets work all the same: They are "stupid" units which are only distinguished because of look and feel visuals. All these units are `solid` and `masking` and can carry resources. As a map editor you should follow the following rules:

- Planets (`planet`) usually carry the `iron` resource and with lesser priority `carbon` and `silicon`. Planets usually do carry resources.
- Moons (`moon`) may carry `silicon` as a resource. Other resources are less common.
- Meteoroids (`meteoroid`) may carry `ìron` and may carry `platinum` with a propability of < 1%.
- Comets (`comet`) may carry `gold`.

Just to give you a flat list, those are the available resources in flattiverse:

- `iron` (common): Used for almost all structural changes on your ship.
- `platinum` (rare): Used to improve the performance of structural components.
- `carbon` (common) and `silicon` (uncommon): Used for most electrical (things which contain circuit boards, cpus, etc.) upgrades.
- `gold` (rare): Used to improve the performance of electrical systems.

Those additional properties (in regards to unit or `solid`/`steady` unit can be found):

```json
{    
    "resources":
    {
        "iron": 22.4,
        "platinum": 2.2,
        "carbon": 1.1,
        "silicon": 4.1,
        "gold": 0.7
    }
}
```

`resources` child elements specify the corresponding extraction rate of the corresponding resource per tick.

### PlayerUnit (`playerUnit`)

The player unit is a player ship or base (or probe or drone or platform or whatever, it really just depends on the configuration). This unit is controlled by a player and therefore can't be edited by the map editor. It's undoubtly the most complex unit in the game - at least regarding it's in game configuration. Player units can be spawned by a player when he is calling `controllableRegister` and player units are of course `mobile` units.

Player units are persistent units, so even if they get destroyed, they won't be removed from the game (but from the scan). Player units will be removed from the game after a player did disconnect or logoff properly or after calling `controllableUnregister`.

A player units JSON will look like this:

```json
{
    "turnRate": -7.2,
    "systems":
    {
        "hull":
        {
            "level": 1,
            "value": 37.1
        },
        "armor":
        {
            "level": 7,
            "value": 2
        },
        "shield":
        {
            "level": 2,
            "value": 16.2
        },
        "thruster":
        {
            "level": 1,
            "value": 0.2
        },
        "nozzle":
        {
            "level": 3,
            "value": -2.3
        },
        "scanner":
        {
            "level": 2,
            "value": 6
        },
        "analyzer":
        {
            "level": 4,
            "value": 2
        },
        "cellsEnergy":
        {
            "level": 4,
            "value": 2.1
        },
        "cellsParticles":
        {
            "level": 4,
            "value": 2.1
        },
        "batteryEnergy":
        {
            "level": 4,
            "value": 6273.2
        },
        "batteryParticles":
        {
            "level": 4,
            "value": 272.3
        },
        "weapon":
        {
            "levelLauncher": 2,
            "levelPayloadDamage": 3,
            "levelPayloadRadius": 3,
            "levelFactory": 4,
            "levelStorage": 5,
            "ammunition": 7.2,
        },
        "cargo":
        {
            "levelIron": 3,
            "valueIron": 16.2,
            "levelCarbon": 2,
            "valueCarbon": 0.1,
            "levelSilicon": 4,
            "valueSilicon": 0.2,
            "levelPlatinum": 1,
            "valuePlatinum": 0.11,
            "levelGold": 7,
            "valueGold": 22.1,
            "levelSpecial": 1,
            "contentSpecial":
            [
                "missionTarget"
            ]
        },
        "extractor":
        {
            "levelIron": 1,
            "valueIron": 1.1,
            "levelCarbon": 1,
            "valueCarbon": 1.1,
            "levelSilicon": 1,
            "valueSilicon": 1.2,
            "levelPlatinum": 2,
            "valuePlatinum": 1.11,
            "levelGold": 2,
            "valueGold": 1.1
        }
    }
}
```

- `turnRate` specifies the speed of much quick the ship is turning currently. The `direction` will change by every tick of `turnRate`.
- `systems` specify the various ship systems. Connectors for flattiverse need to implement some kind of mechanic to derive the limits for the corresponding system from the level. We will document here various maximums, however they will currently change quite quick.
  - `hull`, `shield`, `thruster`, `nozzle`, `scanner`, `analyzer`, `cellsEnergy`, `cellsParticles`, `batteryEnergy`, `batteryParticles` specify "simple" systems which indicate the status of the corresponding system. Some examples: From the `hull`.`level` property you can derive the maximum hitpoints of the ships hull. (See next section.) From `hull`.`value` you can see the current status, in this case how many hitpoints the ships hull has. Another example would be `cellsParticles`. You can derive the collection speed of particles from `cellsParticles`.`level`. `value` is the current loading rate and therefore is 0 most of the time (eg. when not near a sun). All other components mentioned here work somehow like the same: From `level` you can derive the maximum capabilities or maximum values of the system.
  - `armor` works like the other systems, however the `value` works in another way: `level` specifies how good the armors protection is while the value specifies how much of the protection is still available. The `armor` can be refilled with resources, so to say.
  - `weapon` is a complex system and consists of the following sub systems, states:
    - `levelLauncher`: Specifies the maximum speed of the launched projectile.
    - `levelPayloadDamage`: Specifies the damage a projectiles explosion does.
    - `levelPayloadRadius`: Specifies the radius of the resulting explosion.
    - `levelFactory`: Specifies how quick shots are produced.
    - `levelStorage`: Specifies how many produced shots can be stored.
    - `ammunition`: Specifies the amount of shots ready. The number of 7.2 indicates 7 ready shots and an 8th shot in production finished by 20%.
  - `cargo` specifies the cargo capabilities (`level`) and load (`value`) per resource. But there is a special system:
    - `levelSpecial`: Specifies how huge the special cargo is.
    - `contentSpecial`: This array holds strings describing all carried things in the special cargo. This is used for future game modes like "capture the flag".
  - `extractor`: This complex system and the properties below this object specify the status of all extractor systems - one system per ressource. This follows the same patterns as before while the corresponding `value` is the extraction rate.

### Shot (`shot`)

A shot is a projectile launched by a PlayerUnit. It will explode, resulting in a damage-dealing explosion, after a short while or when hitting another object.

A shot has the following values:

- `explosionDamage` specifies the damage the resulting explosion will deal.
- `explosionRadius` specifies the radius of the resulting explosion.
- `lifetime` specifies the time until the shot detonates, if it does not collide with another unit until then.

```json
{    
    "explosionDamage": 11.5,
    "explosionRadius": 40.1,
    "lifetime": 29
}
```

### Explosion (`explosion`)

An explosion is the consequence of an exploding shot, or of another unit saying it's last goodbye in the form of a fiery ball of destruction.

An explosion has only one value:

- `damage` specifies the damage dealt by this explosion to all units caught in it's radius.

```json
{    
    "damage": 16.2    
}
```
