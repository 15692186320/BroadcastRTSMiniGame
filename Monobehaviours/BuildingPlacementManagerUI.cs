using System.Collections.Generic;
using UnityEngine;
using static BuildingTypeSO;

public class BuildingPlacementManagerUI : MonoBehaviour
{
    [SerializeField] private RectTransform buildingContainer;
    [SerializeField] private RectTransform buildingTemplate;
    [SerializeField] private BuildingTypeListSO buildingTypeListSO;

    private Dictionary<BuildingTypeSO, BuildingPlacementManagerUI_ButtonSingle> buildingButtonDictionary;

    private void Awake()
    {
        buildingTemplate.gameObject.SetActive(false);

        buildingButtonDictionary = new Dictionary<BuildingTypeSO, BuildingPlacementManagerUI_ButtonSingle>();

        foreach (BuildingTypeSO buildingType in buildingTypeListSO.buildingTypeSOList)
        {
            if (!buildingType.showInBuildingPlacementManagerUI)
            {
                continue;
            }
           
            RectTransform buildingRectTransform = Instantiate(buildingTemplate, buildingContainer);
            buildingRectTransform.gameObject.SetActive(true);
            BuildingPlacementManagerUI_ButtonSingle buttonSingle = buildingRectTransform.GetComponent<BuildingPlacementManagerUI_ButtonSingle>();

            buildingButtonDictionary[buildingType] = buttonSingle;

            buttonSingle.Setup(buildingType);
        }
    }

    private void Start()
    {
        BuildingPlacementManager.Instance.OnActiveBuildingTypeSOChanged += BuildingPlacementManager_OnActiveBuildingTypeSOChanged;
        UpdateSelectedVisual();
    }

    private void BuildingPlacementManager_OnActiveBuildingTypeSOChanged(object sender, System.EventArgs e)
    {
        UpdateSelectedVisual();
    }

    private void UpdateSelectedVisual()
    {
        foreach (BuildingTypeSO buildingTypeSO in buildingButtonDictionary.Keys)
        {
            buildingButtonDictionary[buildingTypeSO].HideSelected();
        }
        buildingButtonDictionary[BuildingPlacementManager.Instance.GetActiveBuildingTypeSO()].ShowSelected() ;
    }
}
