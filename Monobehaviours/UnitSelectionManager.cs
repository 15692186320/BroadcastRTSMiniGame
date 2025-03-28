using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; private set; }

    public event EventHandler OnSelectionAreaStart;
    public event EventHandler OnSelectionAreaEnd;
    public event EventHandler OnSelectedEntitiesChanged;

    private Vector2 selectionStartMousePosition;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!BuildingPlacementManager.Instance.GetActiveBuildingTypeSO().IsNone())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            selectionStartMousePosition = Input.mousePosition;
            OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 selectionEndMousePosition = Input.mousePosition;

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            //重置选中
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);
            for (int i = 0; i < entityArray.Length; i++)
            {
                entityManager.SetComponentEnabled<Selected>(entityArray[i], false);

                Selected selected = selectedArray[i];
                selected.onDeselected = true;
                entityManager.SetComponentData(entityArray[i], selected);
            }

           
            //获取选择区域
            Rect selectionAreaRect = GetSelectionAreaRect();
            //计算选择区域面积
            float selectionAreaSize = selectionAreaRect.width * selectionAreaRect.height;
            //设置最小多选区域
            float multipleSelectionSizeMin = 40f;
            //判断是否多选
            bool isMultipleSelection = selectionAreaSize > multipleSelectionSizeMin;

            if (isMultipleSelection)
            {
                entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithPresent<Selected>().Build(entityManager);
                entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<LocalTransform> localTransformsArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                for (int i = 0; i < localTransformsArray.Length; i++)
                {
                    LocalTransform unitLocalTransform = localTransformsArray[i];
                    Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);
                    if (selectionAreaRect.Contains(unitScreenPosition))
                    {
                        entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
                        Selected selected = entityManager.GetComponentData<Selected>(entityArray[i]);
                        selected.onSelected = true;
                        entityManager.SetComponentData<Selected>(entityArray[i], selected);
                    }
                }
            }
            else
            {
                //两种方法相同
              //  new EntityQueryBuilder(Allocator.Temp).WithAll<PhysicsWorldSingleton>().Build(entityManager);
               entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
                UnityEngine.Ray CameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastInput raycastInput = new RaycastInput
                {
                    Start = CameraRay.GetPoint(0f),
                    End = CameraRay.GetPoint(9999f),
                    Filter = new CollisionFilter
                    {
                        BelongsTo = ~0u,
                        CollidesWith = 1u << GameAssets.UNITS_LAYER | 1u << GameAssets.BUILDINGS_LAYER,
                        GroupIndex = 0,
                    }
                };
                if(collisionWorld.CastRay(raycastInput,out Unity.Physics.RaycastHit raycastHit))
                {
                    if ( entityManager.HasComponent<Selected>(raycastHit.Entity))
                    {
                        entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);
                        Selected selected = entityManager.GetComponentData<Selected>(raycastHit.Entity);
                        selected.onSelected = true;
                        entityManager.SetComponentData(raycastHit.Entity, selected);

                    }
                }
            }

            OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
            OnSelectedEntitiesChanged?.Invoke(this, EventArgs.Empty);   
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
            PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
            UnityEngine.Ray CameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastInput raycastInput = new RaycastInput
            {
                Start = CameraRay.GetPoint(0f),
                End = CameraRay.GetPoint(9999f),
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.UNITS_LAYER | 1u << GameAssets.BUILDINGS_LAYER,
                    GroupIndex = 0,
                }
            };
            bool isAttackingSingleTarget = false;
            if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit))
            {
                if (entityManager.HasComponent<Faction>(raycastHit.Entity))
                {
                    Faction faction = entityManager.GetComponentData<Faction>(raycastHit.Entity);
                    if (faction.factionType == FactionType.Zombie)
                    {
                        isAttackingSingleTarget = true;


                        entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().WithPresent<TargetOverride>().Build(entityManager);
                        //查找所有选中的 存在目标覆盖的

                        NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                        //存入数组
                        NativeArray<TargetOverride> targetOverrideArray = entityQuery.ToComponentDataArray<TargetOverride>(Allocator.Temp);
                        //将实体的目标覆盖组件存入数组
                        for (int i = 0; i < targetOverrideArray.Length; i++)
                        {
                            TargetOverride targetOverride = targetOverrideArray[i];
                            targetOverride.targetEntity = raycastHit.Entity;
                            targetOverrideArray[i] = targetOverride;
                            //将所有选中实体的 目标覆盖组件 的 目标设置为点选的目标
                            entityManager.SetComponentEnabled<MoveOverride>(entityArray[i], false);
                        }
                        entityQuery.CopyFromComponentDataArray(targetOverrideArray);
                    }

                }
            }

            if (!isAttackingSingleTarget)
            {
                    entityQuery =  new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().WithPresent<MoveOverride,TargetOverride>().Build(entityManager);

                    NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                    NativeArray<MoveOverride> moveOverrideArray = entityQuery.ToComponentDataArray<MoveOverride>(Allocator.Temp);
                NativeArray<TargetOverride> targetOverrideArray = entityQuery.ToComponentDataArray<TargetOverride>(Allocator.Temp);
                NativeArray<float3> movePositionArray =  GenerateMovePositionArray(mouseWorldPosition, entityArray.Length);
            for (int i = 0; i < moveOverrideArray.Length; i++)
            {
                    MoveOverride moveOverride = moveOverrideArray[i];
                    moveOverride.targetPosition = movePositionArray[i];
                    moveOverrideArray[i] = moveOverride;
                    entityManager.SetComponentEnabled<MoveOverride>(entityArray[i], true);

                    TargetOverride targetOverride = targetOverrideArray[i];
                    targetOverride.targetEntity = Entity.Null;
                    targetOverrideArray[i] = targetOverride;

                }
                    entityQuery.CopyFromComponentDataArray(moveOverrideArray);
                entityQuery.CopyFromComponentDataArray(targetOverrideArray);
            }
            //处理建筑聚集点
            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected,BuildingBarracks,LocalTransform>().Build(entityManager);

            NativeArray<BuildingBarracks> buildingBarracksArray = entityQuery.ToComponentDataArray<BuildingBarracks>(Allocator.Temp);
            NativeArray<LocalTransform> localTransformsArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            for (int i = 0; i < buildingBarracksArray.Length; i++)
            {
                BuildingBarracks buildingBarracks = buildingBarracksArray[i];
                buildingBarracks.rallyPositionOffset =(float3) mouseWorldPosition - localTransformsArray[i].Position;
                LocalTransform rallyLocalTransform = entityManager.GetComponentData<LocalTransform>(buildingBarracks.rallyTargetPrefabEntity);
                rallyLocalTransform.Position = buildingBarracks.rallyPositionOffset;
                //rallyLocalTransform.Rotation = parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
                entityManager.SetComponentData(buildingBarracks.rallyTargetPrefabEntity, rallyLocalTransform);

                buildingBarracksArray[i] = buildingBarracks;

                //Debug.Log("1" + rallyLocalTransform.ValueRO.Position);
                //rallyLocalTransform.ValueRW.Position = buildingBarracks.ValueRO.rallyPositionOffset;
                //Debug.Log("2" + buildingBarracks.ValueRO.rallyPositionOffset);


            }
            entityQuery.CopyFromComponentDataArray(buildingBarracksArray);

        }
    }


    public Rect GetSelectionAreaRect()
    {
        Vector2 selectionEndMousePosition = Input.mousePosition;

        Vector2 lowerLeftCorner = new Vector2(
            Mathf.Min(selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Min(selectionStartMousePosition.y, selectionEndMousePosition.y));

        Vector2 upRightCorner = new Vector2(
    Mathf.Max(selectionStartMousePosition.x, selectionEndMousePosition.x),
    Mathf.Max(selectionStartMousePosition.y, selectionEndMousePosition.y));
        return new Rect(
            lowerLeftCorner.x,
            lowerLeftCorner.y,
            upRightCorner.x-lowerLeftCorner.x,
            upRightCorner.y-lowerLeftCorner.y );
    }


    private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition,int positionCount)
    {
        NativeArray<float3> positionArray = new NativeArray<float3>(positionCount,Allocator.Temp);
        if (positionCount == 0)
        {
            return positionArray;
        }

        positionArray[0] = targetPosition;
        if (positionCount == 1)
        {
            return positionArray;
        }
       
       // int _unitWidth = 5;

        float offsetX =2f;
        float offsetZ = 1.5f;

       // int rowCount = Mathf.CeilToInt(positionCount / 5);

        for (int i = 1; i < positionCount; i++)
        {
            int row = i / 5;
            int column = i % 5;

            float x = targetPosition.x + column * offsetX;  // X轴偏移
            float y = targetPosition.y;  // Y轴保持不变
            float z = targetPosition.z + row * offsetZ;  // Z轴偏移

            positionArray[i] =new float3( x, y, z );
          
           
        }
        return positionArray;

        //float ringSize = 2.2f;
        //int ring = 0;
        //int positionIndex = 1;
        //while (positionIndex < positionCount)
        //{



        //}
    }
}
