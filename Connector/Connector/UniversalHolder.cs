using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// A universal holder.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UniversalHolder<T> : IEnumerable<T> where T : UniversalEnumerable
    {
        private T[] values;

        internal UniversalHolder(T[] values)
        {
            this.values = values;
        }

        /// <summary>
        /// A list.
        /// </summary>
        public List<T> List
        {
            get
            {
                List<T> list = new List<T>();

                foreach (T value in values)
                    if (value != null)
                        list.Add(value);

                return list;
            }
        }

        /// <summary>
        /// A sorted list.
        /// </summary>
        public SortedList<string, T> SortedList
        {
            get
            {
                SortedList<string, T> list = new SortedList<string, T>();

                foreach (T value in values)
                    if (value != null)
                        list.Add(value.Name, value);

                return list;
            }
        }

        /// <summary>
        /// Returns an element of the holder.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <returns>The element specified by the index.</returns>
        public T this[string name]
        {
            get
            {
                foreach (T value in values)
                    if (value != null && value.Name == name)
                        return value;

                // throw new GameException(0x30);

                return default(T);
            }
        }

        /// <summary>
        /// Returns the enumerator of the holder.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new UniversalEnumerator<T>(values);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new UniversalEnumerator<T>(values);
        }

        internal void updateDatabasis(T[] values)
        {
            this.values = values;
        }
    }
}