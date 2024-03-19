using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

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

    private void UpdateButtonState()
    {
        Unit selectedUnit = GameManager.instance.playerUnits[_unitIndex];
        if (selectedUnit.tag == "BlockedUnit")
        {
            _isCooldown = true;
            unitButton.interactable = false;
        }
        else
        {
            _isCooldown = false;
            unitButton.interactable = true;
        }
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
            GameManager.instance.UpgradeUnit(_unitIndex);
        }
        else
        {
            _audioSource.clip = placingSound;
            _audioSource.Play();
            GameManager.instance.SelectUnit(_unitIndex);
            GameManager.instance.unitPlacement.OnUnitButtonPress(selectedUnit, _unitIndex);
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
