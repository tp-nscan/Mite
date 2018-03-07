using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT;

namespace Mite.Common
{
    public static class BText
    {
        public static float Width(this R<float> r)
        {
            return r.MaxX - r.MinX;
        }

        public static float Width<T>(this RV<float,T> r)
        {
            return r.MaxX - r.MinX;
        }

        public static float Height(this R<float> r)
        {
            return r.MaxY - r.MinY;
        }

        public static float Height<T>(this RV<float, T> r)
        {
            return r.MaxY - r.MinY;
        }

    }
}
