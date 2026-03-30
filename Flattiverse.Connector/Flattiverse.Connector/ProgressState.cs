namespace Flattiverse.Connector;

/// <summary>
/// Pollable progress state for chunked connector operations.
/// </summary>
public sealed class ProgressState
{
    private long _current;
    private long _total;
    private long _updates;
    private bool _finished;

    /// <summary>
    /// Current amount already transferred or decoded by the ongoing chunked operation.
    /// </summary>
    public long Current
    {
        get { return _current; }
    }

    /// <summary>
    /// Total amount reported by the server for the current chunked operation.
    /// </summary>
    public long Total
    {
        get { return _total; }
    }

    /// <summary>
    /// Number of progress reports that have been applied to this instance.
    /// </summary>
    public long Updates
    {
        get { return _updates; }
    }

    /// <summary>
    /// Whether the connector has observed the final chunk of the operation.
    /// </summary>
    public bool Finished
    {
        get { return _finished; }
    }

    /// <summary>
    /// Progress normalized to the range <c>[0; 1]</c>.
    /// Returns <c>0</c> while no total is known yet and <c>1</c> once a zero-length operation finished.
    /// </summary>
    public double Progress01
    {
        get
        {
            if (_total == 0)
                return _finished ? 1.0 : 0.0;

            return (double)_current / _total;
        }
    }

    internal void Reset()
    {
        _current = 0;
        _total = 0;
        _updates = 0;
        _finished = false;
    }

    internal void Report(long current, long total)
    {
        _current = current;
        _total = total;
        _updates++;
        _finished = current == total;
    }
}
