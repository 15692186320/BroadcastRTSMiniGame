using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct ShootAttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReferences>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
       EntitiesReferences entitiesReferences =  SystemAPI.GetSingleton<EntitiesReferences>();
        foreach ((RefRW<LocalTransform> localTransform, 
                        RefRW<ShootAttack> shootAttack,
                        RefRO<Target>target,
                        RefRW<UnitMover> unitMover,
                        Entity entity)  in 
                SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<ShootAttack>,
                RefRO<Target>,
                RefRW<UnitMover>>().WithDisabled<MoveOverride>().WithEntityAccess())
        {
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                continue;
            }



            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            if (math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position) > shootAttack.ValueRO.attackDistance)
            {
                unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
                continue;
            }
            else
            {
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
            }

            float3 aimDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;
            aimDirection = math.normalize(aimDirection);
            quaternion targetRotation = quaternion.LookRotation(aimDirection,math.up());
            //Debug.Log(targetRotation);
            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, targetRotation, SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);

          
        }


        //炮台逻辑



        foreach ((RefRW<LocalTransform> localTransform,
                       RefRW<ShootAttack> shootAttack,
                       RefRO<Target> target,
                       Entity entity) in
               SystemAPI.Query<
               RefRW<LocalTransform>,
               RefRW<ShootAttack>,
               RefRO<Target>>().WithEntityAccess())
        {
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                continue;
            }



            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);

            if (math.distance(localTransform.ValueRO.Position,targetLocalTransform.Position)> shootAttack.ValueRO.attackDistance)
            {
                continue;
            }

            if (SystemAPI.HasComponent<MoveOverride>(entity) && SystemAPI.IsComponentEnabled<MoveOverride>(entity))
            {
                continue;
            }


            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (shootAttack.ValueRO.timer > 0f)
            {
                continue;
            }
            shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;

            if (SystemAPI.HasComponent<TargetOverride>(target.ValueRO.targetEntity))
            {

                RefRW<TargetOverride> enemyTargetOverride = SystemAPI.GetComponentRW<TargetOverride>(target.ValueRO.targetEntity);

                if (enemyTargetOverride.ValueRO.targetEntity == Entity.Null)
                {
                    enemyTargetOverride.ValueRW.targetEntity = entity;
                }
            }

            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletPrefabEntity);
           LocalToWorld wroldPosition = SystemAPI.GetComponent<LocalToWorld>(entity);

            

            float3 bulletSpawnWorldPosition =math.transform(wroldPosition.Value,shootAttack.ValueRO.bulletspawnLocalPosition);
            //Debug.Log(" localTransform" + localTransform.ValueRO.Position);
            //Debug.Log("shootAttack.ValueRO.bulletspawnLocalPosition" + shootAttack.ValueRO.bulletspawnLocalPosition);
            //Debug.Log("bulletSpawnWorldPosition" + bulletSpawnWorldPosition);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(bulletSpawnWorldPosition));

            RefRW<Bullet> bulletBullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bulletBullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;

            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;

            shootAttack.ValueRW.onShoot.isTrigger = true;
            shootAttack.ValueRW.onShoot.shootFromPosition = bulletSpawnWorldPosition;
        }




    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
