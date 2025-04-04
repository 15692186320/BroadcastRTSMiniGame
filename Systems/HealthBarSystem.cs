﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthBarSystem : ISystem
{
                private ComponentLookup<LocalTransform> localTransformComponentLookup;
                private ComponentLookup<Health> healthComponentLookup;
                private ComponentLookup<PostTransformMatrix> postTransformMatrixComponentLookup;
    public void OnCreate(ref SystemState state)
    {
        localTransformComponentLookup = state.GetComponentLookup<LocalTransform>();
        healthComponentLookup = state.GetComponentLookup<Health>(true);
        postTransformMatrixComponentLookup = state.GetComponentLookup<PostTransformMatrix>();
    }



   // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        localTransformComponentLookup.Update(ref state);
        healthComponentLookup.Update(ref state) ;
        postTransformMatrixComponentLookup.Update(ref state);




        Vector3 cameraForward = Vector3.zero;

        if (Camera.main!=null)
        {
            cameraForward = Camera.main.transform.forward;
        }

        HealthBarJob healthBarJob = new HealthBarJob
        {
            cameraForward=cameraForward,
            localTransformComponentLookup = localTransformComponentLookup,
            healthComponentLookup = healthComponentLookup,
            postTransformMatrixComponentLookup = postTransformMatrixComponentLookup,
        };

        healthBarJob.ScheduleParallel();


        //foreach ((RefRW<LocalTransform>localTransform ,RefRO<HealthBar> healthBar) in SystemAPI.Query<RefRW<LocalTransform>,RefRO<HealthBar>>())
        //{
        //    LocalTransform parentLocalTransform = SystemAPI.GetComponent<LocalTransform>(healthBar.ValueRO.healthEntity);

        //    if (localTransform.ValueRO.Scale == 1f)
        //    {
        //        localTransform.ValueRW.Rotation = parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
        //    }
           


        //    Health health = SystemAPI.GetComponent<Health>(healthBar.ValueRO.healthEntity);

        //    if (!health.onHealthChanged)
        //    {
        //        continue;
        //    }

        //    float healthNormalized = (float)health.healthAmount / health.healthAmountMax;

        //    if (healthNormalized == 1f)
        //    {
        //        localTransform.ValueRW.Scale = 0f;
        //    }
        //    else
        //    {
        //        localTransform.ValueRW.Scale = 1f;
        //    }

        //   RefRW<PostTransformMatrix> barVisualPostTransformMatrix = SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barVisualEntity);

        //    barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalized,1,1);
        //}
    }


}


[BurstCompile]
public partial struct HealthBarJob : IJobEntity
{
    public float3 cameraForward;
    [ReadOnly] public ComponentLookup<Health> healthComponentLookup;

    [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> localTransformComponentLookup;
    //在这里确保healthbar不会互相读写数据 可以使用此标签放开安全限制
    [NativeDisableParallelForRestriction] public ComponentLookup<PostTransformMatrix> postTransformMatrixComponentLookup;
    public void Execute(in HealthBar healthBar,Entity entity)
    {
        RefRW<LocalTransform> localTransform = localTransformComponentLookup.GetRefRW(entity);

        LocalTransform parentLocalTransform = localTransformComponentLookup[healthBar.healthEntity];

        if (localTransform.ValueRO.Scale == 1f)
        {
            localTransform.ValueRW.Rotation = parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
        }



        Health health = healthComponentLookup[healthBar.healthEntity];

        if (!health.onHealthChanged)
        {
            return;
        }

        float healthNormalized = (float)health.healthAmount / health.healthAmountMax;

        if (healthNormalized == 1f)
        {
            localTransform.ValueRW.Scale = 0f;
        }
        else
        {
            localTransform.ValueRW.Scale = 1f;
        }

        RefRW<PostTransformMatrix> barVisualPostTransformMatrix = postTransformMatrixComponentLookup.GetRefRW(healthBar.barVisualEntity);

        barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalized, 1, 1);
    }
}
