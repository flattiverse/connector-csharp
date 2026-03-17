using System.Globalization;
using System.Text;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Units;

namespace CliShip;

class Program
{
    private const string DefaultUri = "ws://127.0.0.1:5000";
    private const string DefaultTeam = "Pink";
    private const string DefaultAuth = "replace-with-player-auth";
    private const string DefaultShipName = "CliShip";
    private const float PositionGain = 0.02f;
    private const float MovementGain = 1f;
    private const float PositionDeadzone = 0.25f;
    private const float MovementDeadzone = 0.01f;
    private const float EngineDeadzone = 0.001f;
    private const float AwaitPositionRadius = 1f;

    private readonly struct Options
    {
        public readonly string Uri;
        public readonly string Auth;
        public readonly string Team;
        public readonly string ShipName;
        public readonly uint MaxTicks;
        public readonly string[] Commands;

        public Options(string uri, string auth, string team, string shipName, uint maxTicks, string[] commands)
        {
            Uri = uri;
            Auth = auth;
            Team = team;
            ShipName = shipName;
            MaxTicks = maxTicks;
            Commands = commands;
        }
    }

    private sealed class PolicyState
    {
        public bool AutoContinue;
        public bool ContinuePending;
        public bool HasTarget;
        public Vector? Target;
        public bool HasScan;
        public float ScanWidth;
        public float ScanLength;
        public float ScanAngle;
        public uint StatusEvery;
    }

    private sealed class RuntimeState
    {
        public readonly Galaxy Galaxy;
        public readonly PolicyState Policies;

        public ClassicShipControllable? Ship;
        public bool HasStartTick;
        public uint StartTick;
        public uint CurrentTick;
        public uint LastStatusTick;
        public bool StopRequested;

        public RuntimeState(Galaxy galaxy)
        {
            Galaxy = galaxy;
            Policies = new PolicyState();
            Ship = null;
            HasStartTick = false;
            StartTick = 0;
            CurrentTick = 0;
            LastStatusTick = 0;
            StopRequested = false;
        }
    }

    private static async Task Main(string[] args)
    {
        bool helpRequested = false;

        foreach (string arg in args)
            if (arg == "--help" || arg == "-h")
                helpRequested = true;

        if (helpRequested)
        {
            PrintUsage();
            return;
        }

        Options options;

        try
        {
            options = ParseOptions(args);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Invalid arguments: {exception.Message}");
            PrintUsage();
            return;
        }

        Galaxy galaxy = await Galaxy.Connect(options.Uri, options.Auth, options.Team).ConfigureAwait(false);
        RuntimeState state = new RuntimeState(galaxy);

        try
        {
            Console.WriteLine($"Connected as {galaxy.Player.Name} of team {galaxy.Player.Team.Name} to {options.Uri}.");
            Console.WriteLine(options.MaxTicks == 0
                ? "Max tick limit: unlimited."
                : $"Max tick limit: {options.MaxTicks} relative ticks from the first received galaxy tick.");

            foreach (string commandText in options.Commands)
            {
                if (state.StopRequested)
                    break;

                await ExecuteCommand(options, state, commandText).ConfigureAwait(false);
            }

            while (!state.StopRequested && galaxy.Active)
            {
                FlattiverseEvent? @event = await ProcessNextEvent(options, state).ConfigureAwait(false);

                if (@event is null)
                    break;
            }
        }
        finally
        {
            galaxy.Dispose();
        }
    }

    private static Options ParseOptions(string[] args)
    {
        string uri = DefaultUri;
        string auth = DefaultAuth;
        string team = DefaultTeam;
        string shipName = DefaultShipName;
        int index = 0;

        while (index < args.Length && args[index].StartsWith("--", StringComparison.Ordinal))
        {
            string option = args[index];

            if (index + 1 >= args.Length)
                throw new ArgumentException($"Missing value for option {option}.");

            string value = args[index + 1];

            switch (option)
            {
                case "--uri":
                    uri = value;
                    break;
                case "--auth":
                    auth = value;
                    break;
                case "--team":
                    team = value;
                    break;
                case "--name":
                    shipName = value;
                    break;
                default:
                    throw new ArgumentException($"Unknown option {option}.");
            }

            index += 2;
        }

        if (index >= args.Length)
            throw new ArgumentException("Expected <maxTicks> followed by zero or more commands.");

        uint maxTicks = ParseUInt(args[index], "maxTicks");
        index++;

        int commandCount = args.Length - index;
        string[] commands = new string[commandCount];

        for (int commandIndex = 0; commandIndex < commandCount; commandIndex++)
            commands[commandIndex] = args[index + commandIndex];

        return new Options(uri, auth, team, shipName, maxTicks, commands);
    }

