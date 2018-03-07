namespace TT

module Data =

    // (Dense, Sparse) (int, float) (1d, 2d, 4d) (N, U, B, UB) 

    type SparseVal1d<'a> = {dex:int; value:'a}
    type SparseVal2d<'a> = {x:int; y:int; value:'a}
    type SparseVal4d<'a> = {x1:int; y1:int; x2:int; y2:int; value:'a}

    type Er = {ID:int; Fn:string; Ln:string}

    type ValBnd =
    | N       //[minVal, maxVal]
    | U       //[0, maxVal]
    | B       //[-1, 1]
    | UB      //[0, 1]

    type MatrixShape =
    | D1
    | D2
    | D4
    | S1
    | S2
    | S4


    type IntArray =
    | Dense1d of ValBnd * int * int[]
    | Dense2d of ValBnd * Sz2<int> * int[]
    | Dense4d of ValBnd * Sz4<int> * int[]
    | Sparse1d of ValBnd * int * SparseVal1d<int>[]
    | Sparse2d of ValBnd * Sz2<int> * SparseVal2d<int>[]
    | Sparse4d of ValBnd * Sz4<int> * SparseVal4d<int>[]


    type F32Array =
    | Dense1d of ValBnd * int * float32[]
    | Dense2d of ValBnd * Sz2<int> * float32[]
    | Dense4d of ValBnd * Sz4<int> * float32[]
    | Sparse1d of ValBnd * int * SparseVal1d<float32>[]
    | Sparse2d of ValBnd * Sz2<int> * SparseVal2d<float32>[]
    | Sparse4d of ValBnd * Sz4<int> * SparseVal4d<float32>[]


    type GenArray =
    | IntArray of IntArray
    | F32Array of F32Array


    let MakeIntDense1D vb length data =
       GenArray.IntArray (IntArray.Dense1d (vb, length, data))

    let MakeDenseArray vb sz2 data =
        (vb, sz2, data)

    let MakeDense4DInt vb sz4 data =
        (vb sz4 data)

    let MakeSparse1DInt vb length data =
        (vb length data)

    let MakeSparse2DInt vb length data =
        (vb length data)

    let MakeSparse4DInt vb length data =
        (vb length data)

    let MakeDense1DFloat vb length data =
        (vb length data)

    let MakeDense2DFloat vb length data =
        (vb length data)

    let MakeDense4DFloat vb length data =
        (vb length data)

    let MakeSparse1DFloat vb length data =
        (vb length data)

    let MakeSparse2DFloat vb length data =
        (vb length data)

    let MakeSparse4DFloat vb length data =
        (vb length data)