using System;
using System.Collections.Generic;
using System.Linq;
using TT;

namespace Mite.Common
{
    public static class EnumerExt
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var pair in list)
            {
                action.Invoke(pair);
            }
        }

        public static bool AreEqual<T>(
            this IEnumerable<T> lhs,
            IEnumerable<T> rhs,
            Func<T, T, bool> comp
            )
        {
            var lstLeft = lhs.ToArray();
            var lstRight = rhs.ToArray();

            if (lstLeft.Length != lstRight.Length)
            {
                return false;
            }
            return Enumerable.Range(0, lstLeft.Length)
                    .All(i => comp(lstLeft[i], lstRight[i]));
        }

        public static string ToCsvString(this IEnumerable<float> floats, string format)
        {
            return floats.Aggregate("", (s, f) => s + "\t" + f.ToString(format));
        }

        public static IEnumerable<P2<int>> Raster(this Sz2<int> bounds)
        {
            for (var x = 0; x < bounds.X; x++)
            {
                for (var y = 0; y < bounds.Y; y++)
                {
                    yield return new P2<int>(x, y);
                }
            }
        }
    }
}
