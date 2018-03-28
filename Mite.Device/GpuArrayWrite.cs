using System.Collections.Generic;
using System.Linq;

namespace Mite.Device
{
    public class GpuArrayWrite<T>
    {
        readonly List<List<GpuData<T>>> _array;

        public GpuArrayWrite(int length)
        {
            _array = new List<List<GpuData<T>>>(length);
            for (var i = 0; i < length; i++)
            {
                _array.Add(new List<GpuData<T>>(1));
            }
        }

        public void AddGpuData(T val, int index, d3 block, d3 thread)
        {
            _array[index].Add(new GpuData<T>(index:index, order:_array[index].Count(), val:val, block:block, thread:thread));
        }

        public List<GpuData<T>> GetGpuData(int index)
        {
            return _array[index];
        }

        public IEnumerable<GpuData<T>> GetAllGpuDatas()
        {
            return _array.SelectMany(d=>d);
        }
    }
}
