namespace TT
open System
open System.Windows.Media

type ImageData = { plotPoints:P2V<float32, Color>[]; filledRects:RV<float32, Color>[];
                   openRects:RV<float32, Color>[]; plotLines:LS2V<float32, Color>[];
                   boundingRect:R<float32>;}

type GraphData = { Title:string; TitleX:string; TitleY:string;
                   xLabeler:float32->string; yLabeler:float32->string}


module Id =

    let MakeImageData (plotPoints: seq<P2V<float32, Color>>) 
                    (filledRects: seq<RV<float32, Color>>) 
                    (openRects: seq<RV<float32, Color>>)
                    (plotLines: seq<LS2V<float32, Color>>) =

        let pps = plotPoints |> Seq.toArray
        let ors = openRects |> Seq.toArray
        let frs = filledRects |> Seq.toArray
        let pls = plotLines |> Seq.toArray

        let mutable br = pps |> Array.fold (BT.StretchRP) BT.AntiRofF32
        br <- ors |> Array.fold (BT.StretchRR) br
        br <- frs |> Array.fold (BT.StretchRR) br
        br <- pls |> Array.fold (BT.StretchRL) br

        { 
            plotPoints=pps;
            filledRects=frs; 
            openRects=ors; 
            plotLines=pls; 
            boundingRect={ R.MinX=(br.MinX);
                            R.MaxX=(br.MaxX);
                            R.MinY=(br.MinY);
                            R.MaxY=(br.MaxY);};
        }


                            
    let ClipImageData (imageData: ImageData) (clipRegion:R<float32> ) =
        { 
            plotPoints = imageData.plotPoints; 
            filledRects = imageData.filledRects |> Array.map(BT.RVClipByR<Color, float32> clipRegion);  
            openRects = imageData.openRects |> Array.map(BT.RVClipByR<Color, float32> clipRegion); 
            plotLines = imageData.plotLines; 
            boundingRect = clipRegion;
        }



    let MakeImageDataAndClip (plotPoints: seq<P2V<float32, Color>>) 
                             (filledRects: seq<RV<float32, Color>>)
                             (openRects: seq<RV<float32, Color>>)
                             (plotLines: seq<LS2V<float32, Color>>)
                             (clipRegion: R<float32>) =

     let iD = MakeImageData plotPoints filledRects openRects plotLines
     ClipImageData iD clipRegion

    let InitImageData() = 
        { 
            ImageData.plotPoints = Array.empty<P2V<float32, Color>>; 
            filledRects = Array.empty<RV<float32, Color>>;  
            openRects = Array.empty<RV<float32, Color>>;
            plotLines = Array.empty<LS2V<float32, Color>>; 
            boundingRect = { R.MinX = (float32 0); R.MaxX = (float32 0); R.MinY = (float32 0); R.MaxY = (float32 0)}; 
        }


    let MakeGraphData (title: string)
                      (titleX: string)
                      (titleY: string)
                      (xLabeler: float32->string)
                      (yLabeler: float32->string) =

        {
            Title = title;  
            TitleX = titleX; 
            TitleY = titleY;
            xLabeler = xLabeler;
            yLabeler = yLabeler;
        }