    private static async Task ExecuteCommand(Options options, RuntimeState state, string commandText)
    {
        string value;

        if (commandText.Equals("create", StringComparison.OrdinalIgnoreCase))
        {
            if (state.Ship is not null && state.Ship.Active)
                throw new InvalidOperationException("A ship is already selected. Use attach-id/attach-name instead.");

            ClassicShipControllable ship = await state.Galaxy.CreateClassicShip(options.ShipName).ConfigureAwait(false);
            state.Ship = ship;
            state.Policies.ContinuePending = false;
            Console.WriteLine($"COMMAND: created ClassicShip #{ship.Id} named {ship.Name}.");
            return;
        }

        if (TryReadCommandArgument(commandText, "attach-id:", out value))
        {
            byte id = ParseByte(value, "attach-id");

            if (!state.Galaxy.Controllables.TryGet(id, out Controllable? controllable) || controllable is not ClassicShipControllable ship)
                throw new InvalidOperationException($"No ClassicShip controllable with id {id} exists.");

            state.Ship = ship;
            state.Policies.ContinuePending = false;
            Console.WriteLine($"COMMAND: attached ClassicShip #{ship.Id} named {ship.Name} in cluster {ship.Cluster.Name}.");
            return;
        }

        if (TryReadCommandArgument(commandText, "attach-name:", out value))
        {
            if (!state.Galaxy.Controllables.TryGet(value, out Controllable? controllable) || controllable is not ClassicShipControllable ship)
                throw new InvalidOperationException($"No ClassicShip controllable named {value} exists.");

            state.Ship = ship;
            state.Policies.ContinuePending = false;
            Console.WriteLine($"COMMAND: attached ClassicShip #{ship.Id} named {ship.Name} in cluster {ship.Cluster.Name}.");
            return;
        }

        if (commandText.Equals("continue", StringComparison.OrdinalIgnoreCase))
        {
            ClassicShipControllable ship = RequireShip(state);
            await ship.Continue().ConfigureAwait(false);
            state.Policies.ContinuePending = true;
            Console.WriteLine($"COMMAND: continue requested for ship #{ship.Id}.");
            return;
        }

        if (commandText.Equals("status", StringComparison.OrdinalIgnoreCase))
        {
            PrintStatus(state);
            return;
        }

        if (commandText.Equals("targetoff", StringComparison.OrdinalIgnoreCase))
        {
            ClassicShipControllable ship = RequireShip(state);
            state.Policies.HasTarget = false;
            state.Policies.Target = null;

            if (ship.Active && ship.Alive && ship.Engine.Target != 0f)
            {
                await ship.Engine.Off().ConfigureAwait(false);
                Console.WriteLine($"COMMAND: target policy cleared and engine turned off for ship #{ship.Id}.");
                return;
            }

            Console.WriteLine("COMMAND: target policy cleared.");
            return;
        }

        if (commandText.Equals("scanoff", StringComparison.OrdinalIgnoreCase))
        {
            ClassicShipControllable ship = RequireShip(state);
            state.Policies.HasScan = false;

            if (ship.Active && ship.Alive && ship.MainScanner.Active)
            {
                await ship.MainScanner.Off().ConfigureAwait(false);
                Console.WriteLine($"COMMAND: scan policy cleared and main scanner turned off for ship #{ship.Id}.");
                return;
            }

            Console.WriteLine("COMMAND: scan policy cleared.");
            return;
        }

        if (commandText.Equals("engineoff", StringComparison.OrdinalIgnoreCase))
        {
            ClassicShipControllable ship = RequireShip(state);
            await ship.Engine.Off().ConfigureAwait(false);
            Console.WriteLine($"COMMAND: engine off requested for ship #{ship.Id}.");
            return;
        }

        if (commandText.Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            state.StopRequested = true;
            Console.WriteLine("COMMAND: exit requested.");
            return;
        }

        if (TryReadCommandArgument(commandText, "auto-continue:", out value))
        {
            if (value.Equals("on", StringComparison.OrdinalIgnoreCase))
            {
                state.Policies.AutoContinue = true;
                Console.WriteLine("COMMAND: auto-continue enabled.");
                return;
            }

            if (value.Equals("off", StringComparison.OrdinalIgnoreCase))
            {
                state.Policies.AutoContinue = false;
                state.Policies.ContinuePending = false;
                Console.WriteLine("COMMAND: auto-continue disabled.");
                return;
            }

            throw new ArgumentException("auto-continue expects on or off.");
        }

        if (TryReadCommandArgument(commandText, "status-every:", out value))
        {
            state.Policies.StatusEvery = ParseUInt(value, "status-every");
            Console.WriteLine(state.Policies.StatusEvery == 0
                ? "COMMAND: periodic status output disabled."
                : $"COMMAND: periodic status output every {state.Policies.StatusEvery} ticks.");
            return;
        }

        if (TryReadCommandArgument(commandText, "target:", out value))
        {
            Vector target = ParseVector(value, "target");
            state.Policies.HasTarget = true;
            state.Policies.Target = target;
            Console.WriteLine($"COMMAND: target policy set to ({target.X:0.###}, {target.Y:0.###}).");
            return;
        }

        if (TryReadCommandArgument(commandText, "scan:", out value))
        {
            float width;
            float length;
            float angle;
            ParseScan(value, out width, out length, out angle);
            state.Policies.HasScan = true;
            state.Policies.ScanWidth = width;
            state.Policies.ScanLength = length;
            state.Policies.ScanAngle = angle;
            Console.WriteLine($"COMMAND: scan policy set to width={width:0.###}, length={length:0.###}, angle={angle:0.###}.");
            return;
        }

        if (TryReadCommandArgument(commandText, "engine:", out value))
        {
            ClassicShipControllable ship = RequireShip(state);
            Vector movement = ParseVector(value, "engine");
            await ship.Engine.Set(movement).ConfigureAwait(false);
            Console.WriteLine($"COMMAND: engine target set to ({movement.X:0.###}, {movement.Y:0.###}) for ship #{ship.Id}.");
            return;
        }

        if (TryReadCommandArgument(commandText, "shoot:", out value))
        {
            ClassicShipControllable ship = RequireShip(state);
            Vector relativeMovement;
            ushort ticks;
            float load;
            float damage;
            ParseShot(value, out relativeMovement, out ticks, out load, out damage);
            await ship.Weapon.Shoot(relativeMovement, ticks, load, damage).ConfigureAwait(false);
            Console.WriteLine(
                $"COMMAND: shoot requested with movement=({relativeMovement.X:0.###}, {relativeMovement.Y:0.###}), ticks={ticks}, load={load:0.###}, damage={damage:0.###}.");
            return;
        }

        if (commandText.Equals("await-alive", StringComparison.OrdinalIgnoreCase))
        {
            await WaitForCondition(
                options,
                state,
                delegate
                {
                    ClassicShipControllable ship = RequireShip(state);
                    return ship.Alive;
                },
                delegate
                {
                    ClassicShipControllable ship = RequireShip(state);
                    return $"ship #{ship.Id} became alive at tick {state.CurrentTick} in cluster {ship.Cluster.Name}.";
                }).ConfigureAwait(false);
            return;
        }

        if (commandText.Equals("await-dead", StringComparison.OrdinalIgnoreCase))
        {
            await WaitForCondition(
                options,
                state,
                delegate
                {
                    ClassicShipControllable ship = RequireShip(state);
                    return !ship.Alive;
                },
                delegate
                {
                    ClassicShipControllable ship = RequireShip(state);
                    return $"ship #{ship.Id} is dead at tick {state.CurrentTick}.";
                }).ConfigureAwait(false);
            return;
        }

        if (TryReadCommandArgument(commandText, "await-position:", out value))
        {
            Vector target = ParseVector(value, "await-position");
            await WaitForCondition(
                options,
                state,
                delegate
                {
                    ClassicShipControllable ship = RequireShip(state);

                    if (!ship.Alive)
                        return false;

                    Vector delta = new Vector(target.X - ship.Position.X, target.Y - ship.Position.Y);
                    return delta < AwaitPositionRadius;
                },
                delegate
                {
                    ClassicShipControllable ship = RequireShip(state);
                    return
                        $"reached position ({target.X:0.###}, {target.Y:0.###}) within radius {AwaitPositionRadius:0.###} at tick {state.CurrentTick} in cluster {ship.Cluster.Name}, actual=({ship.Position.X:0.###}, {ship.Position.Y:0.###}).";
                }).ConfigureAwait(false);
            return;
        }

        if (TryReadCommandArgument(commandText, "await-tick:", out value))
        {
            uint tick = ParseUInt(value, "await-tick");
            await WaitForCondition(
                options,
                state,
                delegate { return state.CurrentTick >= tick; },
                delegate { return $"reached tick {state.CurrentTick}, awaited tick {tick}."; }).ConfigureAwait(false);
            return;
        }

        if (TryReadCommandArgument(commandText, "await-event:", out value))
        {
            EventKind kind = ParseEventKind(value);
            await WaitForEventKind(options, state, kind).ConfigureAwait(false);
            return;
        }

        throw new ArgumentException($"Unknown command {commandText}.");
    }

