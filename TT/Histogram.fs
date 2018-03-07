namespace TT
open System

module Histos =

    type Bin1d = IV<float32, int>


    type Bin2d = RV<float32, int>


    let IncrementBin1d {Bin1d.Min=min; Max=max; V=count} = 
            {Min=min; Max=max; V=count+1}


    let IncrementBin2d {Bin2d.MinX=minX; MaxX=maxX; MinY=minY; MaxY=maxY; V=count} = 
            {MinX=minX; MaxX=maxX; MinY=minY; MaxY=maxY; V=count+1}


    let Bin1dSetMaker (min:float32) (max:float32) (binCount:int) =
        let binWidth = (max-min)/ System.Convert.ToSingle(binCount)
        let bb (i:int) =  min + (float32 i) * binWidth
        [|0..(binCount-1)|] |> Array.map(fun i -> 
                      { Bin1d.Min=bb(i); Max=bb(i+1); V=0 })


    let Bin2dSetMaker (bounds:R<float32>) (binCount:Sz2<int>) =
        let binWidthX = (BT.SpanX bounds)/ (float32 binCount.X)
        let binWidthY = (BT.SpanY bounds)/ (float32 binCount.Y)
        let bbX (i:int) = bounds.MinX + (float32 i) * binWidthX
        let bbY (i:int) = bounds.MinY + (float32 i) * binWidthY       
        Array2D.init binCount.X binCount.Y 
                     (fun x y -> 
                     { MinX=bbX(x); MaxX=bbX(x+1); MinY=bbY(y); MaxY=bbY(y+1); V=0 })


    let Binkey (min:float32) (scale:float32) (value:float32) =
            (int ((value - min) * scale - 0.5f))


    let BinLoader1d keyMaker (binCount:int) (value:float32) (bins:Bin1d[])  =
            let b = keyMaker value
            if ((b > -1) && (b < binCount))
            then bins.[b] <- IncrementBin1d bins.[b]
            bins


    let BinLoader2d keyMakerX (binCountX:int) keyMakerY (binCountY:int)
                    (value:P2<float32>) (bins:Bin2d[,]) =
            let bX = keyMakerX value.X
            let bY = keyMakerY value.Y
            if ((bX > -1) && (bX < binCountX) && (bY > -1) && (bY < binCountY))
            then bins.[bX, bY] <- IncrementBin2d bins.[bX, bY]
            bins


    let Histogram1d (min:float32) (max:float32) (binCount:int) (vals:seq<float32>) =
            let bins = Bin1dSetMaker min max binCount
            let scale = (float32 binCount)/(max-min)
            let keyMaker = Binkey min scale
            let loader = BinLoader1d keyMaker binCount
            Seq.fold ( fun acc value -> loader value acc ) bins vals


    let Histogram2d (bounds:R<float32>) (binCount:Sz2<int>) (vals:seq<P2<float32>>) =
            let bins = Bin2dSetMaker bounds binCount
            let scaleX = (float32 binCount.X)/(BT.SpanX bounds)
            let scaleY = (float32 binCount.Y)/(BT.SpanY bounds)
            let keyMakerX = Binkey bounds.MinX scaleX
            let keyMakerY = Binkey bounds.MinY scaleY
            let loader = BinLoader2d keyMakerX binCount.X keyMakerY binCount.Y
            Seq.fold ( fun acc value -> loader value acc ) bins vals