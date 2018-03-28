using Mite.Device;
using TT;

namespace Mite.WPF.ViewModel.Common
{
    public class GpuArrayVm : GraphTilesVm
    {
        public GpuArrayVm(
            GpuArrayWrite<float> gpuArrayWrite,
            R<int> latticeBounds, 
            string title = "Title", 
            string titleX = "TitleX", 
            string titleY = "TitleY") : base(latticeBounds, title, titleX, titleY)
        {
            GpuArrayWrite = gpuArrayWrite;
            SetUpdater(MakeTileVm);
        }

        public GpuArrayWrite<float> GpuArrayWrite { get; private set; }

        private TileVm MakeTileVm(P2<int> dataLoc, R<double> imagePatch)
        {
            var offset = dataLoc.X + dataLoc.Y * (LatticeBounds.MaxX - LatticeBounds.MinX);
            var gpuData = GpuArrayWrite.GetGpuData(offset)[0];
            var vmRet = new TileVm
            {
                BoundingRect = new R<float>(
                    minX: (float)imagePatch.MinX,
                    maxX: (float)imagePatch.MaxX,
                    minY: (float)imagePatch.MinY,
                    maxY: (float)imagePatch.MaxY
                ),
                Color = ColorSets.GetLegColor(ColorSets.RedBlueSFLeg, gpuData.Value),
                TextA = $"({gpuData.Block.X}, {gpuData.Block.Y}, {gpuData.Block.Z})",
                TextB = $"",
                TextC = $"({gpuData.Thread.X}, {gpuData.Thread.Y}, {gpuData.Thread.Z})"
            };
            return vmRet;
        }
    }
}
