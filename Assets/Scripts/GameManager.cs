using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject[] placementSpots; // Assign in editor
    public Unit[] playerUnits; // Assign in editor
    public UnitPlacement unitPlacement; //Assign in editor
    public UnitButton unitButtonPrefab; // Assign in editor
    public Transform unitButtonParent; // Assign in editor
    private Unit selectedUnit;
    public Dictionary<int, Unit> placedUnits = new Dictionary<int, Unit>();
    private int unitIndex;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
    }

    private void Start()
    {
        ResetGame();
    }

    private void ResetGame()
    {
        placedUnits.Clear();
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
        this.unitIndex = unitIndex;
    }

    public void OnPlaceUnitButtonPressed()
    {
        if (selectedUnit != null)
        {
            unitPlacement.OnUnitButtonPress(selectedUnit, unitIndex);
        }
    }

    public void PlaceUnit(Unit unit, int unitIndex)
    {
        if (!placedUnits.ContainsKey(unitIndex))
        {
            placedUnits[unitIndex] = unit;
        }
    }

    public void RemoveUnit(int unitIndex)
    {
        if (placedUnits.ContainsKey(unitIndex))
        {
            placedUnits.Remove(unitIndex);
        }
    }

    public void UpgradeUnit(int unitIndex)
    {
        if (!placedUnits.ContainsKey(unitIndex))
        {
            return;
        }
        Unit unit = placedUnits[unitIndex];
        unit.upgradeLevel++;
        unit.MaxHealth += 10;
        unit.attackDamage += 5;
        Debug.Log(unit.name + " was upgraded to level " + unit.upgradeLevel);
        if (unitPlacement.onUnitPlaced.TryGetValue(unitIndex, out UnityEvent unitPlacedEvent))
        {
            unitPlacedEvent?.Invoke();
        }
    }
}
