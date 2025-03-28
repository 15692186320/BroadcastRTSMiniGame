using Unity.Entities;
using UnityEngine;

[CreateAssetMenu()]
public class UnitTypeSO : ScriptableObject
{
    public enum UnitType
    {
        None,
        Solider,
        AISolider,
        Zombie
    }

    public UnitType unitType;
    public float progessMax;
    public Sprite sprite;

    public Entity GetPrefabEntity(EntitiesReferences entitiesReferences)
    {
        switch (unitType)
        {
            default:
            case UnitType.None:

            case UnitType.Solider: return entitiesReferences.soliderPrefabEntity;

            case UnitType.AISolider:
                return entitiesReferences.aIsoliderPrefabEntity;

            case UnitType.Zombie:
                return entitiesReferences.zombiePrefabEntity;
        }
    }
}
