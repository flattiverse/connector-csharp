using System.Diagnostics.CodeAnalysis;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Sparse read-only lookup wrapper for named connector objects such as teams, clusters, players, or controllables.
/// </summary>
/// <typeparam name="T">Concrete connector object type stored in this holder.</typeparam>
public class UniversalHolder<T> : IEnumerable<T> where T: class, INamedUnit
{
    private readonly T?[] _data;
    
    internal UniversalHolder(T?[] data)
    {
        _data = data;
    }

    /// <summary>
    /// Gets the existing element at the specified protocol index.
    /// </summary>
    /// <param name="index">The index to look up.</param>
    /// <exception cref="SpecifiedElementNotFoundGameException">Thrown, if no active element exists at that index.</exception>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _data.Length)
                throw new SpecifiedElementNotFoundGameException();
            
            T? tData = _data[index];

            if (tData is null)
                throw new SpecifiedElementNotFoundGameException();

            return tData;
        }
    }
    
    /// <summary>
    /// Gets the existing element with the specified name.
    /// </summary>
    /// <param name="name">The name of the object.</param>
    /// <exception cref="SpecifiedElementNotFoundGameException">Thrown, if no active element with that name exists.</exception>
    public T this[string name]
    {
        get
        {
            foreach (T? t in _data)
                if (t is not null && t.Name == name)
                    return t;

            throw new SpecifiedElementNotFoundGameException();
        }
    }
    
    /// <summary>
    /// Tries to find an active element with the specified name.
    /// </summary>
    /// <param name="name">The name of the object.</param>
    /// <param name="element">The found element, or <see langword="null" /> if none exists.</param>
    /// <returns><see langword="true" /> if an element with that name exists; otherwise <see langword="false" />.</returns>
    public bool TryGet(string name, [NotNullWhen(true)] out T? element)
    {
        foreach (T? t in _data)
            if (t is not null && t.Name == name)
            {
                element = t;
                return true;
            }
        
        element = null;
        return false;
    }

    /// <summary>
    /// Tries to find an active element at the specified protocol index.
    /// </summary>
    /// <param name="index">The index to look up.</param>
    /// <param name="element">The found element, or <see langword="null" /> if none exists.</param>
    /// <returns><see langword="true" /> if an active element exists at that index; otherwise <see langword="false" />.</returns>
    public bool TryGet(int index, [NotNullWhen(returnValue: true)] out T? element)
    {
        if (index < 0 || index >= _data.Length)
        {
            element = null;
            return false;
        }
        
        T? tData = _data[index];
        
        if (tData is null)
        {
            element = null;
            return false;
        }
        
        element = tData;
        return true;
    }

    /// <summary>
    /// Counts currently active elements in the holder.
    /// </summary>
    public int Count
    {
        get
        {
            int count = 0;
            
            foreach (T? t in _data)
                if (t is not null)
                    count++;

            return count;
        }
    }
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        foreach (T? t in _data)
            if (t is not null)
                yield return t;
    }
}
