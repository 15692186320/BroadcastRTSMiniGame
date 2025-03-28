using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class EnemyAttackHQAuthoring : MonoBehaviour
{
    public class Baker : Baker<EnemyAttackHQAuthoring>
    {
        public override void Bake(EnemyAttackHQAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyAttackHQ());
        }
    }
}

public struct EnemyAttackHQ : IComponentData
{

}
