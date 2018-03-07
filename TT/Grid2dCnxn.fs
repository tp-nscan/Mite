namespace TT
open System
open MathNet.Numerics
open MathNet.Numerics.Random
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix

module Grid2dCnxn =
    
    type CnxPattern =
        | Star
        | Ring
        | Block of int
        | Disk of int


    let StarNbrs = 
        seq {
              yield  { P2.X =  0; Y = -1; };
              yield  { P2.X =  1; Y =  0; }; 
              yield  { P2.X =  0; Y =  1; };
              yield  { P2.X = -1; Y =  0; }
            }

 
    let StarNbrsF<'T> fV = 
        [| 
            { P2V.X =  0; Y = -1; V = fV  0 -1};
            { P2V.X =  1; Y =  0; V = fV -1  0}; 
            { P2V.X =  0; Y =  1; V = fV  0  1};
            { P2V.X = -1; Y =  0; V = fV -1  0}
        |]


    let RingNbrs = 
        seq {  
            yield { P2.X = -1; Y = -1; };
            yield { P2.X =  0; Y = -1; };
            yield { P2.X =  1; Y = -1; };

            yield { P2.X = -1; Y =  0; };
            yield { P2.X =  1; Y =  0; };

            yield { P2.X = -1; Y =  1; };
            yield { P2.X =  0; Y =  1; };
            yield { P2.X =  1; Y =  1; };
        }


    let RingNbrsF<'T> fV = 
        [|  
            { P2V.X = -1; Y = -1; V = fV -1 -1};
            { P2V.X =  0; Y = -1; V = fV  0 -1};
            { P2V.X =  1; Y = -1; V = fV  1 -1};

            { P2V.X = -1; Y =  0; V = fV -1  0};
            { P2V.X =  1; Y =  0; V = fV  1  0};

            { P2V.X = -1; Y =  1; V = fV -1  1};
            { P2V.X =  0; Y =  1; V = fV  0  1};
            { P2V.X =  1; Y =  1; V = fV  1  1};
        |]


    let BlockNeighbors (radius:int) =
        seq {  
            for row in -radius .. radius do
                    for col in -radius .. radius do
                        yield { P2.X = col; Y=row; }
        }


    let BlockNeighborsF (radius:int) fV =
        [|
            for row in -radius .. radius do
                    for col in -radius .. radius do
                        yield { P2V.X = col; Y=row; V= fV col row }
        |]


    let DiskNbrs (radius:int) =
        let rFloat = (float radius)
        seq {
            for row in -radius .. radius do
                let rowsq = float ( row*row )
                for col in -radius .. radius do
                    let colsq =  float ( col*col )
                    let dsq = Math.Sqrt(colsq + rowsq)
                    if (dsq < rFloat + 0.415) then
                        yield { P2.X = col; Y=row; }
            }


    let DiskNbrsF (radius:int) fV =
        let rFloat = (float radius)
        seq {
            for row in -radius .. radius do
                let rowsq = float ( row*row )
                for col in -radius .. radius do
                    let colsq =  float ( col*col )
                    let dsq = Math.Sqrt(colsq + rowsq)
                    if (dsq < rFloat + 0.415) then
                        yield { P2V.X = col; Y=row; V= fV col row }
            }
         

    // returns a seq<P2V<int,P2<int>>> of connection info
    let AllOffsets (strides:Sz2<int>) (offsets:seq<P2<int>>) =
        A2dUt.Raster2d strides 
        |> Seq.map(fun pt-> offsets 
                            |> Seq.map(fun off-> {P2V.X=pt.X; Y=pt.Y; V=off}))
        |> Seq.concat


   
    let GeneralLS2Vs (strides:Sz2<int>) (offsets:seq<P2<int>>) 
                 (localWeights:int->int->int->int->float32) =
        AllOffsets strides offsets
        |> Seq.map(fun inf-> 
                    { 
                      LS2V.X1 = inf.X; 
                           Y1 = inf.Y; 
                           X2 = (inf.X + inf.V.X + strides.X) % strides.X; 
                           Y2 = (inf.Y + inf.V.Y + strides.Y) % strides.Y;
                            V = localWeights inf.X inf.Y inf.V.X inf.V.Y
                     }
                  )

   
    let InvariantLS2Vs (strides:Sz2<int>) (offsets:seq<P2<int>>) 
                 (localWeights:int->int->float32) =
        AllOffsets strides offsets
        |> Seq.map(fun inf-> 
                    { 
                      LS2V.X1 = inf.X; 
                           Y1 = inf.Y; 
                           X2 = (inf.X + inf.V.X + strides.X) % strides.X; 
                           Y2 = (inf.Y + inf.V.Y + strides.Y) % strides.Y;
                            V = localWeights inf.V.X inf.V.Y
                     }
                  )


    let UniformStar (strides:Sz2<int>) = 
        InvariantLS2Vs strides StarNbrs (fun x y -> 1.0f)


    let GradientStar (strides:Sz2<int>) = 
        let gf x1 y1 x2 y2 =
            let x1f = (float32 x1)
            let y1f = (float32 x2)
            let x2f = (float32 y1)
            let y2f = (float32 y2)
            let sX = (float32 strides.X)
            let sY = (float32 strides.Y)
            (x1f - y1f) / sX
        GeneralLS2Vs strides StarNbrs gf


//    let LS2VForNode (offsets: (int->int->float32) -> P2V<int,float32>[]) 
//                    (valuator:int->int->float32) 
//                    (stride:int) (x:int) (y:int) =
//        (offsets valuator) |> Array.map( 
//            fun p -> { 
//                      LS2V.X1 = x; 
//                           Y1 = y; 
//                           X2 = (p.X + x + stride) % stride; 
//                           Y2 = (p.Y + y + stride) % stride;
//                           V = p.V
//                     }
//            )


    let MatrixEntryToVectorEntry colCount (mE:int*int*float32) =
        match mE with
        | (r,c,v) -> (r*colCount + c, v)


    let Coordify stride dex =
        let modus = dex % stride
        {
            P2.X = modus;
            P2.Y = (dex - modus) / stride;
        }


    let Cnx4dToCnx2d (stride:int) (coords:seq<LS2<int>>) =
        coords |> Seq.map(fun d4->{P2.X = d4.Y1*stride + d4.X1; 
                                      Y = d4.Y2*stride + d4.X2})


    let Z4VTo3Tuple (stride:int) (coords:seq<LS2V<int,float32>>) =
        coords |> Seq.map(fun z4V-> 
            (z4V.Y1*stride + z4V.X1, z4V.Y2*stride + z4V.X2, z4V.V))

    
    let CnxZ4To3Tuple (stride:int) (coords:seq<LS2V<int, float32>>) =
        coords |> Seq.map(fun z4-> 
            (z4.Y1*stride + z4.X1, z4.Y2*stride + z4.X2, z4.V))


    //let Matrix3TupleToZ4 (stride:int) (tuples: seq<Tuple<int,int,float32>>) =
    //    tuples |> Seq.map(fun t-> 
    //        let ax1 = Coordify stride t.
    //        let ax2 = Coordify stride t.Item2
    //        {
    //            LS2V.X1  = ax1.X; 
    //                  X2  = ax1.Y; 
    //                  Y1  = ax2.X;
    //                  Y2  = ax2.Y;
    //                  V = t.Item3
    //         })


    let LS2VForGridWithRingNbrs (stride:int) =
        let rng = GenV.Twist(123)
        let seqVals = GenS.NormalF32 rng 0.0f 0.31f
        None