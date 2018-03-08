namespace TT


type P2<'c>  = { X:'c; Y:'c }
type Sz2<'c>  = { X:'c; Y:'c }
type Sz4<'c>  = { X1:'c; Y1:'c; X2:'c; Y2:'c }
type P3<'c>  = { X:'c; Y:'c; Z:'c }
type I<'c>  = { Min:'c; Max:'c }
type R<'c>  = { MinX:'c; MaxX:'c; MinY:'c; MaxY:'c } 
type LS2<'c>  = { X1:'c; Y1:'c; X2:'c; Y2:'c } 

type P1V<'c,'v>  = { X:'c; V:'v }
type P2V<'c,'v>  = { X:'c; Y:'c; V:'v }
type P3V<'c,'v>  = { X:'c; Y:'c; Z:'c; V:'v }
type IV<'c,'v>  = { Min:'c; Max:'c; V:'v  }
type RV<'c,'v>  = { MinX:'c; MaxX:'c; MinY:'c; MaxY:'c; V:'v }
type LS2V<'c,'v>  = { X1:'c; Y1:'c; X2:'c; Y2:'c; V:'v }

module NumUt =
    
    let Epsilon = 0.00001f

    let Fraction32Of (num:int) (frac:float32) =
        System.Convert.ToInt32(((float32 num) * frac))

    let AbsF32 (v:float32) =
        if v < 0.0f then -v
        else v


    let MapUF (min:float) (max:float) (value:float) =
        let span = max-min
        min + value*span


    let inline AddInRange min max a b =
        let res = a + b
        if res < min then min
        else if res > max then max
        else res


    let inline FlipWhen a b flipProb draw current =
        match (flipProb < draw) with
        | true -> if a=current then b else a
        | false -> current


    let ToUF value =
        if value < 0.0f then 0.0f
        else if value > 1.0f then 1.0f
        else value


    let ToSF value =
        if value < -1.0f then -1.0f
        else if value > 1.0f then 1.0f
        else value


    let FloatToUF value =
        if value < 0.0 then 0.0f
        else if value > 1.0 then 1.0f
        else (float32 value)


    let FloatToSF value =
        if value < -1.0 then -1.0f
        else if value > 1.0 then 1.0f
        else (float32 value)


    let inline AorB a b thresh value =
        if value < thresh then a
        else b



module BT =

    let AntiIofS = 
        { 
            I.Min = System.Single.PositiveInfinity; 
            Max = System.Single.NegativeInfinity; 
        }

    let AntiIofInt = 
        { 
            I.Min = System.Int32.MaxValue; 
            Max = System.Int32.MinValue;
        }

    let AntiRofF32 = 
        { 
            R.MinX = System.Single.PositiveInfinity; 
            R.MaxX = System.Single.NegativeInfinity; 
            MinY = System.Single.PositiveInfinity;  
            MaxY = System.Single.NegativeInfinity; 
        }

    let AntiRofInt = 
        { 
            R.MinX = System.Int32.MaxValue; 
            R.MaxX = System.Int32.MinValue; 
            MinY = System.Int32.MaxValue;  
            MaxY = System.Int32.MinValue; 
        }


    // makes an interval from arbitrary values with the expected ordering
    let inline RegularI< ^a when ^a: comparison> (x1:^a) (x2:^a) =
        if x1 < x2 then
            { I.Min=x1; I.Max=x2 }
        else 
            { I.Min=x2; I.Max=x1 }
            

    // makes a rectangle from arbitrary values with the expected orderings
    let inline RegularR< ^a when ^a: comparison> (x1:^a) (x2:^a) (y1:^a) (y2:^a) =
        if x1 < x2 then
            if y1 < y2 then
                { R.MinX=x1; R.MaxX=x2; R.MinY=y1; R.MaxY=y2; }
            else 
                { R.MinX=x1; R.MaxX=x2; R.MinY=y2; R.MaxY=y1; }
        else 
            if y1 < y2 then
                { R.MinX=x2; R.MaxX=x1; R.MinY=y1; R.MaxY=y2; }
            else 
                { R.MinX=x2; R.MaxX=x1; R.MinY=y2; R.MaxY=y1; }


    let inline RVClipByR< ^v, ^a when ^a: comparison> (bounds : R< ^a>) (mrv : RV< ^a, ^v>) =
      { 
        RV.MinX = if (mrv.MinX > bounds.MinX) then mrv.MinX else bounds.MinX; 
        RV.MaxX = if (mrv.MaxX < bounds.MaxX) then mrv.MaxX else bounds.MaxX;  
        RV.MinY = if (mrv.MinY > bounds.MinY) then mrv.MinY else bounds.MinY;  
        RV.MaxY = if (mrv.MaxY < bounds.MaxY) then mrv.MaxY else bounds.MaxY; 
        V = mrv.V
      }


    let inline walk_the_creature_2 (creature:^a when ^a:(member Walk : unit -> unit)) =
        (^a : (member Walk : unit -> unit) creature)


    let inline Span (range:^a when ^a:(member Min : ^b) and ^a:(member Max : ^b)) =
       (^a : (member Max : ^b) range) - (^a : (member Min : ^b) range)


    let inline Mid (range:^a when ^a:(member Min : ^b) and ^a:(member Max : ^b)) 
                   (p1:^b) =
       ((^a : (member Max : ^b) range) + (^a : (member Min : ^b) range) ) / 2


    let inline InL (range:^a when ^a:(member Min : ^b) and ^a:(member Max : ^b)) 
                   (p1:^b) =
       ((^a : (member Max : ^b) range) >= p1) && ((^a : (member Min : ^b) range) <= p1)


    let inline Area (range:^a when 
                        ^a:(member MinX : ^b) and ^a:(member MaxX : ^b) 
                        and
                        ^a:(member MinY : ^b) and ^a:(member MaxY : ^b)
                    ) =

       ((^a : (member MaxX : ^b) range) - (^a : (member MinX : ^b) range))
       *
       ((^a : (member MaxY : ^b) range) - (^a : (member MinY : ^b) range))


    let inline SpanX (range:^a when ^a:(member MinX : ^b) and ^a:(member MaxX : ^b) ) =
       ((^a : (member MaxX : ^b) range) - (^a : (member MinX : ^b) range))


    let inline SpanY (range:^a when ^a:(member MinY : ^b) and ^a:(member MaxY : ^b) ) =
       ((^a : (member MaxY : ^b) range) - (^a : (member MinY : ^b) range))


    let inline InRP (range:^a when 
                        ^a:(member MinX : ^b) and ^a:(member MaxX : ^b) 
                        and
                        ^a:(member MinY : ^b) and ^a:(member MaxY : ^b)
                    ) 
                    (p2:^c when 
                        ^c:(member X : ^b) and ^c:(member Y : ^b) 
                    ) =

       (^a : (member MaxX : ^b) range) >= (^c : (member X : ^b) p2)
       &&
       (^a : (member MinX : ^b) range) <= (^c : (member X : ^b) p2)
       &&
       (^a : (member MaxY : ^b) range) >= (^c : (member Y : ^b) p2)
       &&
       (^a : (member MinY : ^b) range) <= (^c : (member Y : ^b) p2)


    let inline StretchIP (range:I< ^b>) (p1:^b ) =
        { 
            I.Min = if (range.Min > p1) then p1 else range.Min
            Max   = if (range.Max < p1) then p1 else range.Max
        }


    let inline StretchII (range:I< ^b>) (iv:^c when ^c:(member Min : ^b)
                                                and ^c:(member Max : ^b) ) =
        { 
            I.Min = if (range.Min > (^c : (member Min : ^b) iv)) then (^c : (member Min : ^b) iv) else range.Min
            Max   = if (range.Max < (^c : (member Max : ^b) iv)) then (^c : (member Max : ^b) iv) else range.Max
        }


    let inline StretchRP (box:R< ^b>) (p2:^c when ^c:(member X : ^b) and ^c:(member Y : ^b) ) =
        { 
            R.MinX = if (box.MinX > (^c : (member X : ^b) p2)) then (^c : (member X : ^b) p2) else box.MinX
            MaxX   = if (box.MaxX < (^c : (member X : ^b) p2)) then (^c : (member X : ^b) p2) else box.MaxX 
            MinY   = if (box.MinY > (^c : (member Y : ^b) p2)) then (^c : (member Y : ^b) p2) else box.MinY
            MaxY   = if (box.MaxY < (^c : (member Y : ^b) p2)) then (^c : (member Y : ^b) p2) else box.MaxY
        }

    let inline StretchRPF32 (box:R<float32>) (p2:P2<float32>) =
        { 
            R.MinX = if (box.MinX > p2.X) then p2.X else box.MinX
            MaxX   = if (box.MaxX < p2.X) then p2.X else box.MaxX
            MinY   = if (box.MinY > p2.Y) then p2.Y else box.MinY
            MaxY   = if (box.MaxY < p2.Y) then p2.Y else box.MaxY
        }

    let inline StretchRR (box:R< ^b>) 
                           (rect:^c when ^c:(member MinX : ^b) and ^c:(member MaxX : ^b) 
                                     and ^c:(member MinY : ^b) and ^c:(member MaxY : ^b)) =
        { 
            R.MinX = if (box.MinX > (^c : (member MinX : ^b) rect)) then (^c : (member MinX : ^b) rect) else box.MinX
            MaxX   = if (box.MaxX < (^c : (member MaxX : ^b) rect)) then (^c : (member MaxX : ^b) rect) else box.MaxX 
            MinY   = if (box.MinY > (^c : (member MinY : ^b) rect)) then (^c : (member MinY : ^b) rect) else box.MinY
            MaxY   = if (box.MaxY < (^c : (member MaxY : ^b) rect)) then (^c : (member MaxY : ^b) rect) else box.MaxY
        }


    let inline StretchRL (box:R< ^b>) 
                           (line:^c when ^c:(member X1 : ^b) and ^c:(member X2 : ^b) 
                                     and ^c:(member Y1 : ^b) and ^c:(member Y2 : ^b)) =
        { 
            R.MinX = if (box.MinX > (^c : (member X1 : ^b) line)) then (^c : (member X1 : ^b) line) else box.MinX
            MaxX   = if (box.MaxX < (^c : (member X2 : ^b) line)) then (^c : (member X2 : ^b) line) else box.MaxX 
            MinY   = if (box.MinY > (^c : (member Y1 : ^b) line)) then (^c : (member Y1 : ^b) line) else box.MinY
            MaxY   = if (box.MaxY < (^c : (member Y2 : ^b) line)) then (^c : (member Y2 : ^b) line) else box.MaxY
        }


    let inline FilterRP (box:R< ^b>) (p2:seq< ^c> when ^c:(member X : ^b) and ^c:(member Y : ^b) ) =
            p2 |> Seq.filter(fun p -> InRP box p)


    let inline BoundingRP (box:R< ^b>) (p2:seq< ^c> when ^c:(member X : ^b) and ^c:(member Y : ^b) ) =
            p2 |> Seq.fold (fun acc elem -> StretchRP acc elem ) box


    let inline BoundRectP2F32 p2s =
            p2s |> Array.fold(fun acc p -> StretchRPF32 acc p ) AntiRofF32 


    let inline BoundingRR (box:R< ^b>) 
                            (rects:seq< ^c> when ^c:(member MinX : ^b) and ^c:(member MaxX : ^b) 
                                             and ^c:(member MinY : ^b) and ^c:(member MaxY : ^b)) =
            rects |> Seq.fold (fun acc elem -> StretchRR acc elem ) box


    let inline BoundingRLS2 (box:R< ^b>) 
                            (lines:seq< ^c> when ^c:(member X1 : ^b) and ^c:(member X2 : ^b) 
                                             and ^c:(member Y1 : ^b) and ^c:(member Y2 : ^b)) =
            lines |> Seq.fold (fun acc elem -> StretchRL acc elem ) box


    let inline BoundingIP (range:I< ^b>) (p1:seq< ^b>) =
            p1 |> Seq.fold (fun acc elem -> StretchIP acc elem ) range


    let inline BoundingII (range:I< ^b>) 
                          (sI:seq< ^a> when ^a:(member Min: ^b) 
                                        and ^a:(member Max: ^b)) =
            sI |> Seq.fold (fun acc elem -> StretchII acc elem ) range


    let Count (sz:Sz2<int>) =
        sz.X * sz.Y


module AUt =

    let Hamming s1 s2 = Array.map2((=)) s1 s2 |> Seq.sumBy(fun b -> if b then 0 else 1)


    let inline CompareArrays<'a> comp (seqA:'a[]) (seqB:'a[]) =
        Seq.fold (&&) true (Seq.zip seqA seqB |> Seq.map (fun (aa,bb) -> comp aa bb))


    let inline LenSq zeroVal (array:'a[]) =
        array |> Array.fold(fun acc v -> acc + v*v) zeroVal  


    let Len (array:float32[]) =
        array |> LenSq 0.0f
              |> float
              |> sqrt
              |> float32


    let ScaleF32 (scale:float32) (array:float32[]) =
        array |> Array.map(fun v -> scale *v)


    let Scale (scale:float) (array:float32[]) =
        array |> ScaleF32 (float32 scale)


module A2dUt = 

    let Raster2d (strides:Sz2<int>) =
        seq { for col in 0 .. strides.X - 1 do
                for row in 0 .. strides.Y - 1 do
                    yield {P2.X= col; Y= row;}
            }
       
    let RasterRoi (roiD:R<int>) (dataWidth:int) =
       seq { for col in roiD.MinX .. roiD.MaxX - 1 do
               for row in roiD.MinY .. roiD.MaxY - 1 do
                  yield {P2.X= col; Y= row;}
           }

    let SymRowMajorIndex x y =
        if (x >= y) then x*(x+1)/2 + y
        else y*(y+1)/2 + x

    let UtCoords stride =
       let rec qq stride coords =
           seq {
                    match coords with
                    | a, b when b < a -> 
                        yield (a, b)
                        yield! (qq stride (a, b + 1))
                    | a, b when b < (stride- 1) -> 
                        yield (a, b)
                        yield! qq stride (a + 1, 0)
                    | a, b ->
                        yield (a, b)
           }
       seq {
             yield!  qq stride (0, 0)
       }
         

    // Creates a 1D array that represents a 2d upper triangular matrix
    let UpperTriangulate sqSize f =
        let cached = (UtCoords sqSize) 
                        |> Seq.map(fun (a,b) -> f a b)
                        |> Seq.toArray                            
        (fun x y -> cached.[SymRowMajorIndex x y] )


  // Creates a 1D array that represents a 2d upper triangular matrix with uniform diagonal
    let UpperTriangulateDiag (diagVal:'a) stride f =
        let cached = (UtCoords stride) 
                        |> Seq.map(fun (a,b) -> 
                                        match (a,b) with
                                        | (a,b) when a=b -> diagVal
                                        | (a,b) -> f a b)
                        |> Seq.toArray                            

        (fun x y -> cached.[SymRowMajorIndex x y] )


    let inline SetUniformDiagonal (mtx:'a[,]) (diagVal) = 
        Array2D.init (mtx.GetLength 0) 
                     (mtx.GetLength 1) 
                     (fun x y -> if(x=y) then diagVal  else mtx.[x,y])


    let Transpose (mtx : _ [,]) = 
        Array2D.init (mtx.GetLength 1) (mtx.GetLength 0) (fun x y -> mtx.[y,x])
    

    let GetRowsForArray2D (array:'a[,]) =
      let rowC = array.GetLength(0)
      let colC = array.GetLength(1)
      Array.init rowC (fun i ->
        Array.init colC (fun j -> array.[i,j]))


    let FillRowMajor rowCount colCount (vals:seq<'a>) =
        let data = vals |> Seq.take(rowCount*colCount) |> Seq.toArray
        Array2D.init rowCount colCount (fun x y -> data.[y+x*colCount])
    

    let FillColumnMajor rowCount colCount (vals:seq<'a>) =
        let data = vals |> Seq.take(rowCount*colCount) |> Seq.toArray
        Array2D.init rowCount colCount (fun x y -> data.[x+y*colCount])


    let flattenRowMajor (A:'a[,]) = A |> Seq.cast<'a>


    let flattenColumnMajor (A:'a[,]) = A |> Transpose |> Seq.cast<'a>


    let getColumn c (A:_[,]) = A.[*,c] |> Seq.toArray


    let getRow r (A:_[,]) = A.[r,*] |> Seq.toArray


    let P2sForA2d (rowCt:int) (colCt:int) =
        seq { for row in 0 .. rowCt - 1 do
                for col in 0 .. colCt - 1 do
                    yield {P2.X = col; Y=row; }
        }


    let ToP2V (a2d:'a[,]) =
        seq { for row in 0 .. (a2d.GetLength 0) - 1 do
                for col in 0 .. (a2d.GetLength 1) - 1 do
                    yield {P2V.X = col; Y=row; V= a2d.[row, col]}
            }


module SeqUt = 

    let IterB (s:System.Collections.Generic.IEnumerator<'a>) =
        function () ->
                    s.MoveNext() |> ignore
                    s.Current


    let inline ZipMap f a b = Seq.zip a b |> Seq.map (fun (x,y) -> f x y)


//    let SeqToI (vals:seq<'a>) =
//        
//        let itsy = vals.GetEnumerator()
//        let mutable movin = itsy.MoveNext()
//        if not (itsy.MoveNext()) then
//            Seq.empty<I<'a>>
//        else
//            let mutable first = itsy.Current
//            let mutable second = itsy.Current
//            seq {
//                    while itsy.MoveNext() do
//                        second <- itsy.Current
//                        yield {I.Min = first; I.Max =second}
//                        first <- second
//                }
//        

    let AddInRange min max offsets baseSeq =
        Seq.map2 (NumUt.AddInRange min max) baseSeq offsets


    let AddInUF32 offsets baseSeq =
        Seq.map2 (NumUt.AddInRange 0.0f 1.0f) baseSeq offsets


    let AddInSF32 offsets baseSeq =
        Seq.map2 (NumUt.AddInRange -1.0f 1.0f) baseSeq offsets
