namespace TT

module FuncTable =
    type ArcTanh(gain:float32, res:int) =

        member this.Gain = gain
        member this.Resolution = res
        member this.fRes = (float32 res)
        member this.Values =
            let fgain = (float gain)
            let step = fgain /(float res)
            let exps = [|0..2*res|] |> Array.map(fun i ->  
                          exp(- fgain + step * (float i)))
            [|0..res|] |> Array.map(fun i -> 
                           let p = exps.[res + i]
                           let m = exps.[res - i]
                           (float32((p-m) /(p+m))))

        member this.GetValue(x:float32) =
            let sDex = System.Convert.ToInt32(x*this.fRes)
            if (x<0.0f) then - this.Values.[-sDex]
            else this.Values.[sDex]

