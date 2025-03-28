using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;


[UpdateAfter(typeof(ShootAttackSystem))]
partial struct AnimationStateSystem : ISystem
{
    private  ComponentLookup<ActiveAnimation> activeAnimationComponentLookUp;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        activeAnimationComponentLookUp = state.GetComponentLookup<ActiveAnimation>(false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        activeAnimationComponentLookUp.Update(ref state);
        IdleWalkingAnimationStateJob idleWalkingAnimationStateJob = new IdleWalkingAnimationStateJob { 
        activeAnimationComponentLookUp = activeAnimationComponentLookUp,
        };
        idleWalkingAnimationStateJob.ScheduleParallel();


        activeAnimationComponentLookUp.Update(ref state);
        ShootAttackAnimationStateJob shootAttackAnimationStateJob = new ShootAttackAnimationStateJob
        {
            activeAnimationComponentLookUp = activeAnimationComponentLookUp,
        };
        shootAttackAnimationStateJob.ScheduleParallel();


        activeAnimationComponentLookUp.Update(ref state);
        MeleeAttackAnimationStateJob meleeAttackAnimationStateJob = new MeleeAttackAnimationStateJob
        {
            activeAnimationComponentLookUp = activeAnimationComponentLookUp,
        };
        meleeAttackAnimationStateJob.ScheduleParallel();

        //foreach ((RefRO<AnimatedMesh> animatedMesh,
        //               RefRO<UnitMover> unitMover,
        //               RefRO<UnitAnimation>unitAnimation) in 
        //    SystemAPI.Query<RefRO<AnimatedMesh>,
        //                                 RefRO<UnitMover>,
        //                                 RefRO<UnitAnimation>>())
        //{
        //    RefRW<ActiveAnimation> activeAnimation = SystemAPI.GetComponentRW<ActiveAnimation>(animatedMesh.ValueRO.meshEntity);
        //    if (unitMover.ValueRO.isMoving)
        //    {
        //        activeAnimation.ValueRW.nextAnimationType = unitAnimation.ValueRO.walkAnimation;
        //    }
        //    else
        //    {
        //        activeAnimation.ValueRW.nextAnimationType = unitAnimation.ValueRO.idleAnimation;
        //    }
        //}

        //foreach ((RefRO<AnimatedMesh> animatedMesh,
        //               RefRO<ShootAttack> shootAttack,
        //               RefRO<UnitAnimation> unitAnimation) in
        //    SystemAPI.Query<RefRO<AnimatedMesh>,
        //                                 RefRO<ShootAttack>,
        //                                 RefRO<UnitAnimation>>())
        //{
        //    if (shootAttack.ValueRO.onShoot.isTrigger)
        //    {
        //        RefRW<ActiveAnimation> activeAnimation = SystemAPI.GetComponentRW<ActiveAnimation>(animatedMesh.ValueRO.meshEntity);
        //        activeAnimation.ValueRW.nextAnimationType = unitAnimation.ValueRO.shootAnimation;
        //    }
        //}


        //        foreach ((RefRO<AnimatedMesh> animatedMesh,
        //       RefRO<MeleeAttack> meleeAttack,
        //       RefRO<UnitAnimation> unitAnimation) in
        //SystemAPI.Query<RefRO<AnimatedMesh>,
        //                         RefRO<MeleeAttack>,
        //                         RefRO<UnitAnimation>>())
        //        {
        //            if (meleeAttack.ValueRO.onAttacked)
        //            {
        //                RefRW<ActiveAnimation> activeAnimation = SystemAPI.GetComponentRW<ActiveAnimation>(animatedMesh.ValueRO.meshEntity);
        //                activeAnimation.ValueRW.nextAnimationType = AnimationDataSO.AnimationType.zombieAttack;
        //                //Debug.Log(activeAnimation.ValueRW.nextAnimationType + "+"+ unitAnimation.ValueRO.zombieAttack.ToString());
        //            }
        //        }


    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}


[BurstCompile]
public partial struct IdleWalkingAnimationStateJob : IJobEntity
{
    [NativeDisableParallelForRestriction]public ComponentLookup<ActiveAnimation> activeAnimationComponentLookUp;
    public void Execute(in AnimatedMesh animatedMesh,in UnitMover unitMover,in UnitAnimation unitAnimation)
    {
      RefRW<ActiveAnimation>  activeAnimation = activeAnimationComponentLookUp.GetRefRW(animatedMesh.meshEntity);

        if (unitMover.isMoving)
        {
            activeAnimation.ValueRW.nextAnimationType = unitAnimation.walkAnimation;
        }
        else
        {
            activeAnimation.ValueRW.nextAnimationType = unitAnimation.idleAnimation;
        }
    }
}

[BurstCompile]
public partial struct ShootAttackAnimationStateJob : IJobEntity
{
    [NativeDisableParallelForRestriction] public ComponentLookup<ActiveAnimation> activeAnimationComponentLookUp;
    public void Execute(in AnimatedMesh animatedMesh,in ShootAttack shootAttack,in UnitAnimation unitAnimation)
    {
        if (shootAttack.onShoot.isTrigger)
        {
            RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookUp.GetRefRW(animatedMesh.meshEntity);
            activeAnimation.ValueRW.nextAnimationType = unitAnimation.shootAnimation;
        }
    }
}

[BurstCompile]
public partial struct MeleeAttackAnimationStateJob : IJobEntity
{
    [NativeDisableParallelForRestriction] public ComponentLookup<ActiveAnimation> activeAnimationComponentLookUp;
    public void Execute(in AnimatedMesh animatedMesh,in MeleeAttack meleeAttack)
    {
        if (meleeAttack.onAttacked)
        {
            RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookUp.GetRefRW(animatedMesh.meshEntity);
            activeAnimation.ValueRW.nextAnimationType = AnimationDataSO.AnimationType.zombieAttack;
            //Debug.Log(activeAnimation.ValueRW.nextAnimationType + "+"+ unitAnimation.ValueRO.zombieAttack.ToString());
        }
    }
}
