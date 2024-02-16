namespace Flattiverse.Connector;

public class UniversalHolder<T> : IEnumerable<T> where T: NamedUnit
{
    private readonly T?[] data;
    
    internal UniversalHolder(T?[] data)
    {
        this.data = data;
    }

    /// <summary>
    /// Gathers the object on 
    /// </summary>
    /// <param name="index"></param>
    /// <exception cref="GameException"></exception>
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