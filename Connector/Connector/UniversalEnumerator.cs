using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    class UniversalEnumerator<T> : IEnumerator<T> where T : UniversalEnumerable
    {
        private T[] values;
        private T value;
        private int position = -1;

        internal UniversalEnumerator(T[] values)
        {
            this.values = values;
        }

        public T Current
        {
            get
            {
                return value;
            }
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return value;
            }
        }

        public bool MoveNext()
        {
            position++;

            while (position < values.Length)
            {
                value = values[position];

                if (value != null)
                    return true;

                position++;
            }

            return false;
        }

        public void Reset()
        {
            position = -1;
        }
    }
}
