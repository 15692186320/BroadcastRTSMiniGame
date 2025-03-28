using RVO;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;


[UpdateAfter(typeof(MeleeAttackSystem))]
partial struct AgentSyncSystem : ISystem
{
    private Simulator simulator;
    private NativeHashMap<Entity, int> entityToAgentIndex;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        Simulator.Instance.setTimeStep(SystemAPI.Time.DeltaTime);
        Simulator.Instance.setAgentDefaults(
            5f,   // neighborDist
            10,   // maxNeighbors
            1.5f, // timeHorizon
            2f,   // timeHorizonObst
            0.5f, // radius
            2f,   // maxSpeed
            new Vector2(0, 0) // velocity
        );
        entityToAgentIndex = new NativeHashMap<Entity, int>(1000,Allocator.Persistent);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        Simulator.Instance.Clear();
        entityToAgentIndex.Dispose();
    }
}