    private static async Task WaitForCondition(Options options, RuntimeState state, Func<bool> condition, Func<string> message)
    {
        if (condition())
        {
            Console.WriteLine($"AWAIT: {message()}");
            return;
        }

        while (!state.StopRequested)
        {
            FlattiverseEvent? @event = await ProcessNextEvent(options, state).ConfigureAwait(false);

            if (@event is null || state.StopRequested)
                return;

            if (condition())
            {
                Console.WriteLine($"AWAIT: {message()}");
                return;
            }
        }
    }

    private static async Task WaitForEventKind(Options options, RuntimeState state, EventKind awaitedKind)
    {
        while (!state.StopRequested)
        {
            FlattiverseEvent? @event = await ProcessNextEvent(options, state).ConfigureAwait(false);

            if (@event is null || state.StopRequested)
                return;

            if (@event.Kind != awaitedKind)
                continue;

            Console.WriteLine($"AWAIT: event {awaitedKind} matched at tick {state.CurrentTick}: {@event}");
            return;
        }
    }

    private static async Task<FlattiverseEvent?> ProcessNextEvent(Options options, RuntimeState state)
    {
        if (!state.Galaxy.Active)
        {
            state.StopRequested = true;
            Console.WriteLine("Galaxy connection became inactive. Exiting.");
            return null;
        }

        FlattiverseEvent @event = await state.Galaxy.NextEvent().ConfigureAwait(false);

        if (@event is GalaxyTickEvent tickEvent)
        {
            if (!state.HasStartTick)
            {
                state.HasStartTick = true;
                state.StartTick = tickEvent.Tick;
            }

            state.CurrentTick = tickEvent.Tick;
            await ApplyPolicies(state).ConfigureAwait(false);

            if (options.MaxTicks != 0 && tickEvent.Tick - state.StartTick >= options.MaxTicks)
            {
                state.StopRequested = true;
                Console.WriteLine($"Stopped after {tickEvent.Tick - state.StartTick} ticks at galaxy tick {tickEvent.Tick} because max tick limit {options.MaxTicks} was reached.");
            }
        }
        else
            Console.WriteLine(@event);

        if (@event.Kind == EventKind.ConnectionTerminated)
            state.StopRequested = true;

        if (state.Ship is not null && !state.Ship.Active)
        {
            state.StopRequested = true;
            Console.WriteLine($"Selected ship #{state.Ship.Id} named {state.Ship.Name} became inactive. Exiting.");
        }

        return @event;
    }

