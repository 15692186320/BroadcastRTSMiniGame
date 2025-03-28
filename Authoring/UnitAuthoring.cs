using Unity.Entities;
using UnityEngine;

public class UnitAuthoring : MonoBehaviour
{


    public class UnitBaker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Unit{
            });
            AddComponent(entity, new AgentIndex
            {
                Value = -1,
            });
        }
    }

}

public struct Unit : IComponentData
{

}
