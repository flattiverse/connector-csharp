using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Represents an indexed list which can automatically defragment itself.
    /// </summary>
    /// <typeparam name="T">The type the list shall contain.</typeparam>
    class IndexList<T> : IEnumerable<T> where T : class
    {
        private int max = 0;
        private int air = 0;

        private T?[] values;

        private object sync = new object();

        public IndexList(int size)
        {
            values = new T[size];
        }

        public int Insert(T value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "value can't be null.");

            lock (sync)
            {
                if (air != 0)
                    for (int position = 0; position < max; position++)
                        if (values[position] == null)
                        {
                            values[position] = value;
                            air--;
                            return position;
                        }

                if (max == values.Length)
                    return -1;

                values[max] = value;
                max++;
                return max - 1;
            }
        }

        public T? this[int index] => values[index];

        public void Wipe(int index)
        {
            lock (sync)
            {
                values[index] = null;

                if (max - 1 == index)
                    max--;
                else
                    air++;
            }
        }

        public int Air => air;

        public int Count => max - air;

        private IEnumerator<T> enumerate()
        {
            T? value;

            for (int position = 0; position < max; position++)
            {
                value = values[position];

                if (value != null)
                    yield return value;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return enumerate();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return enumerate();
        }
    }
}