    private static async Task ApplyPolicies(RuntimeState state)
    {
        if (state.Ship is null || !state.Ship.Active)
            return;

        ClassicShipControllable ship = state.Ship;

        if (!ship.Alive)
        {
            if (state.Policies.AutoContinue && !state.Policies.ContinuePending)
            {
                await ship.Continue().ConfigureAwait(false);
                state.Policies.ContinuePending = true;
                Console.WriteLine($"POLICY: tick {state.CurrentTick} auto-continue requested for ship #{ship.Id}.");
            }

            return;
        }

        state.Policies.ContinuePending = false;

        if (state.Policies.HasScan)
        {
            if (!ship.MainScanner.Active ||
                MathF.Abs(ship.MainScanner.TargetWidth - state.Policies.ScanWidth) > 0.001f ||
                MathF.Abs(ship.MainScanner.TargetLength - state.Policies.ScanLength) > 0.001f ||
                MathF.Abs(ship.MainScanner.TargetAngle - state.Policies.ScanAngle) > 0.001f)
            {
                await ship.MainScanner.Set(state.Policies.ScanWidth, state.Policies.ScanLength, state.Policies.ScanAngle).ConfigureAwait(false);
                await ship.MainScanner.On().ConfigureAwait(false);
                Console.WriteLine(
                    $"POLICY: tick {state.CurrentTick} scanner set to width={state.Policies.ScanWidth:0.###}, length={state.Policies.ScanLength:0.###}, angle={state.Policies.ScanAngle:0.###}.");
            }
        }

        if (state.Policies.HasTarget && state.Policies.Target is not null)
        {
            Vector engineTarget = CalculateEngineTarget(
                ship.Position,
                ship.Movement,
                state.Policies.Target.X,
                state.Policies.Target.Y,
                ship.Engine.Maximum);

            if (ship.Engine.Target != engineTarget)
            {
                if (engineTarget < EngineDeadzone)
                {
                    await ship.Engine.Off().ConfigureAwait(false);
                    Console.WriteLine($"POLICY: tick {state.CurrentTick} engine turned off near target.");
                }
                else
                {
                    await ship.Engine.Set(engineTarget).ConfigureAwait(false);
                    Console.WriteLine(
                        $"POLICY: tick {state.CurrentTick} target=({state.Policies.Target.X:0.###}, {state.Policies.Target.Y:0.###}) engine=({engineTarget.X:0.###}, {engineTarget.Y:0.###}).");
                }
            }
        }

        if (state.Policies.StatusEvery == 0)
            return;

        if (state.CurrentTick == 0 || state.CurrentTick - state.LastStatusTick >= state.Policies.StatusEvery)
        {
            PrintStatus(state);
            state.LastStatusTick = state.CurrentTick;
        }
    }

