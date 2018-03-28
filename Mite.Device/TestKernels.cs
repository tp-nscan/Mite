using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mite.Device
{
    public static class TestKernels
    {
        public static Action<d3, d3, d3, d3> IntKernelWrap(GpuArrayWrite<int> gpuArrayWrite,
                                d3 gridDim, d3 blockDim)
        {
            return (d3 gd, d3 bd, d3 blockId, d3 threadId) =>
                IntKernel(gpuArrayWrite, gd, bd, blockId, threadId);
        }

        public static void IntKernel(GpuArrayWrite<int> output, d3 gridDim, 
                                             d3 blockDim, d3 blockId, d3 threadId)
        {
            output.AddGpuData(1, blockId.X * blockDim.X + threadId.X, blockId, threadId);
        }

        public static Action<d3, d3, d3, d3> FloatKernelWrap(GpuArrayWrite<float> gpuArrayWrite,
            d3 gridDim, d3 blockDim)
        {
            return (d3 gd, d3 bd, d3 blockId, d3 threadId) =>
                FloatKernel(gpuArrayWrite, gd, bd, blockId, threadId);
        }

        public static void FloatKernel(GpuArrayWrite<float> output, d3 gridDim,
            d3 blockDim, d3 blockId, d3 threadId)
        {
            var index = blockId.d3Dex(gridDim) * blockDim.Volume() + threadId.d3Dex(blockDim);
            var totalVolume = blockDim.Volume() * gridDim.Volume();

            output.AddGpuData(
                val: (2*index - totalVolume) /(float)totalVolume, 
                index: index, block: blockId, thread: threadId);
        }

    }

    public static class GBext
    {
        public static int d3Dex(this d3 d3Id, d3 d3Dim)
        {
            return d3Id.X + d3Id.Y * d3Dim.X + d3Id.Z * d3Dim.X * d3Dim.Y;
        }
    }
}
