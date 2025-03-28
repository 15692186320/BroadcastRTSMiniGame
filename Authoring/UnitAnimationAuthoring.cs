using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Entities;
using UnityEngine;

public class UnitAnimationAuthoring : MonoBehaviour
{

    public AnimationDataSO.AnimationType idleAnimation;

    public AnimationDataSO.AnimationType walkAnimation;

    public AnimationDataSO.AnimationType shootAnimation;

    public AnimationIdsZombieAnimation zombieWalk;

    public AnimationIdsZombieAnimation zombieAttack;

    public class Baker : Baker<UnitAnimationAuthoring>
    {
        public override void Bake(UnitAnimationAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UnitAnimation
            {
                idleAnimation = authoring.idleAnimation,
                walkAnimation = authoring.walkAnimation,    
                shootAnimation = authoring.shootAnimation,
                zombieWalk = authoring.zombieWalk,
                zombieAttack = authoring.zombieAttack
            });
        }
    }
}



public struct UnitAnimation : IComponentData
{
    public AnimationDataSO.AnimationType idleAnimation;

    public AnimationDataSO.AnimationType walkAnimation;

    public AnimationDataSO.AnimationType shootAnimation;

    public AnimationIdsZombieAnimation zombieWalk;

    public AnimationIdsZombieAnimation zombieAttack;
}
