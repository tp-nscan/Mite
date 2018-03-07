namespace TT
open System
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.Random
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix

module BTconv =

    type ValBound =
        | N       //[minVal, maxVal]
        | U       //[0, maxVal]
        | B       //[-1, 1]
        | UB      //[0, 1]

    type NumberType =
        | Int
        | F32

    
    type ArrayShape =
        | D1
        | D2
        | D4
        | S1
        | S2
        | S4


    let Base64ToIntA strData =
        let bytes = Convert.FromBase64String strData
        Array.init (bytes.Length / 4) (fun x  -> BitConverter.ToInt32(bytes, x * 4))


    let Base64ToF32A strData =
        let bytes = Convert.FromBase64String strData
        Array.init (bytes.Length / 4) (fun x  -> BitConverter.ToSingle(bytes, x * 4))


    let Base64ToIntA2 (bounds:Sz2<int>) strData =
        let bytes = Convert.FromBase64String strData
        Array2D.init (bounds.Y) 
                     (bounds.X) 
                     (fun x y -> BitConverter.ToInt32(bytes, (x * bounds.X + y) * 4))


    let Base64ToF32A2 (bounds:Sz2<int>) strData =
        let bytes = Convert.FromBase64String strData
        Array2D.init (bounds.Y) 
                     (bounds.X) 
                     (fun x y -> BitConverter.ToSingle(bytes, (x * bounds.X + y) * 4))


    let IntAtoBase64 (intA:int[]) =
        Seq.init (intA.GetLength(0)) (fun i -> BitConverter.GetBytes(intA.[i]))
        |> Seq.concat 
        |> Seq.toArray
        |> Convert.ToBase64String


    let F32AtoBase64 (f32A:float32[]) =
        Seq.init (f32A.GetLength(0)) (fun i -> BitConverter.GetBytes(f32A.[i]))
        |> Seq.concat 
        |> Seq.toArray
        |> Convert.ToBase64String


    let IntA2toBase64 (intA:int[,]) =
        intA |> A2dUt.flattenRowMajor
        |> Seq.map (fun i -> BitConverter.GetBytes(i))
        |> Seq.concat 
        |> Seq.toArray
        |> Convert.ToBase64String


    let F32A2toBase64 (f32A:float32[,]) =
        f32A |> A2dUt.flattenRowMajor
        |> Seq.map (fun i -> BitConverter.GetBytes(i))
        |> Seq.concat 
        |> Seq.toArray
        |> Convert.ToBase64String





module MathNetConv =

    let Base64ToDenseF32Vector length strData =
        let bytes = Convert.FromBase64String strData
        DenseVector.init length (fun x  -> BitConverter.ToSingle(bytes, x * 4))


    let Base64ToSparseF32Vector length (strData:(string*string)) =
        let dexBytes = Convert.FromBase64String (fst strData)
        let valBytes = Convert.FromBase64String (snd strData)
        let mVals = seq {0 .. (dexBytes.GetLength(0) / 4) - 1}
                    |> Seq.map(fun i -> ( BitConverter.ToInt32(dexBytes, i * 4),
                                          BitConverter.ToSingle(valBytes, i * 4)))
        Vector<float32>.Build.SparseOfIndexed(length, mVals)


    let Base64ToDenseF32Matrix (bounds:Sz2<int>) strData =
        let bytes = Convert.FromBase64String strData
        DenseMatrix.init bounds.Y bounds.X (fun r c -> 
            BitConverter.ToSingle(bytes, (r * bounds.X + c ) * 4))


    let Base64ToSparseF32Matrix (bounds:Sz2<int>) (strData:(string*string*string)) =
        let (rDexes, colDexes, vals) = strData
        let rowBytes = Convert.FromBase64String rDexes
        let colBytes = Convert.FromBase64String colDexes
        let valBytes = Convert.FromBase64String vals
        let mVals = seq {0 .. (rowBytes.GetLength(0) / 4) - 1}
                    |> Seq.map(fun i -> ( BitConverter.ToInt32(rowBytes, i * 4),
                                          BitConverter.ToInt32(colBytes, i * 4),
                                          BitConverter.ToSingle(valBytes, i * 4)))
        Matrix<float32>.Build.SparseOfIndexed(bounds.Y, bounds.X, mVals)
        

    let DenseF32VectorToBase64 (f32V:Vector<float32>) =
        Seq.init (f32V.Count) (fun i -> BitConverter.GetBytes(f32V.[i]))
        |> Seq.concat 
        |> Seq.toArray
        |> Convert.ToBase64String


    let SparseF32VectorToBase64 (f32V:Vector<float32>) =
        let tups = VecUt.GetSparseF32Tuples(f32V)
        let strDex = tups |> Seq.map(fun t -> BitConverter.GetBytes(fst t)) 
                          |> Seq.concat
                          |> Seq.toArray
                          |> Convert.ToBase64String

        let strVals = tups |> Seq.map(fun t -> BitConverter.GetBytes(snd t)) 
                           |> Seq.concat
                           |> Seq.toArray
                           |> Convert.ToBase64String

        (strDex, strVals)


    let DenseF32MatrixtoBase64 (f32M:Matrix<float32>) =
        f32M |> MatrixUt.ToRowMajorSequence
        |> Seq.map (fun i -> BitConverter.GetBytes(i))
        |> Seq.concat 
        |> Seq.toArray
        |> Convert.ToBase64String


    let SparseF32MatrixToBase64 (f32M:Matrix<float32>) =
        let T1 (a,b,c) = a
        let T2 (a,b,c) = b
        let T3 (a,b,c) = c

        let tups = MatrixUt.GetSparseF32Tuples(f32M)
        let strRows = tups |> Seq.map(fun t -> BitConverter.GetBytes((T1 t))) 
                           |> Seq.concat
                           |> Seq.toArray
                           |> Convert.ToBase64String

        let strCols = tups |> Seq.map(fun t -> BitConverter.GetBytes((T2 t))) 
                           |> Seq.concat
                           |> Seq.toArray
                           |> Convert.ToBase64String

        let strVals = tups |> Seq.map(fun t -> BitConverter.GetBytes(T3 t)) 
                           |> Seq.concat
                           |> Seq.toArray
                           |> Convert.ToBase64String

        (strRows, strCols, strVals)