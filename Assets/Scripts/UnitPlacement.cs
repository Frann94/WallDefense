using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitPlacement : MonoBehaviour
{
    public GameManager gameManager;
    public LayerMask unitLayer;
    public float snapDistance = 1f;
    public Dictionary<int, UnityEvent> onUnitPlaced = new Dictionary<int, UnityEvent>();
    public AudioClip unitPlaced;
    public AudioClip invalidPlace;

    private Unit _currentUnit;
    private BoxCollider2D _unitCollider;
    private bool _isPlacementValid = true;
    private string _origTag;
    private int _unitIndex;
    private AudioSource _audioSource;
    private Color _origColor;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void OnUnitButtonPress(Unit unit, int index)
    {
        _currentUnit = Instantiate(unit);
        _currentUnit.IsBeingPlaced = true;
        _origTag = _currentUnit.tag;
        _unitCollider = _currentUnit.GetComponent<BoxCollider2D>();
        _unitIndex = index;

        AdjustSpriteOpacity(_currentUnit, 0.5f);
    }

    private void AdjustSpriteOpacity(Unit unit, float opacity)
    {
        SpriteRenderer spriteRenderer = unit.GetComponent<SpriteRenderer>();
        _origColor = spriteRenderer.color;
        Color color = spriteRenderer.color;
        color.a = opacity;
        spriteRenderer.color = color;
    }

    private void Update()
    {
        if (_currentUnit == null)
        {
            return;
        }

        Vector3 placementPos = Input.touchCount > 0 ?
            Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position) :
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        placementPos.z = 0;
        if (Input.GetMouseButtonUp(0))
        {
            CheckPlacementValidityAndSnap(placementPos, true);
        }
        else
        {
            CheckPlacementValidityAndSnap(placementPos, false);
        }
    }

    private void CheckPlacementValidityAndSnap(Vector3 placementPos, bool attemptToPlace)
    {
        _unitCollider.enabled = false;
        _currentUnit.tag = "Untagged";
        Vector2 placementPos2D = placementPos;
        float diagonalLength = Mathf.Sqrt(Mathf.Pow(_unitCollider.size.x, 2) + Mathf.Pow(_unitCollider.size.y, 2));
        float radius = diagonalLength / 2f;
        LayerMask combinedLayerMask = unitLayer | LayerMask.GetMask("NoPlacement");
        SpriteRenderer spriteRenderer = _currentUnit.GetComponent<SpriteRenderer>();

        _isPlacementValid = !Physics2D.OverlapCircle(placementPos2D, radius, combinedLayerMask);

        spriteRenderer.color = _isPlacementValid ? _origColor : Color.red;
        GameObject closestSpot = gameManager.GetClosestPlacementSpot(placementPos);
        bool snapToSpot = closestSpot != null && Vector3.Distance(placementPos, closestSpot.transform.position) <= snapDistance;
        _currentUnit.transform.position = snapToSpot ? closestSpot.transform.position : placementPos;

        if (attemptToPlace)
        {
            PlaceUnitOrDestroy(snapToSpot);
        }
    }

    private void PlaceUnitOrDestroy(bool snapToSpot)
    {
        if (_isPlacementValid || snapToSpot)
        {
            _audioSource.clip = unitPlaced;
            _audioSource.Play();
            _unitCollider.enabled = true;
            _currentUnit.tag = _origTag;
            _currentUnit.IsBeingPlaced = false;

            AdjustSpriteOpacity(_currentUnit, 1.0f);
            GameManager.instance.PlaceUnit(_currentUnit, _unitIndex);

            if (onUnitPlaced.TryGetValue(_unitIndex, out UnityEvent unitPlacedEvent))
            {
                unitPlacedEvent?.Invoke();
            }
            gameManager.levelManager.Points -= _currentUnit.cost;
            gameManager.levelManager.UpdatePointsUI();
            _currentUnit = null;
        }
        else
        {
            _audioSource.clip = invalidPlace;
            _audioSource.Play();
            Destroy(_currentUnit.gameObject);
            _currentUnit = null;
        }
    }
}
