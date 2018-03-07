namespace TT
open System
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.Random

module GenV = 
    
    let Twist (seed:int) =
        MersenneTwister(seed)

    let RandomName (prefix:string) =
        let rng = Random.MersenneTwister()
        let suffix = rng.Next()
        sprintf "%s_%i" prefix suffix


    // 0f <=> 1f with probability p
    let FlipBUF32 (rng:Random) (p:float) (value:float32) =
        if (rng.NextDouble() < p) then
            1.0f - value
        else
            value


    /// -1f <=> 1f with probability p
    let FlipBSF32 (probability:float) (rng:Random) (value:float32) =
        if (rng.NextDouble() < probability) then
            value * -1.0f
        else
            value



module GenS = 

    // a sequence of random int from (0, n)
    let SeqOfRandZN (n:int) (rng:Random) =
        Seq.initInfinite ( fun i -> rng.Next(n))


    let NormalF (rnd:Random) (mean:float32) (stddev:float32) =
        Normal.Samples(rnd=rnd, mean=(float mean), stddev=(float stddev))


    let NormalF32 (rnd:Random) (mean:float32) (stddev:float32) =
        Normal.Samples(rnd=rnd, mean=(float mean), stddev=(float stddev))
            |> Seq.map(fun v->(float32 v))


    let NormalSF32 (rnd:Random) (mean:float32) (stddev:float32) =
        Normal.Samples(rnd=rnd, mean=(float mean), stddev=(float stddev))
            |> Seq.map(fun v->(NumUt.FloatToSF v))


    let NormalUF32 (rnd:Random) (mean:float32) (stddev:float32) =
        Normal.Samples(rnd=rnd, mean=(float mean), stddev=(float stddev))
            |> Seq.map(fun v->(NumUt.FloatToUF v))


    // a sequence of random float32 from (0, max)
    let SeqOfUniformRandF32 (min:float32) (max:float32) (rng:Random) =
        let span = max - min
        Seq.initInfinite ( fun i -> min + (float32 (rng.NextDouble())) * span)


    // a sequence of random float32 from (0, max)
    let SeqOfRandUF32 (rng:Random) =
        Seq.initInfinite ( fun i -> float32 (rng.NextDouble()))


    // a sequence of random float32 from (-max, max)
    let SeqOfRandSF32 (rng:Random) =
        SeqOfUniformRandF32 -1.0f 1.0f rng


    // random draws from {true, false}
    let SeqOfRandBools (rng:Random) (trueProb:float) =
        Seq.initInfinite ( fun i -> rng.NextDouble() < trueProb )


     // random draws from {0f, 1f}
    let SeqOfRandBUF32 (pOfOne:float32) (rng:Random) =
        let myCollapser = NumUt.AorB 0.0f 1.0f pOfOne
        Seq.initInfinite ( fun i -> myCollapser (float32 (rng.NextDouble())))


     // random draws from {-1f, 1f}
    let SeqOfRandBSF32 (pOfOne:float32) (rng:Random) =
        let myCollapser = NumUt.AorB -1.0f 1.0f pOfOne
        Seq.initInfinite ( fun i -> myCollapser (float32 (rng.NextDouble())))


    /// applies FlipUF32WithProb to values:float32[] 
    let FlipBUF32Seq (p:float32) (rng:Random) (values:seq<float32>) =
        let dblM = (float p)
        values |> Seq.map(fun v -> (GenV.FlipBUF32 rng dblM v ))


    /// applies FlipF32WithProb to values:float32[] 
    let FlipBSF32Seq (p:float32) (rng:Random) (values:seq<float32>) =
        let dblM = Convert.ToDouble p
        values |> Seq.map(fun v -> (GenV.FlipBSF32 dblM rng v ))


    let SammpleFromBall (dim:int) (rng:Random) = 
        let gen = SeqOfRandSF32 rng
        Seq.initInfinite
            (fun i ->
                    gen |> (Seq.take dim)
                        |> Seq.toArray
            )
            |> Seq.map(fun s -> (s, (AUt.Len s)))
            |> Seq.filter(fun t -> (snd t) < 1.0f)
            |> Seq.map(fun t ->(fst t))


    let SammpleFromSphere (dim:int) (rng:Random) = 
        let gen = SeqOfRandSF32 rng
        Seq.initInfinite
            (fun i ->
                    gen |> Seq.take dim 
                        |> Seq.toArray
            )
            |> Seq.map(fun s -> (s, (AUt.Len s)))
            |> Seq.filter(fun t -> (snd t) < 1.0f)
            |> Seq.map(fun t ->(fst t) |> AUt.ScaleF32 (1.0f / (snd t)))



