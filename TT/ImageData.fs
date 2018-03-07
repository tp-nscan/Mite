namespace TT
open System
open System.Windows.Media

type ImageData = { plotPoints:P2V<float32, Color>[]; filledRects:RV<float32, Color>[];
                   openRects:RV<float32, Color>[]; plotLines:LS2V<float32, Color>[];
                   imageSize:Sz2<float>; boundingRect:R<float32> }

module Id =

 let MakeImageData (plotPoints: seq<P2V<float32, Color>>) 
                         (filledRects: seq<RV<float32, Color>>) 
                         (openRects: seq<RV<float32, Color>>)
                         (plotLines: seq<LS2V<float32, Color>>) 
                         (imageSize: Sz2<float>) =

    let pps = plotPoints |> Seq.toArray
    let ors = openRects |> Seq.toArray
    let frs = filledRects |> Seq.toArray
    let pls = plotLines |> Seq.toArray

    let mutable br = pps |> Array.fold (BT.StretchRP) BT.AntiRofF32
    br <- ors |> Array.fold (BT.StretchRR) br
    br <- frs |> Array.fold (BT.StretchRR) br
    br <- pls |> Array.fold (BT.StretchRL) br

    { plotPoints=pps; filledRects=frs; openRects=ors; plotLines=pls; 
        imageSize=imageSize; boundingRect={ R.MinX=(br.MinX);
                                            R.MaxX=(br.MaxX);
                                            R.MinY=(br.MinY);
                                            R.MaxY=(br.MaxY);} }
