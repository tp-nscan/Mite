using System;

namespace Mite.Device
{
    public static class GpuDevice
    {
        public static void RunGpuKernel(d3 blocks, d3 threads, Action<d3, d3, d3, d3> kernel)
        {
            foreach (var blockId in blocks.Raster())
            {
                foreach (var threadId in threads.Raster())
                {
                    kernel(blocks, threads, blockId, threadId);
                }
            }
        }

    }
}
