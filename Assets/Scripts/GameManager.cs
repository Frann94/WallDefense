using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject[] placementSpots;
    public Unit[] playerUnits;
    public UnitPlacement unitPlacement;
    public UnitButton unitButtonPrefab;
    public Transform unitButtonParent;
    public Dictionary<int, Unit> placedUnits = new Dictionary<int, Unit>();
    public LevelManager levelManager;
    public GameObject pointsText;

    private Unit _selectedUnit;
    private int _selectedUnitIndex;

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
        _selectedUnit = playerUnits[unitIndex];
        _selectedUnitIndex = unitIndex;
    }

    public void OnPlaceUnitButtonPressed()
    {
        if (_selectedUnit != null)
        {
            unitPlacement.OnUnitButtonPress(_selectedUnit, _selectedUnitIndex);
        }
    }

    public void PlaceUnit(Unit unit, int unitIndex)
    {
        placedUnits[unitIndex] = unit;
    }

    public void RemoveUnit(int unitIndex)
    {
        placedUnits.Remove(unitIndex);
    }
}
