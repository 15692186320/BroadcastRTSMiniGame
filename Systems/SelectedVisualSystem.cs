﻿using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateBefore(typeof(ResetEventSystem))]
partial struct SelectedVisualSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>().WithPresent<Selected>())
        {

            if (selected.ValueRO.onDeselected)
            {
                RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                visualLocalTransform.ValueRW.Scale = 0f;
            }

            if (selected.ValueRO.onSelected)
            {
                RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                visualLocalTransform.ValueRW.Scale = selected.ValueRO.showScale;
            }







            //foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>().WithDisabled<Selected>())
            //{
            //    RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
            //    visualLocalTransform.ValueRW.Scale =0f;
            //}


            //foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>())
            //{
            //   RefRW<LocalTransform> visualLocalTransform =  SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
            //    visualLocalTransform.ValueRW.Scale = selected.ValueRO.showScale;
            //}
        }


        foreach ((RefRO<Selected> selected,RefRW<BuildingBarracks> buildingBarracks) in SystemAPI.Query<RefRO<Selected>,RefRW<BuildingBarracks>>().WithPresent<Selected>())
        {

            if (selected.ValueRO.onDeselected)
            {
                RefRW<LocalTransform> rallyLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(buildingBarracks.ValueRO.rallyTargetPrefabEntity);
                rallyLocalTransform.ValueRW.Scale = 0f;
            }

            if (selected.ValueRO.onSelected)
            {
                RefRW<LocalTransform> rallyLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(buildingBarracks.ValueRO.rallyTargetPrefabEntity);

                rallyLocalTransform.ValueRW.Scale =1;

            }
        }
    }
}
