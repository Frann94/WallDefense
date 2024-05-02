using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class UnitButton : MonoBehaviour, IPointerDownHandler
{
    public Image unitIcon;
    public Button unitButton;
    public Unit placedUnit;
    public AudioClip placingSound;
    public AudioClip upgradeSound;

    private AudioSource _audioSource;
    private int _unitIndex;    
    private bool _isCooldown = false;
    private float _cooldownTime = 0f;
    private GameObject _currentFloatingText;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Initialize(int unitIndex, Sprite unitIconSprite)
    {
        this._unitIndex = unitIndex;
        unitIcon.sprite = unitIconSprite;
        GameManager.instance.playerUnits[unitIndex].OnUnitDied += ResetCooldown;

        if (!GameManager.instance.unitPlacement.onUnitPlaced.ContainsKey(unitIndex))
        {
            GameManager.instance.unitPlacement.onUnitPlaced[unitIndex] = new UnityEvent();
        }
        GameManager.instance.unitPlacement.onUnitPlaced[unitIndex].AddListener(StartCooldown);

        UpdateButtonState();
    }

    private void ResetCooldown()
    {
        _isCooldown = false;
        UpdateButtonState();
    }

    public void UpdateButtonState()
    {
        Unit selectedUnit = GameManager.instance.playerUnits[_unitIndex];
        LevelManager levelManager = GameManager.instance.levelManager;
        if (selectedUnit.tag == "BlockedUnit" || selectedUnit.cost > levelManager.Points || _isCooldown==true)
        {
            unitButton.interactable = false;
        }
        else
        {
            _isCooldown = false;
            unitButton.interactable = true;
        }
    }

    public void Update()
    {
        UpdateButtonState();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isCooldown)
        {
            return;
        }

        Unit selectedUnit = GameManager.instance.playerUnits[_unitIndex];
        _cooldownTime = selectedUnit.cooldown;

        if (GameManager.instance.placedUnits.ContainsKey(_unitIndex))
        {
            _audioSource.clip = upgradeSound;
            _audioSource.Play();
            UpgradeUnit(_unitIndex);
        }
        else
        {
            _audioSource.clip = placingSound;
            _audioSource.Play();
            GameManager.instance.SelectUnit(_unitIndex);
            GameManager.instance.unitPlacement.OnUnitButtonPress(selectedUnit, _unitIndex);
        }
    }

    public void UpgradeUnit(int unitIndex)
    {
        Dictionary<int, Unit> placedUnits = GameManager.instance.placedUnits;
        if (!placedUnits.ContainsKey(unitIndex))
        {
            return;
        }

        Unit unit = placedUnits[unitIndex];
        if (unit.upgradeLevel==5)
        {
            ShowFloatingText("Max Level!", unit.transform.position);
            return;
        }
        int upgradeCost = CalculateUpgradeCost(unit.cost, unit.upgradeLevel);
        LevelManager levelManager = GameManager.instance.levelManager;
        if (levelManager.Points >= upgradeCost)
        {
            ShowFloatingText("Level Up! (-" + upgradeCost + "p)", unit.transform.position);
            unit.UpgradeUnit();
            levelManager.Points -= upgradeCost;
            levelManager.UpdatePointsUI();
            StartCooldown();
        }
        else
        {
            ShowFloatingText("You need " + upgradeCost + " points!", unit.transform.position);
        }
    }

    private int CalculateUpgradeCost(int unitCost, int upgradeLevel)
    {
        return unitCost * (upgradeLevel + 1);
    }

    private void ShowFloatingText(string text, Vector3 unitPos)
    {
        GameObject pointsText = GameManager.instance.pointsText;
        if (pointsText != null)
        {
            if (_currentFloatingText != null)
            {
                _currentFloatingText.SetActive(false);
            }
            GameObject floatingText = Instantiate(pointsText, unitPos, Quaternion.identity);
            floatingText.GetComponent<TextMesh>().text = text;
            floatingText.transform.position += new Vector3(-1.5f, 1.5f, 0f);
            Destroy(floatingText,3f);
            _currentFloatingText = floatingText;
        }
    }

    private void StartCooldown()
    {
        StartCoroutine(Cooldown(_cooldownTime));
    }

    private IEnumerator Cooldown(float cooldownTime)
    {
        _isCooldown = true;
        unitButton.interactable = false;

        yield return new WaitForSeconds(cooldownTime);

        _isCooldown = false;
        UpdateButtonState();
    }

    private void OnDestroy()
    {
        GameManager.instance.playerUnits[_unitIndex].OnUnitDied -= ResetCooldown;

        if (GameManager.instance.unitPlacement.onUnitPlaced.ContainsKey(_unitIndex))
        {
            GameManager.instance.unitPlacement.onUnitPlaced[_unitIndex].RemoveListener(StartCooldown);
        }
    }
}