    private static void PrintStatus(RuntimeState state)
    {
        if (state.Ship is null)
        {
            Console.WriteLine($"STATUS: tick={state.CurrentTick} no ship selected.");
            return;
        }

        ClassicShipControllable ship = state.Ship;
        string clusterName = ship.Active ? ship.Cluster.Name : "-";
        string targetPolicy = state.Policies.HasTarget && state.Policies.Target is not null
            ? $"({state.Policies.Target.X:0.###},{state.Policies.Target.Y:0.###})"
            : "-";
        string scanPolicy = state.Policies.HasScan
            ? $"({state.Policies.ScanWidth:0.###},{state.Policies.ScanLength:0.###},{state.Policies.ScanAngle:0.###})"
            : "-";
        string visibleUnits = "-";

        if (ship.Active)
        {
            StringBuilder builder = new StringBuilder();
            int visibleCount = 0;

            foreach (Unit unit in ship.Cluster.Units)
            {
                if (unit.Name == ship.Name)
                    continue;

                if (builder.Length != 0)
                    builder.Append(',');

                builder.Append(unit.Name);
                visibleCount++;
            }

            visibleUnits = $"{visibleCount}[{builder}]";
        }

        Console.WriteLine(
            $"STATUS: tick={state.CurrentTick} ship=#{ship.Id} cluster={clusterName} active={ship.Active} alive={ship.Alive} " +
            $"pos=({ship.Position.X:0.###},{ship.Position.Y:0.###}) mov=({ship.Movement.X:0.###},{ship.Movement.Y:0.###}) " +
            $"energy={ship.EnergyBattery.Current:0.###}/{ship.EnergyBattery.Maximum:0.###} use={ship.EnergyBattery.ConsumedThisTick:0.###} " +
            $"in={ship.EnergyCell.CollectedThisTick:0.###} engineCurrent=({ship.Engine.Current.X:0.###},{ship.Engine.Current.Y:0.###}) " +
            $"engineTarget=({ship.Engine.Target.X:0.###},{ship.Engine.Target.Y:0.###}) " +
            $"scanner={ship.MainScanner.Active} scannerStatus={ship.MainScanner.Status} scanCurrent=({ship.MainScanner.CurrentWidth:0.###},{ship.MainScanner.CurrentLength:0.###},{ship.MainScanner.CurrentAngle:0.###}) " +
            $"scanTarget=({ship.MainScanner.TargetWidth:0.###},{ship.MainScanner.TargetLength:0.###},{ship.MainScanner.TargetAngle:0.###}) " +
            $"policyTarget={targetPolicy} policyScan={scanPolicy} autoContinue={state.Policies.AutoContinue} visibleUnits={visibleUnits}.");
    }

    private static ClassicShipControllable RequireShip(RuntimeState state)
    {
        if (state.Ship is null)
            throw new InvalidOperationException("No ship is selected. Use create, attach-id, or attach-name first.");

        return state.Ship;
    }

    private static bool TryReadCommandArgument(string commandText, string prefix, out string value)
    {
        if (!commandText.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            value = string.Empty;
            return false;
        }

        value = commandText.Substring(prefix.Length);
        return true;
    }

    private static byte ParseByte(string text, string name)
    {
        if (!byte.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out byte value))
            throw new ArgumentException($"Could not parse {name} as byte.");

