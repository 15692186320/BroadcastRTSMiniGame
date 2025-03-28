using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;



[UpdateInGroup(typeof(LateSimulationSystemGroup),OrderLast =true)]
partial struct ResetEventSystem : ISystem
{
    private NativeArray<JobHandle> jobHandleNativeArray;

    private NativeList<Entity> onBarracksUnitQueueChangedEntityList;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        jobHandleNativeArray = new NativeArray<JobHandle>(4, Allocator.Persistent);

        onBarracksUnitQueueChangedEntityList = new NativeList<Entity>(Allocator.Persistent);
    }


   //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (SystemAPI.HasSingleton<BuildingHQ>()) {
            Health hqHealth = SystemAPI.GetComponent<Health>(SystemAPI.GetSingletonEntity<BuildingHQ>());
            if (hqHealth.onDead)
            {
                DOTSEventManager.Instance.TriggerOnHQDead();
            }
        
        }
        jobHandleNativeArray[0] =  new ResetHealthEventsJob().ScheduleParallel(state.Dependency);

        jobHandleNativeArray[1] = new ResetSelectedEventsJob().ScheduleParallel(state.Dependency);

        jobHandleNativeArray[2] = new ResetShootAttackEventsJob().ScheduleParallel(state.Dependency);

        jobHandleNativeArray[3] = new ResetMeleeAttackEventsJob().ScheduleParallel(state.Dependency);

        onBarracksUnitQueueChangedEntityList.Clear();

        new ResetBuildingBarracksEventsJob() { 
        
            onUnitQueueChangedEntityList = onBarracksUnitQueueChangedEntityList.AsParallelWriter(),
        
        }.ScheduleParallel(state.Dependency).Complete();

        DOTSEventManager.Instance.TriggerOnBarracksUnitQueueChanged(onBarracksUnitQueueChangedEntityList);

        state.Dependency = JobHandle.CombineDependencies(jobHandleNativeArray);

        //foreach (RefRW<Selected> selected in SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>())
        //{
        //    selected.ValueRW.onSelected = false;
        //    selected.ValueRW.onDeselected = false;

        //}

        //foreach (RefRW<Health> health in SystemAPI.Query<RefRW<Health>>())
        //{
        //   health.ValueRW.onHealthChanged = false;
        //}

        //foreach (RefRW<ShootAttack> shootattack in SystemAPI.Query<RefRW<ShootAttack>>())
        //{
        //   shootattack.ValueRW.onShoot.isTrigger = false;
        //}
    }



    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

            jobHandleNativeArray.Dispose();

           onBarracksUnitQueueChangedEntityList.Dispose();


    }

}


[BurstCompile]
public partial struct ResetShootAttackEventsJob : IJobEntity
{
    public void Execute(ref ShootAttack shootAttack)
    {
        shootAttack.onShoot.isTrigger = false;
    }
}


[BurstCompile]
public partial struct ResetHealthEventsJob : IJobEntity
{
    public void Execute(ref Health health)
    {
        health.onHealthChanged = false;
        health.onDead = false;
    }
}


[BurstCompile]
[WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
public partial struct ResetSelectedEventsJob : IJobEntity
{
    public void Execute(ref Selected selected)
    {
        selected.onSelected = false;
        selected.onDeselected = false;
    }
}


[BurstCompile]
public partial struct ResetMeleeAttackEventsJob : IJobEntity
{
    public void Execute(ref MeleeAttack meleeAttack)
    {
        meleeAttack.onAttacked = false;
    }
}

[BurstCompile]
public partial struct ResetBuildingBarracksEventsJob : IJobEntity
{
    public NativeList<Entity>.ParallelWriter onUnitQueueChangedEntityList;
    public void Execute(ref BuildingBarracks buildingBarracks,Entity entity)
    {
        if (buildingBarracks.onUnitQueueChanged)
        {
            onUnitQueueChangedEntityList.AddNoResize(entity);
        }
        buildingBarracks.onUnitQueueChanged = false;
       
    }
}