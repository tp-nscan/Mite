namespace TT
open MathNet.Numerics
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix
open MathNet.Numerics.Random
open BT
open FuncTable

type CliqueBuildRandMemParams = {
    MemSize : int
    MemCount : int
    RandSeed : int
}

type CliqueGroupRandInitParams = {
    GroupSize : int
    GroupCount : int
    RandSeed : int
    Sigma : float32
}

type CliqueGroupReplicaParams = {
    Memories : Matrix<float32>
    ReplicaCount : int
    RandSeed : int
    ReplicaDistance : float32
}

type CreateSynapseParams = {
    Memories : Matrix<float32>
    ClipFrac : float32
}

type CliqueUpdateParams = {
    NodeGroupSynapses : Matrix<float32>
    Gain : float32
    StepSize : float32
    RandSeed : int
    NoiseLevel : float32
    Epochs : int
}

type GenCliqueUpdateParams = {
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

type CliqueNetworkTr = {
    NodeGroups : Matrix<float32>
    NodeGroupGradient : Option<Matrix<float32>>
    NodeGroupDeltas : Option<Matrix<float32>>
}


module CliqueE =

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
                            NumUt.ToSF(
                                gradient.[x,y] * stepSize + 
                                noiseArray.[nodeGroups.ColumnCount*x + y]
                                )
                        )

        { 
            CliqueNetworkTr.NodeGroups = nodeGroups.Map2((fun a b -> NumUt.ToSF(a + b)), deltas);
            NodeGroupGradient = Some gradient;
            NodeGroupDeltas = Some deltas
        }


    let UpdateEpoch 
            (nodeGroups:Matrix<float32>) 
            (cliqueParameters:CliqueUpdateParams) =

        let arcT = new ArcTanh(cliqueParameters.Gain, 1000)
        let rng = Random.MersenneTwister(cliqueParameters.RandSeed)
        let seqNoise = GenS.SeqOfUniformRandF32 -cliqueParameters.NoiseLevel 
                                                 cliqueParameters.NoiseLevel 
                                                 rng
        let CurriedNext =
                Update cliqueParameters.NodeGroupSynapses
                       arcT
                       cliqueParameters.StepSize

        let rec Ura (nodeGroups:Matrix<float32>) iter =
            match iter with
            | 0 -> UpdateTr cliqueParameters.NodeGroupSynapses
                            arcT
                            seqNoise
                            cliqueParameters.StepSize
                            nodeGroups
            | _ -> Ura (CurriedNext seqNoise nodeGroups) (iter - 1)

        Ura nodeGroups cliqueParameters.Epochs


module CliqueEbuilders =

    let CreateRandomCliqueGroup (cliqueGroupRandInitParams:CliqueGroupRandInitParams) =
        let rng = Random.MersenneTwister(cliqueGroupRandInitParams.RandSeed)
        let temp = GenS.NormalSF32 rng 0.0f cliqueGroupRandInitParams.Sigma
                       |> Seq.take(cliqueGroupRandInitParams.GroupSize * cliqueGroupRandInitParams.GroupCount) 
                       |> Seq.toArray
        DenseMatrix.init cliqueGroupRandInitParams.GroupSize cliqueGroupRandInitParams.GroupCount 
                    (fun x y -> temp.[x*cliqueGroupRandInitParams.GroupCount + y])


    let CreateRandomMems (cliqueBuildRandMemParams:CliqueBuildRandMemParams) =
        let seq = (GenS.SeqOfRandBUF32 0.5f (GenV.Twist(cliqueBuildRandMemParams.RandSeed)))
        MatrixUt.DenseFromSeq cliqueBuildRandMemParams.MemSize
                cliqueBuildRandMemParams.MemCount 
                seq


    let MakeFirstCliqueReplicas(cliqueGroupReplicaParams:CliqueGroupReplicaParams) =
        let perturbs = GenS.SeqOfUniformRandF32 -cliqueGroupReplicaParams.ReplicaDistance
                                                 cliqueGroupReplicaParams.ReplicaDistance
                                                (GenV.Twist(cliqueGroupReplicaParams.RandSeed))
        MatrixUt.MutatedCopies
            NumUt.ToSF
            (cliqueGroupReplicaParams.Memories.Column(0).ToColumnMatrix())
            cliqueGroupReplicaParams.ReplicaCount
            perturbs


    let MakeCliqueGroupReplicas(cliqueGroupReplicaParams:CliqueGroupReplicaParams) =
        let perturbs = GenS.SeqOfUniformRandF32 -cliqueGroupReplicaParams.ReplicaDistance
                                                 cliqueGroupReplicaParams.ReplicaDistance
                                                (GenV.Twist(cliqueGroupReplicaParams.RandSeed))
        MatrixUt.MutatedCopies
            NumUt.ToSF
            cliqueGroupReplicaParams.Memories
            cliqueGroupReplicaParams.ReplicaCount
            perturbs


    let CreateSynapses (createSynapseParams:CreateSynapseParams) =
        let sm = createSynapseParams.Memories
                                    .Multiply(createSynapseParams.Memories.Transpose())
        createSynapseParams.Memories
                           .Multiply(createSynapseParams.Memories.Transpose())
                              |> MatrixUt.ToUniformDiag 0.0f
                              |> MatrixUt.ClipSF32 createSynapseParams.ClipFrac



    let MemCorr (memories:Matrix<float32>) (states:Matrix<float32>) =
        memories.Transpose().Multiply(states)


    let MakeCliqueUpdateParams (genCliqueUpdateParams:GenCliqueUpdateParams) =
        let randMems = 
            CreateRandomMems
             {
                CliqueBuildRandMemParams.MemSize = genCliqueUpdateParams.GroupSize
                CliqueBuildRandMemParams.MemCount = genCliqueUpdateParams.MemCount
                CliqueBuildRandMemParams.RandSeed = genCliqueUpdateParams.RandSeed
             }

        let synapses =
            CreateSynapses
             {
                CreateSynapseParams.Memories = randMems
                CreateSynapseParams.ClipFrac = genCliqueUpdateParams.ClipFrac
             }

        ({
            CliqueUpdateParams.NodeGroupSynapses = synapses
            Gain = genCliqueUpdateParams.Gain
            StepSize = genCliqueUpdateParams.StepSize
            RandSeed = genCliqueUpdateParams.RandSeed
            NoiseLevel = genCliqueUpdateParams.NoiseLevel
            Epochs = genCliqueUpdateParams.Epochs
        },
        randMems)


    let IterateNoiseSeed (cliqueUpdateParams:CliqueUpdateParams) =
        let rng = Random.MersenneTwister(cliqueUpdateParams.RandSeed)
        {cliqueUpdateParams with RandSeed = rng.Next()}
