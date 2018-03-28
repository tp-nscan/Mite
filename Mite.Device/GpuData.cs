namespace Mite.Device
{
    public struct GpuData<T>
    {
        public GpuData(int index, int order, T val, d3 block, d3 thread, object ext = null)
        {
            Index = index;
            Order = order;
            Value = val;
            Block = block;
            Thread = thread;
            Ext = ext;
        }

        public int Index;
        public int Order;
        public T Value;
        public object Ext;
        public d3 Block;
        public d3 Thread;
    }
}