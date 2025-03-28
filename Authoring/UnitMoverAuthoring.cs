using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class UnitMoverAuthoring : MonoBehaviour
{
    public float moveSpeed;
    public float rotationSpeed;

    public bool isTowerSolider;

    public class Baker : Baker<UnitMoverAuthoring>
    {
        public override void Bake(UnitMoverAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitMover
            {
                moveSpeed = authoring.moveSpeed,
                rotationSpeed = authoring.rotationSpeed,
                isTowerSolider = authoring.isTowerSolider
            });
            if (!authoring.isTowerSolider)
            {
                AddComponent(entity, new ORCAAgentReference
                {
                    agentIndex = -1
                });
            }
        }
    }
}


public struct UnitMover : IComponentData
{
    public float moveSpeed;
    public float rotationSpeed;
    public float3 targetPosition;
    public bool isMoving;
    public bool isTowerSolider;
}

public struct ORCAAgentReference : IComponentData
{
    public int agentIndex;
}