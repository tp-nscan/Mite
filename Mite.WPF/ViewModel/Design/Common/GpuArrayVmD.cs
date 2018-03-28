using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mite.Device;
using Mite.WPF.ViewModel.Common;
using TT;

namespace Mite.WPF.ViewModel.Design.Common
{
    public class GpuArrayVmD : GpuArrayVm
    {
        public GpuArrayVmD() : 
            base(GpuArrayWrite, Lb)
        {
        }

        static R<int> Lb => new R<int>(0,32,0,32);

        public static GpuArrayWrite<float> GpuArrayWrite
        {
            get
            {
                var blockCount = new d3(4,4,2);
                var threadCount = new d3(4,4,2);
                var arrayOut = new GpuArrayWrite<float>(blockCount.Volume() * threadCount.Volume());
                GpuDevice.RunGpuKernel(
                    blockCount,
                    threadCount,
                    TestKernels.FloatKernelWrap(arrayOut, blockCount, threadCount));

                return arrayOut;
            }
        }
    }
}
