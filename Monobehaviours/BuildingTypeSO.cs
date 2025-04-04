﻿using Unity.Entities;
using UnityEngine;
using static UnitTypeSO;

[CreateAssetMenu]
public class BuildingTypeSO : ScriptableObject
{
    public enum BuildingType
    {
        None,
        ZombieSpawner,
        Tower,
        Barracks,
        HQ,
    }

    public BuildingType buildingType;
    public Transform prefab;
    public float buildingDistanceMin;
    public bool showInBuildingPlacementManagerUI;
    public Sprite sprite;
    public Transform visualPrefab;

    public bool IsNone()
    {
        return buildingType == BuildingType.None;
    }

    public Entity GetPrefabEntity(EntitiesReferences entitiesReferences)
    {
        switch (buildingType)
        {
            default:
            case BuildingType.None:

            case BuildingType.Tower: return entitiesReferences.buildingTowerPrefabEntity;

            case BuildingType.Barracks:
                return entitiesReferences.buildingBarracksPrefabEntity;
        }
    }
}
