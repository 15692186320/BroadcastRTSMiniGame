using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

partial struct ChangeAnimationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        ChangeAnimationJob changeAnimationJob = new ChangeAnimationJob();
        changeAnimationJob.ScheduleParallel();

        //foreach (RefRW<ActiveAnimation> active in SystemAPI.Query<RefRW<ActiveAnimation>>())
        //{

        //    if (active.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.FiringRifle)
        //    {
        //        continue;
        //    }


        //    if (active.ValueRO.activeAnimationType ==AnimationDataSO.AnimationType.zombieAttack)
        //    {
        //        continue;
        //    }

        //    if (active.ValueRO.activeAnimationType != active.ValueRO.nextAnimationType)
        //    {
        //        active.ValueRW.activeAnimationType = active.ValueRO.nextAnimationType;
        //        //SystemAPI.GetAspect<GpuEcsAnimatorAspect>(entity).RunAnimation((int)active.ValueRO.activeAnimationType, transitionSpeed: .1f);
        //    }

        //}
    }
}


[BurstCompile]
public partial struct ChangeAnimationJob : IJobEntity
{
    public void Execute(ref ActiveAnimation activeAnimation)
    {
        if (activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.FiringRifle)
        {
            return;
        }

        if (activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.Hurricane)
        {
            return;
        }


        if (activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.zombieAttack)
        {
            return;
        }

        if (activeAnimation.activeAnimationType != activeAnimation.nextAnimationType)
        {
            activeAnimation.activeAnimationType = activeAnimation.nextAnimationType;
            //SystemAPI.GetAspect<GpuEcsAnimatorAspect>(entity).RunAnimation((int)active.ValueRO.activeAnimationType, transitionSpeed: .1f);
        }
    }

}
