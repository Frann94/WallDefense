using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject[] placementSpots;
    public UnitData[] playerUnits; // Assign in editor.
    private UnitData selectedUnit;
    public UnitPlacement unitPlacement; // Set this in the editor
    public UnitButton unitButtonPrefab; // Assign in editor.
    public Transform unitButtonParent; // Assign in editor.
    public List<UnitData> placedUnits = new List<UnitData>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        placementSpots = GameObject.FindGameObjectsWithTag("PlacementSpot");
        for (int i = 0; i < playerUnits.Length; i++)
        {
            UnitButton unitButton = Instantiate(unitButtonPrefab, unitButtonParent.transform);
            unitButton.Initialize(i, playerUnits[i].unitIcon);
        }
    }

    public GameObject GetClosestPlacementSpot(Vector3 position)
    {
        GameObject closestSpot = null;
        float closestDistance = Mathf.Infinity;
        
        foreach (GameObject spot in placementSpots)
        {
            float distance = Vector3.Distance(spot.transform.position, position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSpot = spot;
            }
        }

        return closestSpot;
    }

    public void SelectUnit(int unitIndex)
    {
        selectedUnit = playerUnits[unitIndex];
    }

    public void OnPlaceUnitButtonClicked()
    {
        if (selectedUnit != null)
        {
            Command command = new PlaceUnitCommand(unitPlacement, selectedUnit.unitPrefab);
            command.Execute();
        }
    }

    public void PlaceUnit(UnitData unit)
    {
        if (!placedUnits.Contains(unit))
        {
            placedUnits.Add(unit);
        }
    }

    public void UpgradeUnit(UnitData unit)
    {
        // Add code here to upgrade the unit
        Debug.Log(unit.name + " was upgraded");
    }
}
