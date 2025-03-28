using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BuildingBarracksSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReferences>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach ((RefRW<BuildingBarracks> buildingBarracks,
            DynamicBuffer<SpawnUnitTypeBuffer> spawnUnitTypeBuffer,
            RefRO<BuildingBarracksUnitEnqueue> buildingBarracksUnitEnqueue,
            EnabledRefRW<BuildingBarracksUnitEnqueue> buildingBarracksUnitEnqueueEnabled) 
            in SystemAPI.Query<RefRW<BuildingBarracks>,
            DynamicBuffer<SpawnUnitTypeBuffer>,
            RefRO<BuildingBarracksUnitEnqueue>,
            EnabledRefRW<BuildingBarracksUnitEnqueue>>())
        {
            spawnUnitTypeBuffer.Add(new SpawnUnitTypeBuffer
            {
                unitType = buildingBarracksUnitEnqueue.ValueRO.unitType,

            });
            buildingBarracksUnitEnqueueEnabled.ValueRW = false;

            buildingBarracks.ValueRW.onUnitQueueChanged = true;

        }


        foreach ((RefRO<LocalTransform> localTransform ,
                        RefRW<BuildingBarracks> buildingBarracks,
                        DynamicBuffer<SpawnUnitTypeBuffer> spawnUnitTypeBuffer) in 
                        SystemAPI.Query<RefRO<LocalTransform>,
                        RefRW<BuildingBarracks>,
                        DynamicBuffer<SpawnUnitTypeBuffer>>())
        {
            RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(buildingBarracks.ValueRO.rallyTargetPrefabEntity);

            visualLocalTransform.ValueRW.Position = buildingBarracks.ValueRO.rallyPositionOffset;



            if (spawnUnitTypeBuffer.IsEmpty)
            {
                continue;
            }
            if (buildingBarracks.ValueRO.activeUnitType != spawnUnitTypeBuffer[0].unitType)
            {
                buildingBarracks.ValueRW.activeUnitType = spawnUnitTypeBuffer[0].unitType;
                UnitTypeSO activeUnitTypeSO = GameAssets.Instance.unitTypeListSO.GetUnitTypeSO(buildingBarracks.ValueRO.activeUnitType);
                buildingBarracks.ValueRW.progressMax = activeUnitTypeSO.progessMax;
            }

            buildingBarracks.ValueRW.progress += SystemAPI.Time.DeltaTime;
            if (buildingBarracks.ValueRO.progress < buildingBarracks.ValueRO.progressMax)
            {
                continue;
            }
            buildingBarracks.ValueRW.progress = 0f;

            UnitTypeSO.UnitType unitType = spawnUnitTypeBuffer[0].unitType;

            UnitTypeSO unitTypeSO = GameAssets.Instance.unitTypeListSO.GetUnitTypeSO(unitType);

            spawnUnitTypeBuffer.RemoveAt(0);
            buildingBarracks.ValueRW.onUnitQueueChanged = true;

           Entity spawnedUnitEntity = state.EntityManager.Instantiate(unitTypeSO.GetPrefabEntity(entitiesReferences));

            SystemAPI.SetComponent(spawnedUnitEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));

            Unity.Mathematics.Random random = new Unity.Mathematics.Random((uint)spawnedUnitEntity.Index);
            float angle = random.NextFloat(0, 2 * math.PI); // 随机角度 (0 到 2*PI)
            float radius = random.NextFloat(0, 5f); // 随机半径 (0 到 5)
            float offsetX = radius * math.cos(angle); // X 轴偏移
            float offsetZ = radius * math.sin(angle); // Z 轴偏移
          

            SystemAPI.SetComponent(spawnedUnitEntity, new MoveOverride
            {
                targetPosition = localTransform.ValueRO.Position + buildingBarracks.ValueRO.rallyPositionOffset+new float3(offsetX,0, offsetZ),
            });
            SystemAPI.SetComponentEnabled<MoveOverride>(spawnedUnitEntity, true);


        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}

public struct AgentIndex : IComponentData
{
    public int Value;
}
