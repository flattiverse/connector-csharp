using System.Diagnostics.CodeAnalysis;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// A generic collection type for NamedUnits.
/// </summary>
/// <typeparam name="T"></typeparam>
public class UniversalHolder<T> : IEnumerable<T> where T: class, INamedUnit
{
    private readonly T?[] _data;
    
    internal UniversalHolder(T?[] data)
    {
        _data = data;
    }

    /// <summary>
    /// Gathers the object on the specified index.
    /// </summary>
    /// <param name="index">The index to look up.</param>
    /// <exception cref="SpecifiedElementNotFoundGameException">Thrown, if the element doesn't exist.</exception>
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
    /// Gathers the object with the specified name.
    /// </summary>
    /// <param name="name">The name of the object.</param>
    /// <exception cref="SpecifiedElementNotFoundGameException">Thrown, if the object hasn't been found.</exception>
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
    /// Tries to gather the object with the corresponding name.
    /// </summary>
    /// <param name="name">The name of the object.</param>
    /// <param name="element">The found element or null, if not found.</param>
    /// <returns>true, if the element has been found, false otherwise.</returns>
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
    /// Tries to gather the object on the specified index.
    /// </summary>
    /// <param name="index">The index to look up.</param>
    /// <param name="element">The found element or null, if not found.</param>
    /// <returns>true, if the element has been found, false otherwise.</returns>
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
    /// Counts the found elements.
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