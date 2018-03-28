using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mite.Device;

namespace Mite.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAddGpuData()
        {
            var ta = new GpuArrayWrite<float>(20);
            ta.AddGpuData(val:2.2f, index:4, block: new d3(44), thread: new d3(22));
            ta.AddGpuData(val: 2.3f, index: 3, block: new d3(33), thread: new d3(22));
            ta.AddGpuData(val: 2.2f, index: 4, block: new d3(55), thread: new d3(22));

            var l = ta.GetAllGpuDatas().ToList();
        }

        [TestMethod]
        public void TestKernelNoTile()
        {
            var blockCount = new d3(4);
            var threadCount = new d3(2);
            var arrayOut = new GpuArrayWrite<int>(blockCount.Volume() * threadCount.Volume());
            GpuDevice.RunGpuKernel(
                blockCount, 
                threadCount, 
                TestKernels.IntKernelWrap(arrayOut, blockCount, threadCount));
        }
    }
}
