using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;

partial struct UnitMoverSystem : ISystem
{
    public const float REACHED_TARGET_POSITION_dISTANCE_SQ = 2f;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UnitMoverJob unitMoverJob = new UnitMoverJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,

        };
        unitMoverJob.ScheduleParallel();

    }

}



//[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{

    public float deltaTime;
    public void Execute(ref LocalTransform localTransform, ref UnitMover unitMover, ref PhysicsVelocity physicsVelocity,ref AgentIndex agentIndex)
    {
        float3 moveDirection =new float3(unitMover.targetPosition.x-localTransform.Position.x,0, unitMover.targetPosition.z-localTransform.Position.z);

        float reachedTargetDistanceSq = UnitMoverSystem.REACHED_TARGET_POSITION_dISTANCE_SQ;

        if (math.lengthsq(moveDirection) <= reachedTargetDistanceSq && !unitMover.isTowerSolider)
        {
            //MyORCASimulation.Instance.m_orca.agents[agentIndex.Value].pos = unitMover.targetPosition;
            //MyORCASimulation.Instance.m_orca.agents[agentIndex.Value].prefVelocity = 0.0f;
            unitMover.isMoving = false;
            physicsVelocity.Linear = float3.zero;
            return;
        }
        //float maxSpeed = unitMover.moveSpeed;
        //float3 targetPos = unitMover.targetPosition;
        //float s = math.clamp(math.distance(MyORCASimulation.Instance.m_orca.agents[agentIndex.Value].pos, targetPos) / (2f * maxSpeed),0.2f,1f);
        //float agentSpeed = maxSpeed ;
        //MyORCASimulation.Instance.m_orca.agents[agentIndex.Value].maxSpeed = agentSpeed ;
        //MyORCASimulation.Instance.m_orca.agents[agentIndex.Value].prefVelocity = math.normalize(moveDirection) * agentSpeed;
        moveDirection = math.normalize(moveDirection);
        physicsVelocity.Linear = unitMover.moveSpeed * moveDirection;
        localTransform.Rotation = math.slerp(localTransform.Rotation, quaternion.LookRotation(moveDirection, math.up()), deltaTime * unitMover.rotationSpeed);
        unitMover.isMoving = true;

    }

}


//[BurstCompile]
//public partial struct UnitMoverJob : IJobEntity
//{

//    public float deltaTime;
//    public void Execute(ref LocalTransform localTransform, ref UnitMover unitMover, ref PhysicsVelocity physicsVelocity)
//    {
//        float3 moveDirection = unitMover.targetPosition - localTransform.Position;
//        float reachedTargetDistanceSq = UnitMoverSystem.REACHED_TARGET_POSITION_dISTANCE_SQ;

//        if (math.lengthsq(moveDirection) <= reachedTargetDistanceSq && !unitMover.isTowerSolider)
//        {
//            physicsVelocity.Linear = float3.zero;
//            physicsVelocity.Angular = float3.zero;
//            unitMover.isMoving = false;
//            return;
//        }
//        unitMover.isMoving = true;

//        moveDirection = math.normalize(moveDirection);
//        moveDirection.y = 0;

//        localTransform.Rotation = math.slerp(localTransform.Rotation, quaternion.LookRotation(moveDirection, math.up()), deltaTime * unitMover.rotationSpeed);

//        physicsVelocity.Linear = moveDirection * unitMover.moveSpeed;
//        physicsVelocity.Angular = float3.zero;
//    }

//}


//foreach ((RefRW<LocalTransform>  localTransform ,
//               RefRO<UnitMover> unitMover,
//                RefRW<PhysicsVelocity> physicsVelocity) 
//                in SystemAPI.Query<
//                    RefRW<LocalTransform>,
//                    RefRO<UnitMover>,
//                    RefRW<PhysicsVelocity>>())
//{
//    float3 moveDirection =unitMover.ValueRO.targetPosition - localTransform.ValueRO.Position;
//    moveDirection = math.normalize (moveDirection);


//    localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, quaternion.LookRotation(moveDirection, math.up()), SystemAPI.Time.DeltaTime*unitMover.ValueRO.rotationSpeed);

//    physicsVelocity.ValueRW.Linear = moveDirection * unitMover.ValueRO.moveSpeed;
//    physicsVelocity.ValueRW.Angular = float3.zero;
//    //localTransform.ValueRW.Position +=moveDirection * moveSpeed.ValueRO.value * SystemAPI.Time.DeltaTime;
//}