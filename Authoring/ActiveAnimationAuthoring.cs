using Unity.Entities;
using UnityEngine;

public class ActiveAnimationAuthoring : MonoBehaviour
{
    public AnimationDataSO.AnimationType nextAnimationType;
    public class Baker : Baker<ActiveAnimationAuthoring>
    {
        public override void Bake(ActiveAnimationAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ActiveAnimation
            {

                nextAnimationType = authoring.nextAnimationType,

                activeAnimationType = AnimationDataSO.AnimationType.None
            });
        }
    }
}


public struct ActiveAnimation : IComponentData
{
    public AnimationDataSO.AnimationType activeAnimationType;
    public AnimationDataSO.AnimationType nextAnimationType;

}
