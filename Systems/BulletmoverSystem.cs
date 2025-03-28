using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BulletmoverSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((RefRW<LocalTransform> localtransform,
                        RefRO<Bullet> bullet,
                        RefRO<Target> target,
                        Entity entity) 
                        in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRO<Bullet>,
                RefRO<Target>>().WithEntityAccess())
        {
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }

           LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            ShootVictim targetShootVictim = SystemAPI.GetComponent<ShootVictim>(target.ValueRO.targetEntity);

            float3 targetPosition = targetLocalTransform.TransformPoint(targetShootVictim.hitLocalPosition);

            float distanceBeforeSq = math.distancesq(localtransform.ValueRO.Position, targetPosition);

            float3 moveDirection =  targetPosition - localtransform.ValueRO.Position;

            moveDirection = math.normalize(moveDirection);

            localtransform.ValueRW.Position += moveDirection * bullet.ValueRO.speed * SystemAPI.Time.DeltaTime;

            float distanceAfterSq = math.distancesq(localtransform.ValueRO.Position, targetPosition);

            if (distanceAfterSq> distanceBeforeSq)
            {
                localtransform.ValueRW.Position = targetPosition;
            }


            float destoryDistanceSq = .2f;

            if (math.distancesq(localtransform.ValueRO.Position, targetPosition) <destoryDistanceSq)
            {
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;
                targetHealth.ValueRW.onHealthChanged = true;
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
