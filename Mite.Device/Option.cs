using System.Collections.Generic;

namespace Mite.Device
{
    public class Option<T> : IEnumerable<T>
    {
        private readonly T[] _data;

        private Option(T[] data)
        {
            _data = data;
        }

        public static Option<T> Create(T value)
        {
            return new Option<T>(new T[] { value });
        }

        public static Option<T> CreateEmpty()
        {
            return new Option<T>(new T[0]);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_data).GetEnumerator();
        }

        System.Collections.IEnumerator
            System.Collections.IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
    }
}
