using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class BuildingBarracksUI : MonoBehaviour
{
    [SerializeField] private Button AIsoliderButton;

    [SerializeField] private Button soliderButton;

    [SerializeField] private Image progressBarImage;

    [SerializeField] private RectTransform unitQueueContainer;
    [SerializeField] private RectTransform unitQueueTemplate;

    private Entity buildingBarracksEntity;

    private EntityManager entityManager;

    private void Awake()
    {
        soliderButton.onClick.AddListener(() =>
        {
            // Debug.Log("Queue Soldier");
            entityManager.SetComponentData(buildingBarracksEntity, new BuildingBarracksUnitEnqueue
            {
                unitType = UnitTypeSO.UnitType.Solider,
            });


            entityManager.SetComponentEnabled<BuildingBarracksUnitEnqueue>(buildingBarracksEntity, true);
           // DynamicBuffer<SpawnUnitTypeBuffer> spawnUnitTypeBuffers = entityManager.GetBuffer<SpawnUnitTypeBuffer>(buildingBarracksEntity, false);
           // spawnUnitTypeBuffers.Add(new SpawnUnitTypeBuffer
           // {
           //     unitType = UnitTypeSO.UnitType.AISolider,
           // });
           //BuildingBarracks buildingBarracks = entityManager.GetComponentData<BuildingBarracks>(buildingBarracksEntity);
           // buildingBarracks.onUnitQueueChanged = true;
           // entityManager.SetComponentData(buildingBarracksEntity, buildingBarracks);

        });

        AIsoliderButton.onClick.AddListener(() =>
        {
            // Debug.Log("Queue Soldier");
            entityManager.SetComponentData(buildingBarracksEntity, new BuildingBarracksUnitEnqueue
            {
                unitType = UnitTypeSO.UnitType.AISolider,
            });


            entityManager.SetComponentEnabled<BuildingBarracksUnitEnqueue>(buildingBarracksEntity, true);
            // DynamicBuffer<SpawnUnitTypeBuffer> spawnUnitTypeBuffers = entityManager.GetBuffer<SpawnUnitTypeBuffer>(buildingBarracksEntity, false);
            // spawnUnitTypeBuffers.Add(new SpawnUnitTypeBuffer
            // {
            //     unitType = UnitTypeSO.UnitType.AISolider,
            // });
            //BuildingBarracks buildingBarracks = entityManager.GetComponentData<BuildingBarracks>(buildingBarracksEntity);
            // buildingBarracks.onUnitQueueChanged = true;
            // entityManager.SetComponentData(buildingBarracksEntity, buildingBarracks);

        });

        unitQueueTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
         entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        UnitSelectionManager.Instance.OnSelectedEntitiesChanged += UnitSelectionManager_OnSelectedEntitiesChanged;
        DOTSEventManager.Instance.onBarracksUnitQueueChanged += DOTSEventManager_onBarracksUnitQueueChanged;
        Hide();
    }

    private void DOTSEventManager_onBarracksUnitQueueChanged(object sender, System.EventArgs e)
    {
        Entity entity = (Entity)sender;
        if (entity == buildingBarracksEntity)
        {
            UpdateUnitQueueVisual();
        }
       
    }

    private void Update()
    {
        UpdateProgressBarVisual();
        //if (entityManager.Exists(buildingBarracksEntity))
        //{
           
           
        //}
        //else
        //{
        //    Hide();
        //}

    }

    private void UnitSelectionManager_OnSelectedEntitiesChanged(object sender, System.EventArgs e)
    {
       
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected, BuildingBarracks>().Build(entityManager);

        NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
        if (entityArray.Length>0)
        {
            //选择了一个带有selected 和BuildingBarracks组件的实体
            buildingBarracksEntity = entityArray[0];
           LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(buildingBarracksEntity);
            float3 uIOffset = new float3(0, 10, 0);
            gameObject.transform.position = localTransform.Position+uIOffset;
           Show();
            UpdateProgressBarVisual();
            UpdateUnitQueueVisual();
        }
        else
        {
            buildingBarracksEntity = Entity.Null;
            Hide();
        }
    }

    private void UpdateProgressBarVisual()
    {
        if (buildingBarracksEntity == Entity.Null || !entityManager.Exists(buildingBarracksEntity))
        {
            Hide();
            progressBarImage.fillAmount = 0f;
            return;
        }
        if (!entityManager.HasComponent<BuildingBarracks>(buildingBarracksEntity))
        {
            Hide();
            progressBarImage.fillAmount = 0f;
            return;
        }


        BuildingBarracks buildingBarracks = entityManager.GetComponentData<BuildingBarracks>(buildingBarracksEntity);
        if (buildingBarracks.activeUnitType == UnitTypeSO.UnitType.None)
        {
            progressBarImage.fillAmount = 0f;
        }
        else
        {
            progressBarImage.fillAmount = buildingBarracks.progress/ buildingBarracks.progressMax;
        }
        
    }

    private void UpdateUnitQueueVisual()
    {
        foreach (Transform child in unitQueueContainer)
        {
            if (child == unitQueueTemplate)
            {
                continue;
            }
            Destroy(child.gameObject);
        }
        DynamicBuffer<SpawnUnitTypeBuffer> spawnUnitTypeDynamicBuffers = entityManager.GetBuffer<SpawnUnitTypeBuffer>(buildingBarracksEntity, true);
        foreach (SpawnUnitTypeBuffer spawnUnitTypeBuffer in spawnUnitTypeDynamicBuffers)
        {
            // Debug.Log(unitQueueContainer.transform.childCount);
            if (unitQueueContainer.transform.childCount < 28)
            {
                RectTransform unitQueueRectTransform = Instantiate(unitQueueTemplate, unitQueueContainer);
                unitQueueRectTransform.gameObject.SetActive(true);
                UnitTypeSO unitTypeSO = GameAssets.Instance.unitTypeListSO.GetUnitTypeSO(spawnUnitTypeBuffer.unitType);
                unitQueueRectTransform.GetComponent<Image>().sprite = unitTypeSO.sprite;
            }
            //RectTransform unitQueueRectTransform = Instantiate(unitQueueTemplate, unitQueueContainer);
            //unitQueueRectTransform.gameObject.SetActive(true);
           
            
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
