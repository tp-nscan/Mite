namespace TT
open System
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.Random
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix
open BT
open GenV
open GenS
open GenBT

module VecUt =

    // given the length, and a sequence of values, a dense vector is produced
    let DenseFromSeq<'a when 'a:(new:unit->'a) and 'a:struct and 'a:> IEquatable<'a> and 'a:> IFormattable and 'a:> ValueType>
                 length (mVals:seq<'a>) =
        let chunk = mVals |> Seq.take length |> Seq.toArray
        DenseVector.init length (fun x  -> chunk.[x])


    // given the length, an a sequence of values, a sparse vector is produced
    let SparseFromSeq<'a when 'a:(new:unit->'a) and 'a:struct and 'a:> IEquatable<'a> and 'a:> IFormattable and 'a:> ValueType>
                 length (mVals:seq<int*'a>) =
        Vector<'a>.Build.SparseOfIndexed(length, mVals)


    let GetSparseF32Tuples (vec:Vector<float32>) =
        { 0 .. (vec.Count - 1) } |> Seq.mapi(fun i v -> (i, vec.At(i)))
                                 |> Seq.filter(fun tup -> (snd tup) > 0.0f)


    // given the length, and a sequence of values, a dense vector is produced
    let Mutate<'a when 'a:(new:unit->'a) and 'a:struct and 'a:> IEquatable<'a> and 'a:> IFormattable and 'a:> ValueType>
                 (vector:Vector<float32>) (perturbs:seq<float32>) (pp:(float32->float32))  =
        let chunk = perturbs |> Seq.take vector.Count |> Seq.toArray
        DenseVector.init vector.Count (fun x  -> pp(vector.Item(x) + chunk.[x]))


    let Rotate (steps:int) (vec:Vector<'a>) =
        let mI = vec.Count
        DenseVector.init mI (fun i ->vec.[(i+steps)%mI])


    let Format (numFormat:string) (vector:Vector<float32>)  =
        vector.Enumerate()
            |> Seq.fold(fun acc v -> acc + v.ToString("0.000") + "\t") 
                        String.Empty


module GenVec =

    let DenseF32 length seed =
        let valA = GenS.SeqOfRandUF32 (GenV.Twist seed)
                   |> Seq.take(length)
                   |> Seq.toArray
        DenseVector.init length 
                     (fun x -> valA.[x])


    let RandSparseF32 length count seed =
        let valA = GenS.SeqOfRandUF32 (GenV.Twist seed)
                |> Seq.take(count)
        let dexA = GenS.SeqOfRandZN length (GenV.Twist seed)
                |> Seq.take(count)
                |> Seq.distinctBy id
        VecUt.SparseFromSeq length (Seq.zip dexA valA)


    
module MatrixUt =

    // given the shape of the matrix, and a complete sequence of values, a 2d dense
    // matrix is produced
    let DenseFromSeq<'a when 'a:(new:unit->'a) and 'a:struct and 'a:> IEquatable<'a> and 'a:> IFormattable and 'a:> ValueType>
                 rows cols (mVals:seq<'a>) =
        let chunk = mVals |> Seq.take (rows*cols) |> Seq.toArray
        DenseMatrix.init rows cols (fun x y -> chunk.[x*cols + y])

    // given the shape of the matrix, and a sequence of values, a 2d sparse
    // matrix is produced
    let SparseFromSeq<'a when 'a:(new:unit->'a) and 'a:struct and 'a:> IEquatable<'a> and 'a:> IFormattable and 'a:> ValueType>
                 rows cols (mVals:seq<int*int*'a>) =
        Matrix<'a>.Build.SparseOfIndexed(rows, cols, mVals)

    let ToRowMajorSequence (matrix:Matrix<'a>) = 
         matrix.ToArray() |> Seq.cast<'a>


    let ToUniformDiag (diagVal:'a) (matrix:Matrix<'a>) = 
        DenseMatrix.init matrix.RowCount matrix.ColumnCount 
                    (fun x y -> if (x=y) then diagVal
                                else matrix.[x,y])


    let Format (numFormat:string) (matrix:Matrix<float32>) =
        matrix.EnumerateRows() 
            |> Seq.fold(fun acc v -> acc + (v |> VecUt.Format numFormat) + "\n") 
                        String.Empty


    let ClipSF32 (frac:float32) (matrix:Matrix<float32>) =
        let sA = matrix |> ToRowMajorSequence
                        |> Seq.map(fun v -> Math.Abs(v))
                        |> Seq.toArray 
                        |> Array.sort  
        let bubble = sA.[(NumUt.Fraction32Of sA.Length frac)]
        DenseMatrix.init matrix.RowCount matrix.ColumnCount 
                    (fun x y -> NumUt.ToUF(matrix.[x,y] / bubble))


    let MutatedCopies (pp:(float32->float32)) (matrix:Matrix<float32>)
                            (copies:int) (perturbs:seq<float32>) =
        let ncc = matrix.ColumnCount * copies
        let chunk = perturbs |> Seq.take (matrix.RowCount * ncc) 
                             |> Seq.toArray
        DenseMatrix.init matrix.RowCount (matrix.ColumnCount * copies)
                (fun x y -> pp(matrix.[x,y/copies] + chunk.[x*ncc + y]))


    let MutateCopiesSF (matrix:Matrix<float32>) (copies:int) 
                       (perturbs:seq<float32>) =
        MutatedCopies NumUt.ToSF matrix copies perturbs
        

    let ToP2V (m:Matrix<float32>) =
        seq { for row in 0 .. m.RowCount - 1 do
                for col in 0 .. m.ColumnCount - 1 do
                    yield {P2V.X = col; Y=row; V= m.[row, col]}
            }


    let GetSparseF32Tuples (m:Matrix<float32>) =
        seq { for row in 0 .. m.RowCount - 1 do
                    for col in 0 .. m.ColumnCount - 1 do
                    let v = m.[row, col]
                    if Math.Abs(v) > NumUt.Epsilon then
                        yield (row, col, v)
            }


    let ToLS2V (m:Matrix<float32>) =
        let sd = Convert.ToInt32(Math.Sqrt (Convert.ToDouble(m.RowCount)))
        seq { for row1 in 0 .. sd - 1 do
                for col1 in 0 .. sd - 1 do
                  for row2 in 0 .. sd - 1 do
                    for col2 in 0 .. sd - 1 do
                      yield {LS2V.X1 = row1; 
                                   Y1 = col1; 
                                   X2 = row2; 
                                   Y2 = col2; 
                                   V = m.[row1*sd + col1, row2*sd + col2]}
            }


    let SizeOf (m:Matrix<float32>) =
           {Sz2.X = m.RowCount; Y=m.ColumnCount;}


    let ExtractColumns (matrix:Matrix<float32>) (colDexes: int seq) =
        let dexes = colDexes |> Seq.toList
        DenseMatrix.init matrix.RowCount dexes.Length
                (fun x y -> matrix.[x, dexes.[y]])


    let ExtractRows (matrix:Matrix<float32>) (rowDexes: int seq) =
        let dexes = rowDexes |> Seq.toList
        DenseMatrix.init dexes.Length matrix.ColumnCount
                (fun x y -> matrix.[dexes.[x], y])


    let Column2Norm (matrix:Matrix<float32>) =
        DenseVector.init matrix.ColumnCount 
            (fun x -> {0 .. matrix.RowCount-1} 
                      |> Seq.fold (fun acc y -> acc + matrix.[y,x]*matrix.[y,x]) 0.0f)


    let InOrder (v:Vector<'a>) =
        {0 .. v.Count-1} |> Seq.map (fun i-> v.[i])
 

    let ProjectTo2D (basis:Matrix<float32>) (states:Matrix<float32>) 
                    (dexX: int seq) (dexY: int seq) =
        let basisX = ExtractRows basis dexX
        let basisY = ExtractRows basis dexY
        let xNorms = Column2Norm (basisX.Multiply states)
        let yNorms = Column2Norm (basisY.Multiply states)
        (yNorms |> InOrder) |> Seq.map2 (fun x y -> {P2.X=x; Y=y;} ) (xNorms |> InOrder)



module GenMatrix =

    let DenseF32 (bounds:Sz2<int>) (seed:int) =
        let valA = GenS.SeqOfRandUF32 (GenV.Twist seed)
                   |> Seq.take(BT.Count bounds)
                   |> Seq.toArray
        DenseMatrix.init (bounds.Y) 
                     (bounds.X) 
                     (fun x y -> valA.[y * bounds.X + x])


    let RandSparseF32 (bounds:Sz2<int>) count seed =
        let valA = GenS.SeqOfRandUF32 (GenV.Twist seed)
                    |> Seq.take(count)
        let rSeq = GenS.SeqOfRandZN bounds.Y (GenV.Twist (seed + 1777))
                    |> Seq.take(count)
        let cSeq = GenS.SeqOfRandZN bounds.X (GenV.Twist seed)
                    |> Seq.take(count)
        let m3 = Seq.zip3 rSeq cSeq valA
                    |> Seq.take(count)
                    |> Seq.distinctBy id

        MatrixUt.SparseFromSeq bounds.Y bounds.X m3
