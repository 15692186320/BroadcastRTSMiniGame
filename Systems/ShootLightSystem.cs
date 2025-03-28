﻿using Unity.Burst;
using Unity.Entities;

partial struct ShootLightSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        foreach ((RefRW<ShootLight> shootLight,Entity entity) in SystemAPI.Query<RefRW<ShootLight>>().WithEntityAccess())
        {
            shootLight.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (shootLight.ValueRO.timer<=0f)
            {
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
