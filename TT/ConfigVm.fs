namespace TT

type CreateCliqueEa = {
    ClipFrac : float32
    Gain : float32
    GroupCount : int
    MemSize : int
    MemCount : int
    NoiseLevel : float32
    RandSeed : int
    StepSize : float32
    Sigma : float32
}

type CreateCliqueEb = {
    ClipFrac : float32
    Gain : float32
    MemSize : int
    ReplicaCount : int
    MemCount : int
    NoiseLevel : float32
    ReplicaDistance : float32
    RandSeed : int
    StepSize : float32
    Sigma : float32
}

type ConfigCliqueE = {
    Gain : float32
    NoiseLevel : float32
    StepSize : float32
}

module CliqueVmConfigs =

    let DefaultCreateCliqueEa =
        {
            CreateCliqueEa.MemSize = 32
            GroupCount = 64
            MemCount = 16
            RandSeed = 2122
            ClipFrac = 0.9f
            Gain = 2.1f
            StepSize = 0.01f
            NoiseLevel = 0.1f
            Sigma = 0.5f
        }

    let DefaultCreateCliqueEb =
        {
            CreateCliqueEb.ReplicaCount = 2
            MemSize = 32
            MemCount = 16
            RandSeed = 2122
            ClipFrac = 0.9f
            Gain = 2.1f
            StepSize = 0.01f
            NoiseLevel = 0.1f
            ReplicaDistance = 0.1f
            Sigma = 0.5f
        }

    let DefaultCreateCliqueEs =
         {
            CreateCliqueEb.ReplicaCount = 20
            MemSize = 32
            MemCount = 3
            RandSeed = 2122
            ClipFrac = 0.9f
            Gain = 2.1f
            StepSize = 0.01f
            NoiseLevel = 0.1f
            ReplicaDistance = 0.1f
            Sigma = 0.5f
         }