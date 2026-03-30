namespace Flattiverse.Connector.Units;

/// <summary>
/// One orbit segment around the configured center or the preceding orbit segment.
/// </summary>
public class Orbit
{
    private float _distance;
    private float _startAngle;
    private int _rotationTicks;

    internal Orbit(float distance, float startAngle, int rotationTicks)
    {
        _distance = distance;
        _startAngle = startAngle;
        _rotationTicks = rotationTicks;
    }

    /// <summary>
    /// Distance from the current orbit center to the orbiting body.
    /// </summary>
    public float Distance
    {
        get { return _distance; }
    }

    /// <summary>
    /// Angle at galaxy tick 0 in degrees.
    /// </summary>
    public float StartAngle
    {
        get { return _startAngle; }
    }

    /// <summary>
    /// Full rotation period in ticks. Negative values rotate in the opposite direction.
    /// </summary>
    public int RotationTicks
    {
        get { return _rotationTicks; }
    }

    /// <summary>
    /// Calculates the offset contributed by this orbit segment at the given galaxy tick.
    /// The caller applies this offset relative to the current orbit center in the chain.
    /// </summary>
    public Vector CalculateOffset(uint tick)
    {
        uint interval = (uint)Math.Abs((long)_rotationTicks);
        uint phaseTick = tick % interval;
        float angle = (_startAngle + 360f * phaseTick / _rotationTicks) % 360f;

        if (angle < 0f)
            angle += 360f;

        return Vector.FromAngleLength(angle, _distance);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"(Distance={_distance:0.###}, StartAngle={_startAngle:0.###}, RotationTicks={_rotationTicks})";
    }
}
