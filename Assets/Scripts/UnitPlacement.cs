using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlacement : MonoBehaviour
{
    public GameManager gameManager; // Set this in the editor
    public LayerMask unitLayer; // Set this in the editor to the layer(s) where units reside
    public float snapDistance = 1f; // Set this in the editor to the distance at which units will snap to spots
    private GameObject currentUnit;
    private Collider2D unitCollider;
    private bool isPlacementValid = true;
    private string origTag;

    public void PlaceUnit(GameObject unitPrefab)
    {
        currentUnit = Instantiate(unitPrefab);
        origTag = currentUnit.tag;
        unitCollider = currentUnit.GetComponent<Collider2D>();
    }


    private void Update()
    {
        // If there's no unit currently being placed, we don't need to do anything
        if (currentUnit == null)
        {
            return;
        }

        // Vector to store the position where we will place our unit
        Vector3 placementPos;

        #if UNITY_STANDALONE || UNITY_WEBGL
        // Mouse input for standalone or web builds
        placementPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        #else
        // Touch input for mobile builds
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            placementPos = Camera.main.ScreenToWorldPoint(touch.position);
        }
        else
        {
            return;
        }
        #endif

        placementPos.z = 0;

        // Disable the collider temporarily for overlap check
        unitCollider.enabled = false;
        currentUnit.tag = "Untagged";

        Vector2 placementPos2D = new Vector2(placementPos.x, placementPos.y);  // Convert to 2D
        float radius = unitCollider.bounds.size.magnitude / 2f;  // Approximate radius

        // Combine the unit layer and the no-placement layer into one LayerMask
        LayerMask combinedLayerMask = unitLayer | LayerMask.GetMask("NoPlacement");

        // Check for collisions at the unit's position with other units and no-placement zones
        isPlacementValid = !Physics2D.OverlapCircle(placementPos2D, radius, combinedLayerMask);

        // Determine whether to snap based on distance to the closest spot
        GameObject closestSpot = gameManager.GetClosestPlacementSpot(placementPos);
        bool snapToSpot = closestSpot != null && Vector3.Distance(placementPos, closestSpot.transform.position) <= snapDistance;

        if (snapToSpot)
        {
            currentUnit.transform.position = closestSpot.transform.position;
        }
        else
        {
            currentUnit.transform.position = placementPos;
        }

        // If the user lifts their finger or releases the mouse button, place the unit
        #if UNITY_STANDALONE || UNITY_WEBGL
        if (Input.GetMouseButtonDown(0) && (isPlacementValid || snapToSpot))
        {
            // Re-enable the collider
            unitCollider.enabled = true;
            currentUnit.tag = origTag;

            currentUnit = null;
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && (isPlacementValid || snapToSpot))
        {
            // Re-enable the collider
            unitCollider.enabled = true;
            currentUnit.tag = origTag;

            currentUnit = null;
        }
#endif
    }


}

