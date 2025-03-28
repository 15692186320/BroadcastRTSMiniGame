using Unity.Entities;
using UnityEngine;

public class AnimationMeshAuthoring : MonoBehaviour
{
    public GameObject meshGameObject;

    public class Baker : Baker<AnimationMeshAuthoring>
    {
        public override void Bake(AnimationMeshAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new AnimatedMesh
            {
                meshEntity = GetEntity(authoring.meshGameObject, TransformUsageFlags.Dynamic)
            }) ;
        }
    }


}

public struct AnimatedMesh : IComponentData
{
    public Entity meshEntity;
}
