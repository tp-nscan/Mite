using System.Collections.Generic;

namespace Mite.Device
{
    public struct d3
    {
        public d3(int x, int y=1, int z=1)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public int X;
        public int Y;
        public int Z;
    }

    public static class d3ext
    {
        public static IEnumerable<d3> Raster(this d3 bounds)
        {
            for (var iBx = 0; iBx < bounds.X; iBx++)
            {
                for (var iBy = 0; iBy < bounds.Y; iBy++)
                {
                    for (var iBz = 0; iBz < bounds.Z; iBz++)
                    {
                        yield return new d3(x:iBx, y:iBy, z:iBz);
                    }
                }

            }
        }

        public static int Volume(this d3 what)
        {
            return what.X * what.Y * what.Z;
        }
    }
} 