        return value;
    }

    private static uint ParseUInt(string text, string name)
    {
        if (!uint.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint value))
            throw new ArgumentException($"Could not parse {name} as uint.");

        return value;
    }

    private static float ParseFloat(string text, string name)
    {
        if (!float.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float value))
            throw new ArgumentException($"Could not parse {name} as float.");

        if (float.IsNaN(value) || float.IsInfinity(value))
            throw new ArgumentException($"{name} must be finite.");

        return value;
    }

    private static Vector ParseVector(string text, string name)
    {
        string[] parts = text.Split(new[] { 'x', 'X' }, StringSplitOptions.None);

        if (parts.Length != 2)
            throw new ArgumentException($"{name} must look like <x>x<y>.");

        return new Vector(ParseFloat(parts[0], $"{name}.x"), ParseFloat(parts[1], $"{name}.y"));
    }

    private static void ParseScan(string text, out float width, out float length, out float angle)
    {
        string[] parts = text.Split(new[] { 'x', 'X' }, StringSplitOptions.None);

        if (parts.Length != 3)
            throw new ArgumentException("scan must look like <width>x<length>x<angle>.");

        width = ParseFloat(parts[0], "scan.width");
        length = ParseFloat(parts[1], "scan.length");
        angle = ParseFloat(parts[2], "scan.angle");
    }

    private static void ParseShot(string text, out Vector relativeMovement, out ushort ticks, out float load, out float damage)
    {
        string[] parts = text.Split(',', StringSplitOptions.None);

        if (parts.Length != 4)
            throw new ArgumentException("shoot must look like <x>x<y>,<ticks>,<load>,<damage>.");

        relativeMovement = ParseVector(parts[0], "shoot.relativeMovement");

        if (!ushort.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out ticks))
            throw new ArgumentException("Could not parse shoot.ticks as ushort.");

        load = ParseFloat(parts[2], "shoot.load");
        damage = ParseFloat(parts[3], "shoot.damage");
    }

    private static EventKind ParseEventKind(string text)
    {
        if (!Enum.TryParse(text, true, out EventKind kind))
            throw new ArgumentException($"Unknown event kind {text}.");

        return kind;
    }

    private static Vector CalculateEngineTarget(Vector position, Vector movement, float targetX, float targetY, float maximum)
    {
        Vector targetDelta = new Vector(targetX - position.X, targetY - position.Y);

        if (targetDelta < PositionDeadzone && movement < MovementDeadzone)
            return new Vector();

        Vector engineTarget = targetDelta * PositionGain - movement * MovementGain;

        if (engineTarget > maximum)
            return engineTarget / engineTarget.Length * maximum;

        if (engineTarget < EngineDeadzone)
            return new Vector();

        return engineTarget;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  CliShip [--uri <uri>] [--auth <auth>] [--team <team>] [--name <ship-name>] <maxTicks> [<command> ...]");
        Console.WriteLine();
        Console.WriteLine("maxTicks:");
        Console.WriteLine("  0 means unlimited.");
        Console.WriteLine("  Positive values are counted relative to the first received galaxy tick.");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  create");
        Console.WriteLine("  attach-id:<id>");
        Console.WriteLine("  attach-name:<name>");
        Console.WriteLine("  continue");
        Console.WriteLine("  auto-continue:on|off");
        Console.WriteLine("  target:<x>x<y>");
        Console.WriteLine("  targetoff");
        Console.WriteLine("  scan:<width>x<length>x<angle>");
        Console.WriteLine("  scanoff");
        Console.WriteLine("  engine:<x>x<y>");
        Console.WriteLine("  engineoff");
        Console.WriteLine("  shoot:<x>x<y>,<ticks>,<load>,<damage>");
        Console.WriteLine("  status");
        Console.WriteLine("  status-every:<ticks>");
        Console.WriteLine("  await-alive");
        Console.WriteLine("  await-dead");
        Console.WriteLine("  await-position:<x>x<y>");
        Console.WriteLine("  await-tick:<tick>");
        Console.WriteLine("  await-event:<EventKind>");
        Console.WriteLine("  exit");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  CliShip 200 create auto-continue:on continue scan:90x300x0 target:200x100 status-every:10");
        Console.WriteLine("  CliShip 200 create continue await-dead continue status exit");
        Console.WriteLine();
        Console.WriteLine($"Defaults: uri={DefaultUri}, team={DefaultTeam}, name={DefaultShipName}, await-position-radius={AwaitPositionRadius:0.###}");
    }
}
