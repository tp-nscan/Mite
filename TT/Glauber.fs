namespace TT
open System
open MathNet.Numerics
open MathNet.Numerics.Random
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix

module Glauber =

    let IntMult (intFact:int) (rhs:float) =
        rhs * Convert.ToDouble(intFact)

    let IntMultA (intFact:int) (rhs:float) (a:float) =
        a + rhs * Convert.ToDouble(intFact)

    let Glauber1D dist freq decay =
      Math.Exp(-decay*dist)*System.Math.Cos(freq*dist)

    let GlauberVals (range:int) freq decay =
        let rS = Convert.ToDouble (range-1)
        [| for j in 0.0 .. (rS) -> Glauber1D j freq decay|]

    let chkGlauberBalance r f d =
        let gvs = (GlauberVals r f d) 
        (gvs , (gvs |> Array.sum))

    let GlauberRadius2 =
        [|1.0f; 1.0f; -1.0f|]

    let GlauberRadius3 =
        GlauberVals 4 0.95 0.465
        |> Array.map(fun v -> Convert.ToSingle v)

    let GlauberRadius5 =
        GlauberVals 6 0.72 0.3465
        |> Array.map(fun v -> Convert.ToSingle v)

    let GlauberRadius7 =
        GlauberVals 8 0.55 0.231 
        |> Array.map(fun v -> Convert.ToSingle v)

    let GlauberRadius9 =
        GlauberVals 10 0.44 0.168
        |> Array.map(fun v -> Convert.ToSingle v)

    let GlauberRadius radius =
        match radius with
        | 2 -> Some GlauberRadius3
        | 3 -> Some GlauberRadius3
        | 5 -> Some GlauberRadius5
        | 7 -> Some GlauberRadius7
        | 9 -> Some GlauberRadius9
        | _ -> None

    let ClippedRingDistance r m i j =
        let dex = (i + r - j) % r
        if (dex < m + 1) then
            Some dex
        else if (dex > r - m - 1) then
            Some (r - dex)
        else None

    let ClippedRingMatrixValues r (values:array<float32>) i j =
        match ClippedRingDistance r (values.Length - 1) i j with
        | Some res -> values.[res]
        | None -> 0.0f

    let GlauberDenseMatrix rank array =
        DenseMatrix.init
            rank rank (ClippedRingMatrixValues rank array)

    let GlauberSparseMatrix rank array =
        SparseMatrix.init 
            rank rank (ClippedRingMatrixValues rank array)

    let GlauberNeutralDense rank radius =
        match GlauberRadius radius with
        | Some array -> Some (GlauberDenseMatrix rank array)
        | None -> None

    let GlauberNeutralSparse rank radius =
        match GlauberRadius radius with
        | Some array -> Some (GlauberSparseMatrix rank array)
        | None -> None

    let GvByIndex (range:int) (firstFreq:float) (firstDecay:float) (freqDex:int) (decayDex:int) =
        GlauberVals range (IntMultA freqDex firstFreq 0.5) (IntMultA decayDex firstDecay 0.0) |> Array.sum

    let GvMatrix (firstFreq:float) (firstDecay:float) =
        let cellFill = GvByIndex 10 firstFreq firstDecay
        SparseMatrix.init 7 7 cellFill


