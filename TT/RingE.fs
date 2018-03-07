namespace TT
open MathNet.Numerics
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix
open MathNet.Numerics.Random
open BT
open FuncTable

type RingBuildRandMemParams = {
    MemSize : int
    MemCount : int
    RandSeed : int
}

type RingGroupRandInitParams = {
    GroupSize : int
    GroupCount : int
    RandSeed : int
    Sigma : float32
}

type RingUpdateParams = {
    NodeGroupSynapses : Matrix<float32>
    Gain : float32
    StepSize : float32
    RandSeed : int
    NoiseLevel : float32
    Epochs : int
}

type GenRingUpdateParams = {
    GroupSize : int
    GroupCount : int
    MemCount : int
    RandSeed : int
    ClipFrac : float32
    Gain : float32
    StepSize : float32
    NoiseLevel : float32
    Epochs : int
}

type RingNetworkTr = {
    NodeGroups : Matrix<float32>
    NodeGroupGradient : Option<Matrix<float32>>
    NodeGroupDeltas : Option<Matrix<float32>>
}

module RingE =

    let Update (synapses:Matrix<float32>)
               (arcTanh:ArcTanh)
               (stepSize:float32)
               (sNoise: seq<float32>)
               (nodeGroups:Matrix<float32>) =

        let nodeGroupsT = nodeGroups.Map(fun v -> arcTanh.GetValue(v))
        let gradient = synapses.Multiply(nodeGroupsT)
        let noiseArray = 
            sNoise |> Seq.take (nodeGroups.RowCount * nodeGroups.ColumnCount)
                   |> Seq.toArray

        DenseMatrix.init 
            nodeGroups.RowCount nodeGroups.ColumnCount
            (fun x y -> 
                NumUt.ToSF(
                    nodeGroups.[x,y] + 
                    gradient.[x,y] * stepSize + 
                    noiseArray.[nodeGroups.ColumnCount*x + y]
                    )
            )


    let UpdateTr (synapses:Matrix<float32>)
                 (arcTanh:ArcTanh)
                 (sNoise: seq<float32>)
                 (stepSize:float32)
                 (nodeGroups:Matrix<float32>) =

        let nodeGroupsT = nodeGroups.Map(fun v -> arcTanh.GetValue(v))
        let gradient = synapses.Multiply(nodeGroupsT)
        let noiseArray = 
            sNoise |> Seq.take (nodeGroups.RowCount * nodeGroups.ColumnCount)
                   |> Seq.toArray

        let deltas = DenseMatrix.init 
                        nodeGroups.RowCount nodeGroups.ColumnCount
                        (fun x y -> 
                            NumUt.ToSF( gradient.[x,y] * stepSize + 
                                  noiseArray.[nodeGroups.ColumnCount*x + y]
                                )
                        )

        { 
            RingNetworkTr.NodeGroups = nodeGroups.Map2((fun a b -> NumUt.ToSF(a + b)), deltas);
            NodeGroupGradient = Some gradient;
            NodeGroupDeltas = Some deltas
        }


    let UpdateEpoch (nodeGroups:Matrix<float32>) 
                    (ringParameters:RingUpdateParams) =

        let arcT = new ArcTanh(ringParameters.Gain, 1000)
        let rng = Random.MersenneTwister(ringParameters.RandSeed)
        let seqNoise = GenS.SeqOfUniformRandF32 ringParameters.NoiseLevel
                                               -ringParameters.NoiseLevel
                                                rng
        let CurriedNext = Update ringParameters.NodeGroupSynapses
                                 arcT
                                 ringParameters.StepSize

        let rec Ura (nodeGroups:Matrix<float32>) iter =
            match iter with
            | 0 -> UpdateTr ringParameters.NodeGroupSynapses
                            arcT
                            seqNoise
                            ringParameters.StepSize
                            nodeGroups
            | _ -> Ura (CurriedNext seqNoise nodeGroups) (iter - 1)

        Ura nodeGroups ringParameters.Epochs


module RingEbuilders =

    let CreateRandomRingGroup (ringGroupRandInitParams:RingGroupRandInitParams) =
        let rng = Random.MersenneTwister(ringGroupRandInitParams.RandSeed)
        let temp = GenS.NormalSF32 rng 0.0f ringGroupRandInitParams.Sigma
                       |> Seq.take(ringGroupRandInitParams.GroupSize * ringGroupRandInitParams.GroupCount) 
                       |> Seq.toArray
        DenseMatrix.init ringGroupRandInitParams.GroupSize ringGroupRandInitParams.GroupCount 
                    (fun x y -> temp.[x*ringGroupRandInitParams.GroupCount + y])


    let CreateRandomMems (ringBuildRandMemParams:RingBuildRandMemParams) =
        let seq = (GenS.SeqOfRandBUF32 0.5f (GenV.Twist(ringBuildRandMemParams.RandSeed)))
        MatrixUt.DenseFromSeq ringBuildRandMemParams.MemSize
                ringBuildRandMemParams.MemCount 
                seq


    let CreateSynapses (createSynapseParams:CreateSynapseParams) =
        let sm = createSynapseParams.Memories
                    .Multiply(createSynapseParams.Memories.Transpose())
        createSynapseParams.Memories
                           .Multiply(createSynapseParams.Memories.Transpose())
                  |> MatrixUt.ToUniformDiag 0.0f
                  |> MatrixUt.ClipSF32 createSynapseParams.ClipFrac



    let MemCorr (memories:Matrix<float32>) (states:Matrix<float32>) =
        memories.Transpose().Multiply(states)


    let MakeRingUpdateParams (genRingUpdateParams:GenRingUpdateParams) =
        let randMems = 
            CreateRandomMems
             {
                RingBuildRandMemParams.MemSize = genRingUpdateParams.GroupSize
                RingBuildRandMemParams.MemCount = genRingUpdateParams.MemCount
                RingBuildRandMemParams.RandSeed = genRingUpdateParams.RandSeed
             }

        let synapses =
            CreateSynapses
             {
                CreateSynapseParams.Memories = randMems
                CreateSynapseParams.ClipFrac = genRingUpdateParams.ClipFrac
             }

        ({
            RingUpdateParams.NodeGroupSynapses = synapses
            Gain = genRingUpdateParams.Gain
            StepSize = genRingUpdateParams.StepSize
            RandSeed = genRingUpdateParams.RandSeed
            NoiseLevel = genRingUpdateParams.NoiseLevel
            Epochs = genRingUpdateParams.Epochs
        },
        randMems)


    let IterateNoiseSeed (ringUpdateParams:RingUpdateParams) =
        let rng = Random.MersenneTwister(ringUpdateParams.RandSeed)
        {ringUpdateParams with RandSeed = rng.Next()}
