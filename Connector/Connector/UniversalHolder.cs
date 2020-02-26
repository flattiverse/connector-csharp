using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// A universal holder.
    /// </summary>
    /// <typeparam name="T">The type to hold.</typeparam>
    public class UniversalHolder<T> : IEnumerable<T> where T : class, UniversalEnumerable
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
            //[return: MaybeNull]
            get
            {
                foreach (T value in values)
                    if (value != null && value.Name == name)
                        return value;

                // throw new GameException(0x30);

                return default(T);
            }
        }

        private IEnumerator<T> enumerate()
        {
            T value;

            for (int position = 0; position < values.Length; position++)
            {
                value = values[position];

                if (value != null)
                    yield return value;
            }
        }

        /// <summary>
        /// Returns the enumerator of the holder.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return enumerate();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return enumerate();
        }

        internal void updateDatabasis(T[] values)
        {
            this.values = values;
        }
    }
}