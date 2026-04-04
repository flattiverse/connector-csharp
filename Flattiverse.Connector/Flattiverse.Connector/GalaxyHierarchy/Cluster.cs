using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml.Linq;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// This is a subset of the galaxy. Each cluster is a map.
/// </summary>
public class Cluster : INamedUnit
{
    /// <summary>
    /// The id within the galaxy of the cluster.
    /// </summary>
    public readonly byte Id;

    /// <summary>
    /// The galaxy this cluster is in.
    /// </summary>
    public readonly Galaxy Galaxy;
    
    private string _name;
    private bool _start;
    private bool _respawn;
    private bool _active;
    
    private Dictionary<string, Unit> _units;

    internal Cluster(Galaxy galaxy, byte id, string name, bool start, bool respawn)
    {
        Galaxy = galaxy;
        Id = id;
        _name = name;
        _start = start;
        _respawn = respawn;
        _active = true;
        
        _units = new Dictionary<string, Unit>();
    }

    internal void Update(string name, bool start, bool respawn)
    {
        _name = name;
        _start = start;
        _respawn = respawn;
        _active = true;
    }
    
    internal void Deactivate()
    {
        _active = false;
    }

    internal bool UpdateUnit(string name, PacketReader reader, [NotNullWhen(true)] out Unit? unit)
    {
        if (!_units.TryGetValue(name, out unit))
            return false;

        unit.UpdateMovement(reader);

        return true;
    }

    internal bool UpdateUnitState(string name, PacketReader reader, [NotNullWhen(true)] out Unit? unit)
    {
        if (!_units.TryGetValue(name, out unit))
            return false;

        unit.UpdateState(reader);

        return true;
    }
    
    internal void AddUnit(Unit unit)
    {
        _units.Add(unit.Name, unit);
    }

    internal bool RemoveUnit(string name, [NotNullWhen(true)] out Unit? unit)
    {
        return _units.Remove(name, out unit);
    }

    internal bool GetUnit([NotNullWhen(true)] out Unit? unit)
    {
        return _units.TryGetValue(_name, out unit);
    }
    
    /// <summary>
    /// The name of the Cluster.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// If true, freshly registered ships spawn in this cluster.
    /// </summary>
    public bool Start => _start;

    /// <summary>
    /// If true, Continue() spawns in this cluster.
    /// </summary>
    public bool Respawn => _respawn;

