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


    let RasterizeArrayToRects (roiD:Sz4<int>) (roiR:Sz4<float>) 
                              (dataWith:int) (values:array<float32>) =

        let xD = (roiR.X2 - roiR.X1) / ((float roiD.X2) - (float roiD.X1))
        let yD = (roiR.Y2 - roiR.Y1) / ((float roiD.Y2) - (float roiD.Y1))
        
        (A2dUt.RasterRoi roiD dataWith )|> Seq.map(fun p ->
           { RV.MinX = (float p.X) * xD; 
             RV.MaxX = (float p.X + 1.) * xD;
             RV.MinY = (float p.Y) * yD; 
             RV.MaxY = (float p.Y + 1.) * yD; 
             RV.V = values.[p.X * dataWith + p.Y]})


    let RasterizeArrayToRects2<'T> (roiD:Sz4<int>) (roiR:Sz4<single>) 
                                   (dataWith:int) (f:int->'T) =

        let xD = (roiR.X2 - roiR.X1) / ((single roiD.X2) - (single roiD.X1))
        let yD = (roiR.Y2 - roiR.Y1) / ((single roiD.Y2) - (single roiD.Y1))
        
        (A2dUt.RasterRoi roiD dataWith )|> Seq.map(fun p ->
           { RV.MinX = (single p.X) * xD; 
             RV.MaxX = (single p.X + 1.f) * xD;
             RV.MinY = (single p.Y) * yD; 
             RV.MaxY = (single p.Y + 1.f) * yD; 
             RV.V = f(p.X * dataWith + p.Y)})