module GenBT =

    let P2ofSeq<'a> (values:seq<'a>) =
        values |> Seq.chunkBySize 2     
               |> Seq.map(fun v -> {P2.X=v.[0]; Y=v.[1];})


    let P3ofSeq (values:seq<'a>) =
        values |> Seq.chunkBySize 3  
               |> Seq.map(fun v -> {P3.X=v.[0]; Y=v.[1]; Z=v.[2];})


    let inline IofSeqChunk< ^a when ^a: comparison> (values:seq<'a>) =
        values |> Seq.chunkBySize 2    
               |> Seq.map(fun v -> ( BT.RegularI v.[0] v.[1] ))


    //Sequences that use this are typicaly ordered, so that assumption is
    //neccessary to make intevals 'a Max>Min
    let IofSeqSlide<'a> (values:seq<'a>) =
        values |> Seq.windowed 2    
               |> Seq.map(fun v -> {I.Min=v.[0]; Max=v.[1];})


    let inline RofSeq< ^a when ^a: comparison> (values:seq< ^a>) =
        values |> Seq.chunkBySize 4   
               |> Seq.map(fun v -> (BT.RegularR v.[0] v.[1] v.[2] v.[3])
                         )

    let LS2ofSeq (values:seq<float32>) =
        values |> Seq.chunkBySize 4
               |> Seq.map(fun v -> {LS2.X1=v.[0]; X2=v.[1]; Y1=v.[2]; Y2=v.[3];})


    let TestF32 (seed:int) (count:int) =
         GenS.SeqOfRandSF32 (GenV.Twist seed)
                |> Seq.take count


    let TestP2 (seed:int) (count:int) =
        P2ofSeq (GenS.SeqOfRandSF32 (GenV.Twist seed)) 
                |> Seq.take count


    let TestP2N (mean:float32) (stddev:float32) (seed:int) (count:int) =
        P2ofSeq (GenS.NormalF32 (GenV.Twist seed) mean stddev) 
                |> Seq.take count


    let TestP3 (seed:int) (count:int) =
        P3ofSeq (GenS.SeqOfRandSF32 (GenV.Twist seed)) 
                |> Seq.take count


    let TestI (seed:int) (count:int) =
        IofSeqChunk<float32> (GenS.SeqOfRandSF32 (GenV.Twist seed)) 
                    |> Seq.take count


    let TestR (seed:int) (count:int) =
        RofSeq (GenS.SeqOfRandSF32 (GenV.Twist seed)) 
                |> Seq.take count


    let TestLS2 (seed:int) (count:int) =
        LS2ofSeq (GenS.SeqOfRandSF32 (GenV.Twist seed)) 
                |> Seq.take count


module GenA2 =

    let RandF32 (bounds:Sz2<int>) (seed:int) = 
        let valA = GenS.SeqOfRandUF32 (GenV.Twist seed)
                   |> Seq.take(BT.Count bounds)
                   |> Seq.toArray
        Array2D.init (bounds.Y) 
                     (bounds.X) 
                     (fun x y -> valA.[y * bounds.X + x])


    let RandInt (bounds:Sz2<int>) (maxN:int) (seed:int) = 
        let valA = GenS.SeqOfRandZN maxN (GenV.Twist seed) 
                   |> Seq.take(BT.Count bounds)
                   |> Seq.toArray
        Array2D.init (bounds.Y) 
                     (bounds.X) 
                     (fun x y -> valA.[y * bounds.X + x])

