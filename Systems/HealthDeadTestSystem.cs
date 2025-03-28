using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthDeadTestSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
       
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((RefRW<Health> health,
            Entity entity) 
            in SystemAPI.Query<
                RefRW<Health>>()
                .WithEntityAccess())
        {
            if (health.ValueRO.healthAmount <=0)
            {
                health.ValueRW.onDead = true;
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
      //  entityCommandBuffer.Playback(state.EntityManager);

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
