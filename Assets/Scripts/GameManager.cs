using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject[] placementSpots;
    public GameObject unitPrefab; // Set this in the editor
    public UnitPlacement unitPlacement; // Set this in the editor

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

    public void OnPlaceUnitButtonClicked()
    {
        Command command = new PlaceUnitCommand(unitPlacement, unitPrefab);
        command.Execute();
    }
}
