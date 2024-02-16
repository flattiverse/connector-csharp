using System.Diagnostics.CodeAnalysis;

namespace Flattiverse.Connector;

public class UniversalHolder<T> : IEnumerable<T> where T: class, INamedUnit
{
    private readonly T?[] data;
    
    internal UniversalHolder(T?[] data)
    {
        this.data = data;
    }

    /// <summary>
    /// Gathers the object on the specified index.
    /// </summary>
    /// <param name="index">The index to look up.</param>
    /// <exception cref="GameException">Thrown, if the element doesn't exist.</exception>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= data.Length)
                throw new GameException(0x30, $"The requested element on index {index} doesn't exist.");
            
            T? tData = data[index];

            if (tData is null)
                throw new GameException(0x30, $"The requested element on index {index} doesn't exist.");

            return tData;
        }
    }
    
    /// <summary>
    /// Gathers the object with the specified name.
    /// </summary>
    /// <param name="name">The name of the object.</param>
    /// <exception cref="GameException">Thrown, if the object hasn't been found.</exception>
    public T this[string name]
    {
        get
        {
            foreach (T? t in data)
                if (t is not null && t.Name == name)
                    return t;

            throw new GameException(0x30, $"The requested element with name \"{name}\" doesn't exist.");
        }
    }
    
    /// <summary>
    /// Tries to gather the object with the corresponding name.
    /// </summary>
    /// <param name="name">The name of the object.</param>
    /// <param name="element">The found element or null, if not found.</param>
    /// <returns>true, if the element has been found, false otherwise.</returns>
    public bool TryGet(string name, [NotNullWhen(returnValue: true)] out T? element)
    {
        foreach (T? t in data)
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
        if (index < 0 || index >= data.Length)
        {
            element = null;
            return false;
        }
        
        T? tData = data[index];
        
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
            
            foreach (T? t in data)
                if (t is not null)
                    count++;

            return count;
        }
    }
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public IEnumerator<T> GetEnumerator()
    {
        foreach (T? t in data)
            if (t is not null)
                yield return t;
    }
}