using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Static scanner subsystem of a modern ship.
/// </summary>
public class StaticScannerSubsystem : DynamicScannerSubsystem
{
    internal StaticScannerSubsystem(Controllable controllable, string name, bool exists, float maximumWidth, float maximumLength,
        float widthSpeed, float lengthSpeed, float angleSpeed, SubsystemSlot slot) :
        base(controllable, name, 0, exists, maximumWidth, maximumLength, widthSpeed, lengthSpeed, angleSpeed, slot)
    {
    }

    internal StaticScannerSubsystem(Controllable controllable, string name, PacketReader reader, SubsystemSlot slot) :
        base(controllable, name, 0, reader, slot)
    {
    }

    public float MaximumAngleOffset
    {
        get { return Exists ? ModernShipGeometry.ScannerMaximumAngleOffset : 0f; }
    }

    public float CurrentAngleOffset
    {
        get
        {
            if (!Exists || !ModernShipGeometry.TryGetLocalAngle(Slot, out float localAngle))
                return 0f;

            return ModernShipGeometry.NormalizeSignedAngle(CurrentAngle - Controllable.Angle - localAngle);
        }
    }

    public float TargetAngleOffset
    {
        get
        {
            if (!Exists || !ModernShipGeometry.TryGetLocalAngle(Slot, out float localAngle))
                return 0f;

            return ModernShipGeometry.NormalizeSignedAngle(TargetAngle - Controllable.Angle - localAngle);
        }
    }

    public new async Task Set(float width, float length, float angleOffset)
    {
        if (!Controllable.Active || !Exists)
            throw new SpecifiedElementNotFoundGameException();

        if (!Controllable.Alive)
            throw new YouNeedToContinueFirstGameException();

        InvalidArgumentKind widthValidity = RangeTolerance.ClampRange(width, MinimumWidth, MaximumWidth, out width);

        if (widthValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(widthValidity, "width");

        InvalidArgumentKind lengthValidity = RangeTolerance.ClampRange(length, MinimumLength, MaximumLength, out length);

        if (lengthValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(lengthValidity, "length");

        InvalidArgumentKind angleValidity = RangeTolerance.ClampRange(angleOffset, -ModernShipGeometry.ScannerMaximumAngleOffset,
            ModernShipGeometry.ScannerMaximumAngleOffset, out angleOffset);

        if (angleValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(angleValidity, "angleOffset");

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0xA2;
            writer.Write(Controllable.Id);
            writer.Write((byte)Slot);
            writer.Write(width);
            writer.Write(length);
            writer.Write(angleOffset);
        });
    }

    public new async Task On()
    {
        if (!Controllable.Active || !Exists)
            throw new SpecifiedElementNotFoundGameException();

        if (!Controllable.Alive)
            throw new YouNeedToContinueFirstGameException();

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0xA3;
            writer.Write(Controllable.Id);
            writer.Write((byte)Slot);
        });
    }

    public new async Task Off()
    {
        if (!Controllable.Active || !Exists)
            throw new SpecifiedElementNotFoundGameException();

        if (!Controllable.Alive)
            throw new YouNeedToContinueFirstGameException();

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0xA4;
            writer.Write(Controllable.Id);
            writer.Write((byte)Slot);
        });
    }

    internal new FlattiverseEvent? CreateRuntimeEvent()
    {
        return base.CreateRuntimeEvent();
    }
}
