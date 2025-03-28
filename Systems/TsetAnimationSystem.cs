using GPUECSAnimationBaker.Engine.AnimatorSystem;
using JetBrains.Annotations;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

partial struct TsetAnimationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //foreach ((RefRW<ActiveAnimation> activeAnimation, Entity entity) in SystemAPI.Query<RefRW<ActiveAnimation>>().WithEntityAccess())
        //{
        //    if (activeAnimation.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.zombieWalk)
        //    {
        //        SystemAPI.GetAspect<GpuEcsAnimatorAspect>(entity).RunAnimation(0, transitionSpeed: .1f);
        //        continue;
        //    }

        //    if (activeAnimation.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.zombieAttack)
        //    {
        //        SystemAPI.GetAspect<GpuEcsAnimatorAspect>(entity).RunAnimation(1,speedFactor:5f, transitionSpeed: .1f);
        //        continue;
        //    }
        //    SystemAPI.GetAspect<GpuEcsAnimatorAspect>(entity).RunAnimation((int)activeAnimation.ValueRO.activeAnimationType, speedFactor:2f,transitionSpeed: .1f);
        //}
        var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

        ActiveAnimationJob activeAnimationJob = new ActiveAnimationJob();
        JobHandle animationJobHandle = activeAnimationJob.ScheduleParallel(state.Dependency);
        //state.Dependency = animationJobHandle;
        HandleAnimationEventsJob eventsJob = new HandleAnimationEventsJob { 
        ECB = ecb.AsParallelWriter()
        };
        JobHandle eventsJobHandle = eventsJob.ScheduleParallel(animationJobHandle);

            state.Dependency = eventsJobHandle;
       
        //foreach ((DynamicBuffer<GpuEcsAnimatorEventBufferElement> animatorEventBuffer, Entity entity) in SystemAPI.Query<DynamicBuffer<GpuEcsAnimatorEventBufferElement>>().WithEntityAccess())
        //{

        //    if (animatorEventBuffer.IsEmpty)
        //    {
        //        continue;
        //    }

        //    RefRW<ActiveAnimation> activeAnimation = SystemAPI.GetComponentRW<ActiveAnimation>(entity);

        //    switch (animatorEventBuffer[0].eventId)
        //    {
        //        case 1:

        //            if (activeAnimation.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.FiringRifle)
        //            {

        //                //Debug.Log(animatorEventBuffer[0].eventId);

        //                animatorEventBuffer.Clear();

        //                activeAnimation.ValueRW.activeAnimationType = AnimationDataSO.AnimationType.None;
        //            }
        //            break;

        //        case 2:

        //            if (activeAnimation.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.zombieAttack)
        //            {

        //                //Debug.Log(animatorEventBuffer[0].eventId);

        //                animatorEventBuffer.Clear();

        //                activeAnimation.ValueRW.activeAnimationType = AnimationDataSO.AnimationType.zombieWalk;
        //            }
        //            break;

        //    }
        //    RefRW<ActiveAnimation> activeAnimation = SystemAPI.GetComponentRW<ActiveAnimation>(entity);

        //    if (!animatorEventBuffer.IsEmpty && activeAnimation.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.FiringRifle)
        //    {

        //        Debug.Log(animatorEventBuffer[0].eventId);

        //        animatorEventBuffer.Clear();

        //        activeAnimation.ValueRW.activeAnimationType = AnimationDataSO.AnimationType.None;
        //    }




        //}
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}

[BurstCompile]
public partial struct ActiveAnimationJob : IJobEntity
{
    public void Execute(GpuEcsAnimatorAspect gpuEcsAnimatorAspect,in ActiveAnimation activeAnimation)
    {
       
        if (activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.zombieWalk)
        {
            gpuEcsAnimatorAspect.RunAnimation(0, speedFactor: 5f, transitionSpeed: .1f);
            return;
        }

        if (activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.zombieAttack)
        {
            gpuEcsAnimatorAspect.RunAnimation(1, speedFactor: 5f, transitionSpeed: .1f);
            return;
        }

        if (activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.Hurricane)
        {
            gpuEcsAnimatorAspect.RunAnimation(0, speedFactor: 1.7f, transitionSpeed: .1f);
            return;
        }

        if (activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.HappyIdle)
        {
            gpuEcsAnimatorAspect.RunAnimation(1, speedFactor: 2f, transitionSpeed: .1f);
            return;
        }

        if (activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.Fastrun)
        {
            gpuEcsAnimatorAspect.RunAnimation(2, speedFactor: 1f, transitionSpeed: .1f);
            return;
        }
        gpuEcsAnimatorAspect.RunAnimation((int)activeAnimation.activeAnimationType, speedFactor: 2f, transitionSpeed: .1f);
    }
}


[BurstCompile]
public partial struct HandleAnimationEventsJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    public void Execute(DynamicBuffer<GpuEcsAnimatorEventBufferElement> animatorEventBuffer, Entity entity, ref ActiveAnimation activeAnimation, [ChunkIndexInQuery] int sortKey)
    {
        if (animatorEventBuffer.IsEmpty)
        {
            return;
        }

        switch (animatorEventBuffer[0].eventId)
        {
            case 0:

                if (activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.FiringRifle)
                {

                    //Debug.Log(animatorEventBuffer[0].eventId);

                    ECB.SetComponent(sortKey, entity, new ActiveAnimation { activeAnimationType = AnimationDataSO.AnimationType.None });

                    ECB.SetBuffer<GpuEcsAnimatorEventBufferElement>(sortKey, entity).Clear();
                }
                break;

            case 1:

                if (activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.Hurricane)
                {

                    //Debug.Log(animatorEventBuffer[0].eventId);

                    ECB.SetComponent(sortKey, entity, new ActiveAnimation { activeAnimationType = AnimationDataSO.AnimationType.HappyIdle });

                    ECB.SetBuffer<GpuEcsAnimatorEventBufferElement>(sortKey, entity).Clear();
                }
                break;



            case 2:

                if (activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.zombieAttack)
                {

                    //Debug.Log(animatorEventBuffer[0].eventId);

                    ECB.SetComponent(sortKey, entity, new ActiveAnimation { activeAnimationType = AnimationDataSO.AnimationType.zombieWalk });
                    ECB.SetBuffer<GpuEcsAnimatorEventBufferElement>(sortKey, entity).Clear();
                }
                break;

        }
    }
}