    /// <summary>
    /// Creates or updates a region within this cluster.
    /// </summary>
    /// <param name="xml">
    /// Region XML with a Region root node. Example:
    /// <code>
    /// &lt;Region Id="66" Name="Spawn A" Left="-150" Top="-300" Right="150" Bottom="0"&gt;
    ///   &lt;Team Id="0" /&gt;
    /// &lt;/Region&gt;
    /// </code>
    /// Team child nodes are accepted.
    /// </param>
    /// <exception cref="InvalidArgumentGameException">Thrown, if <paramref name="xml" /> is empty or invalid at protocol level.</exception>
    /// <exception cref="InvalidXmlNodeValueGameException">Thrown, if the region XML is semantically invalid.</exception>
    public async Task SetRegion(string xml)
    {
        if (string.IsNullOrEmpty(xml))
            throw new InvalidArgumentGameException(InvalidArgumentKind.AmbiguousXmlData, "xml");

        await Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x24;
            writer.Write(Id);
            writer.Write(xml);
        });
    }

    /// <summary>
    /// Removes a region by id from this cluster.
    /// </summary>
    /// <param name="id">Region id in range 0..255.</param>
    /// <exception cref="InvalidArgumentGameException">Thrown, if <paramref name="id" /> is out of range or unknown.</exception>
    public async Task RemoveRegion(int id)
    {
        if (id < 0 || id > byte.MaxValue)
            throw new InvalidArgumentGameException(id < 0 ? InvalidArgumentKind.TooSmall : InvalidArgumentKind.TooLarge, "id");

        await Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x25;
            writer.Write(Id);
            writer.Write((byte)id);
        });
    }

    /// <summary>
    /// Queries all regions of this cluster as XML.
    /// </summary>
    /// <returns>
    /// XML with a Regions root node and Region child nodes.
    /// </returns>
    public async Task<string> QueryRegions()
    {
        PacketReaderLarge reply = await Galaxy.Connection.SendSessionRequestAndGetReplyLarge(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x26;
            writer.Write(Id);
        });

        if (!reply.Read(out ushort regionCount))
            throw new InvalidDataException("This shouldn't have happened: Couldn't read region count in region query response.");

        XElement regionsElement = new XElement("Regions");

        for (int regionIndex = 0; regionIndex < regionCount; regionIndex++)
        {
            if (!reply.Read(out byte regionId) ||
                !reply.Read(out string? regionName) ||
                !reply.Read(out float left) ||
                !reply.Read(out float top) ||
                !reply.Read(out float right) ||
                !reply.Read(out float bottom) ||
                !reply.Read(out uint startLocationTeams))
                throw new InvalidDataException("This shouldn't have happened: Couldn't read complete region query response data.");

            XElement regionElement = new XElement("Region",
                new XAttribute("Id", regionId),
                new XAttribute("Left", left.ToString("R", CultureInfo.InvariantCulture)),
                new XAttribute("Top", top.ToString("R", CultureInfo.InvariantCulture)),
                new XAttribute("Right", right.ToString("R", CultureInfo.InvariantCulture)),
                new XAttribute("Bottom", bottom.ToString("R", CultureInfo.InvariantCulture)));

            if (!string.IsNullOrEmpty(regionName))
                regionElement.SetAttributeValue("Name", regionName);

            for (byte teamId = 0; teamId < 32; teamId++)
            {
                uint teamMask = 1u << teamId;

                if ((startLocationTeams & teamMask) != 0 && teamId != 12 && Galaxy.Teams.TryGet(teamId, out Team? _))
                    regionElement.Add(new XElement("Team", new XAttribute("Id", teamId)));
            }

            regionsElement.Add(regionElement);
        }

        return new XDocument(regionsElement).ToString(SaveOptions.DisableFormatting);
    }

    /// <summary>
    /// Queries all editable units of this cluster, including currently invisible ones like inactive power-ups.
    /// </summary>
    public async Task<EditableUnitSummary[]> QueryEditableUnits(ProgressState? progressState = null)
    {
        return await ChunkedTransfer.DownloadItems(Galaxy.Connection, delegate (ref PacketWriter writer, int offset, ushort maximumCount)
        {
            writer.Command = 0x27;
            writer.Write(Id);
            writer.Write(offset);
            writer.Write(maximumCount);
        }, delegate (ref PacketReaderLarge reader, out EditableUnitSummary unit)
        {
            if (!reader.Read(out byte kindValue) ||
                !reader.Read(out string? name) ||
                name is null)
            {
                unit = null!;
                return false;
            }

            unit = new EditableUnitSummary(name, (UnitKind)kindValue);
            return true;
        }, progressState, ChunkedTransfer.EditableUnitChunkMaximumCount, "editable unit query result").ConfigureAwait(false);
    }

    /// <summary>
    /// Creates or updates a single editable map unit in this cluster.
    /// Root node must be the unit type itself, for example &lt;Sun ... /&gt;.
    /// For &lt;Buoy ... /&gt; an optional Message attribute is supported (max 384 characters).
    /// For &lt;MissionTarget ...&gt; Team is required and child nodes &lt;Vector X="..." Y="..." /&gt; are supported.
    /// </summary>
    /// <param name="xml">Unit XML payload.</param>
    /// <exception cref="InvalidArgumentGameException">Thrown, if <paramref name="xml" /> is empty or malformed on protocol level.</exception>
    /// <exception cref="InvalidXmlNodeValueGameException">Thrown, if the unit XML is semantically invalid.</exception>
    public async Task SetUnit(string xml)
    {
        if (string.IsNullOrEmpty(xml))
            throw new InvalidArgumentGameException(InvalidArgumentKind.AmbiguousXmlData, "xml");

        await Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x28;
            writer.Write(Id);
            writer.Write(xml);
        });
    }

    /// <summary>
    /// Removes a single editable map unit by name.
    /// </summary>
    /// <param name="name">Unit name.</param>
    public async Task RemoveUnit(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooSmall, "name");

        await Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x29;
            writer.Write(Id);
            writer.Write(name);
        });
    }

    /// <summary>
    /// Queries the XML of one specific editable map unit by name.
    /// </summary>
    /// <param name="name">Unit name.</param>
    /// <returns>XML representation of the requested unit.</returns>
    public async Task<string> QueryUnitXml(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooSmall, "name");

        PacketReaderLarge reply = await Galaxy.Connection.SendSessionRequestAndGetReplyLarge(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x2A;
            writer.Write(Id);
            writer.Write(name);
        });

        if (!reply.Read(out string? xml))
            throw new InvalidDataException("This shouldn't have happened: Couldn't read queried unit XML.");

        return xml;
    }

    /// <summary>
    /// Admin-only helper that force-sets one player-unit subsystem tier inside this cluster.
    /// Intended for debugging and balancing tests.
    /// </summary>
    /// <param name="unitName">Player-unit name.</param>
    /// <param name="slot">Concrete subsystem slot.</param>
    /// <param name="tier">Target tier.</param>
    public async Task DebugSetPlayerUnitSubsystemTier(string unitName, SubsystemSlot slot, byte tier)
    {
        if (string.IsNullOrEmpty(unitName))
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooSmall, "unitName");

        await Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x2B;
            writer.Write(Id);
            writer.Write(unitName);
            writer.Write((byte)slot);
            writer.Write(tier);
        });
    }

    /// <summary>
    /// Admin-only helper that force-sets the current shield value of one player-unit inside this cluster.
    /// Intended for debugging and balancing tests.
    /// </summary>
    /// <param name="unitName">Player-unit name.</param>
    /// <param name="current">Target shield value.</param>
    public async Task DebugSetPlayerUnitShieldCurrent(string unitName, float current)
    {
        if (string.IsNullOrEmpty(unitName))
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooSmall, "unitName");

        await Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x2C;
            writer.Write(Id);
            writer.Write(unitName);
            writer.Write(current);
        });
    }

    /// <summary>
    /// Admin-only helper that force-sets the current hull value of one player-unit inside this cluster.
    /// Intended for debugging and balancing tests.
    /// </summary>
    /// <param name="unitName">Player-unit name.</param>
    /// <param name="current">Target hull value.</param>
    public async Task DebugSetPlayerUnitHullCurrent(string unitName, float current)
    {
        if (string.IsNullOrEmpty(unitName))
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooSmall, "unitName");

        await Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x2D;
            writer.Write(Id);
            writer.Write(unitName);
            writer.Write(current);
        });
    }

    /// <summary>
    /// Admin-only helper that force-sets the position of one player-unit inside this cluster and clears its movement.
    /// Intended for debugging and balancing tests.
    /// </summary>
    /// <param name="unitName">Player-unit name.</param>
    /// <param name="position">Target position.</param>
    public async Task DebugSetPlayerUnitPosition(string unitName, Vector position)
    {
        if (string.IsNullOrEmpty(unitName))
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooSmall, "unitName");

        await Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x2E;
            writer.Write(Id);
            writer.Write(unitName);
            position.Write(ref writer);
        });
    }

    /// <summary>
    /// Admin-only helper that clears the non-metal cargo contents of one player-unit inside this cluster.
    /// Intended for debugging and balancing tests.
    /// </summary>
    /// <param name="unitName">Target player-unit name.</param>
    public async Task DebugClearPlayerUnitNonMetalCargo(string unitName)
    {
        if (string.IsNullOrEmpty(unitName))
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooSmall, "unitName");

        await Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x2F;
            writer.Write(Id);
            writer.Write(unitName);
        });
    }

    /// <summary>
    /// Admin-only helper that force-sets one battery current value of one player-unit inside this cluster.
    /// Intended for debugging and balancing tests.
    /// </summary>
    /// <param name="unitName">Player-unit name.</param>
    /// <param name="slot">Battery slot to modify.</param>
    /// <param name="current">Target battery value.</param>
    public async Task DebugSetPlayerUnitBatteryCurrent(string unitName, SubsystemSlot slot, float current)
    {
        if (string.IsNullOrEmpty(unitName))
            throw new InvalidArgumentGameException(InvalidArgumentKind.TooSmall, "unitName");

        await Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x30;
            writer.Write(Id);
            writer.Write(unitName);
            writer.Write((byte)slot);
            writer.Write(current);
        });
    }

    /// <summary>
    /// If false, you have been disconnected or the cluster has been removed and therefore disabled.
    /// </summary>
    public bool Active => _active;

    /// <summary>
    /// Units currently known within this cluster from the connector's point of view.
    /// </summary>
    public IEnumerable<Unit> Units
    {
        get { return _units.Values; }
    }
}
