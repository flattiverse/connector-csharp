using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Represents a team.
/// </summary>
public class Team : INamedUnit
{
    /// <summary>
    /// The id of the team.
    /// </summary>
    public readonly byte Id;

    private string _name;
    
    private byte _red;
    private byte _green;
    private byte _blue;
    
    private bool _active;

    public readonly Galaxy Galaxy;

    internal Team(Galaxy galaxy, byte id, string name, byte red, byte green, byte blue)
    {
        Galaxy = galaxy;
        Id = id;
        
        _name = name;

        _red = red;
        _green = green;
        _blue = blue;

        _active = true;
    }

    public async Task Chat(string message)
    {
        PacketWriter writer = new PacketWriter(new byte[2052]);

        writer.Command = 0xC5;
        
        writer.Write(Id);
        writer.Write(message);
        
        await Galaxy.Connection.SendSessionRequestAndGetReply(writer);
    }
    
    internal void Update(string name, byte red, byte green, byte blue)
    {
        _name = name;

        _red = red;
        _green = green;
        _blue = blue;
    }
    
    internal void Deactivate()
    {
        _active = false;
    }
    
    /// <summary>
    /// The name of the team.
    /// </summary>
    public string Name => _name;
    
    /// <summary>
    /// The red part of the team color.
    /// </summary>
    public byte Red => _red;
    
    /// <summary>
    /// The green part of the team color.
    /// </summary>
    public byte Green => _green;
    
    /// <summary>
    /// The blue part of the team color.
    /// </summary>
    public byte Blue => _blue;
    
    /// <summary>
    /// True as long as the team is active.
    /// </summary>
    public bool Active => _active;
}