namespace TT

module DesignData =

    let Grid2DTestData (strides:Sz2<int>) =
        let denom = ( 0.5f * (float32 (strides.X * strides.Y)))
        A2dUt.Raster2d strides |> Seq.map(fun p ->
           { P2V.X=p.X; Y=p.Y; 
             V = ( float32 (p.X + p.Y * strides.X)) / denom - 1.0f})


    let RasterizeArrayToPoints (strides:Sz2<int>) (values:array<float32>) =
        let denom = ( 0.5f * (float32 (strides.X * strides.Y)))
        A2dUt.Raster2d strides |> Seq.map(fun p ->
           { P2V.X=p.X; Y=p.Y; V = values.[p.X * strides.X + p.Y]})


    let RasterizeArrayToRects<'T> (roiD:R<int>) (roiR:R<float32>) 
                                   (dataWith:int) (f:int->'T) =

        let xD = (roiR.MaxX - roiR.MinX) / (float32 (roiD.MaxX - roiD.MinX))
        let yD = (roiR.MaxY - roiR.MinY) / (float32 (roiD.MaxY - roiD.MinY))
        
        (A2dUt.RasterRoi roiD dataWith )|> Seq.map(fun p ->
           { RV.MinX = (single p.X) * xD; 
             RV.MaxX = (single p.X + 1.f) * xD;
             RV.MinY = (single p.Y) * yD; 
             RV.MaxY = (single p.Y + 1.f) * yD; 
             RV.V = f(p.X * dataWith + p.Y)})