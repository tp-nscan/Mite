namespace TT
open System
open MathNet.Numerics
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix
open MathNet.Numerics.Random
open Rop

type SimReportData =
    | Mem0Corr of Matrix<float32>
    | MemCorrs of Matrix<float32>

module SimRep =
    let ExtractMem0Corr = function
        | Mem0Corr m0c -> m0c
        | MemCorrs mcs -> failwith  "wrong type: MemCorrs"

    let ExtractMemCorrs = function
        | Mem0Corr m0c -> failwith  "wrong type: Mem0Corr"
        | MemCorrs mcs -> mcs

type SimReport = {
    Iteration : int
    Params : obj
    Data : SimReportData
}

type ISim =
   abstract member Iteration : int 
   abstract member ReportFreq : int 
   abstract member Update : unit -> RopResult<ISim,string>
   abstract member SetParams : obj -> RopResult<ISim,string>
   abstract member Params : unit -> obj

module SimUtils =

  type SimTest(iteration:int) =
    let _iteration = iteration
    let _simReportHist = List<SimReport>.Empty

    interface ISim with
        member this.Iteration = 
            _iteration
        member this.ReportFreq = 
            2
        member this.Update() =
           try
             [1..10000] |> List.map(fun t->t*t) |> ignore
             new  SimTest(
                    iteration = _iteration + 1)
                    :> ISim
                    |> Rop.succeed
           with
            | ex -> (sprintf "Error updating SimTest: %s" ex.Message) |> Rop.fail

        member this.SetParams(p) =
           try
             [1..10000] |> List.map(fun t->t*t) |> ignore
             new  SimTest(
                    iteration = _iteration + 1)
                    :> ISim
                    |> Rop.succeed
           with
            | ex -> (sprintf "Error updating SimTest: %s" ex.Message) |> Rop.fail

        member this.Params() = this :> obj


  type SimCliqueSetForEpoch private 
                (CliqueNetworkTr : CliqueNetworkTr,
                 memories : Matrix<float32>,
                 updateParams : CliqueUpdateParams,
                 iteration:int) =

    member this.CliqueNetworkTr = CliqueNetworkTr
    member this.UpdateParams = updateParams
    member this.Iteration = iteration
    member this.Memories = memories

    new(nodeGroups:Matrix<float32>, 
        memories : Matrix<float32>,
        cliqueUpdateParams:CliqueUpdateParams) =
        SimCliqueSetForEpoch
            (
                { 
                    CliqueNetworkTr.NodeGroups = nodeGroups;
                    NodeGroupGradient = None;
                    NodeGroupDeltas = None
                },
                memories,
                cliqueUpdateParams,
                0
            )

    interface ISim with
        member this.Iteration = 
            this.Iteration
        member this.ReportFreq = 
            this.UpdateParams.Epochs
        member this.Update() =
           try
             let newTr = CliqueE.UpdateEpoch this.CliqueNetworkTr.NodeGroups 
                                        this.UpdateParams
             let mem0corr = this.Memories.Column(0)
                              .ToRowMatrix().Multiply(this.CliqueNetworkTr.NodeGroups)
             let newRep = {
                            SimReport.Iteration = this.Iteration;
                            SimReport.Params  = this.UpdateParams;
                            SimReport.Data = SimReportData.Mem0Corr(mem0corr)
                          }

             new SimCliqueSetForEpoch(
                    newTr, 
                    this.Memories,
                    this.UpdateParams,
                    this.Iteration + this.UpdateParams.Epochs)
                    :> ISim
                    |> Rop.succeed
           with
            | ex -> (sprintf "Error updating SimCliqueSetForEpoch: %s" ex.Message) 
                    |> Rop.fail

        member this.SetParams(p) =
           try
             new SimCliqueSetForEpoch(
                    this.CliqueNetworkTr, 
                    this.Memories,
                    p :?> CliqueUpdateParams,
                    this.Iteration)
                    :> ISim
                    |> Rop.succeed
           with
            | ex -> (sprintf "Error SetParams SimCliqueSetForEpoch: %s" ex.Message) |> Rop.fail

        member this.Params() = this.UpdateParams :> obj


  type SimRingSetForEpoch private 
                (RingNetworkTr : RingNetworkTr,
                 memories : Matrix<float32>,
                 updateParams : RingUpdateParams,
                 iteration:int) =

    member this.RingNetworkTr = RingNetworkTr
    member this.UpdateParams = updateParams
    member this.Iteration = iteration
    member this.Memories = memories

    new(nodeGroups:Matrix<float32>, 
        memories : Matrix<float32>,
        ringUpdateParams:RingUpdateParams) =
        SimRingSetForEpoch
            (
                { 
                    RingNetworkTr.NodeGroups = nodeGroups;
                    NodeGroupGradient = None;
                    NodeGroupDeltas = None
                },
                memories,
                ringUpdateParams,
                0
            )

    interface ISim with
        member this.Iteration = 
            this.Iteration
        member this.ReportFreq = 
            this.UpdateParams.Epochs
        member this.Update() =
           try
             let newTr = RingE.UpdateEpoch this.RingNetworkTr.NodeGroups 
                                        this.UpdateParams
             let mem0corr = this.Memories.Column(0)
                              .ToRowMatrix().Multiply(this.RingNetworkTr.NodeGroups)
             let newRep = {
                            SimReport.Iteration = this.Iteration;
                            SimReport.Params  = this.UpdateParams;
                            SimReport.Data = SimReportData.Mem0Corr(mem0corr)
                          }

             new SimRingSetForEpoch(
                    newTr, 
                    this.Memories,
                    this.UpdateParams,
                    this.Iteration + this.UpdateParams.Epochs)
                    :> ISim
                    |> Rop.succeed
           with
            | ex -> (sprintf "Error updating SimRingSetForEpoch: %s" ex.Message) 
                    |> Rop.fail

        member this.SetParams(p) =
           try
             new SimRingSetForEpoch(
                    this.RingNetworkTr, 
                    this.Memories,
                    p :?> RingUpdateParams,
                    this.Iteration)
                    :> ISim
                    |> Rop.succeed
           with
            | ex -> (sprintf "Error SetParams SimRingSetForEpoch: %s" ex.Message) |> Rop.fail

        member this.Params() = this.UpdateParams :> obj