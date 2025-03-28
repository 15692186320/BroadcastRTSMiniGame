using UnityEngine;
using GPUECSAnimationBaker.Engine.AnimatorSystem;

[CreateAssetMenu()]
public class AnimationDataSO : ScriptableObject
{
    public enum AnimationType
    {
        None = -1,
        FiringRifle = 0,
        IdleRiflleSolider = 1,
        WalkWithRifle = 2,

        zombieWalk = 3,
        zombieAttack = 4,
        Hurricane = 5,
        HappyIdle = 6,
        Fastrun = 7
    }

    public AnimationType animationType;
}
