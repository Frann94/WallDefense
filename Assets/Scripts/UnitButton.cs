using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class UnitButton : MonoBehaviour, IPointerDownHandler
{
    public Image unitIcon;
    private int unitIndex;
    public Button unitButton;
    private bool isCooldown = false;
    private float cooldownTime = 0f;
    public Unit placedUnit;
    public AudioClip placingSound;
    public AudioClip upgradeSound;
    private AudioSource audioSource;

    public void Initialize(int unitIndex, Sprite unitIconSprite)
    {
        this.unitIndex = unitIndex;
        unitIcon.sprite = unitIconSprite;
        GameManager.instance.playerUnits[unitIndex].OnUnitDied += ResetCooldown;
        // Make sure the UnityEvent for this button is set up
        if (!GameManager.instance.unitPlacement.onUnitPlaced.ContainsKey(unitIndex))
        {
            GameManager.instance.unitPlacement.onUnitPlaced[unitIndex] = new UnityEvent();
        }
        GameManager.instance.unitPlacement.onUnitPlaced[unitIndex].AddListener(StartCooldown);
        audioSource = GetComponent<AudioSource>();
    }

    private void ResetCooldown()
    {
        isCooldown = false;
        unitButton.interactable = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isCooldown)
        {
            return;
        }
        Unit selectedUnit = GameManager.instance.playerUnits[unitIndex];
        cooldownTime = selectedUnit.cooldown;
        if (GameManager.instance.placedUnits.ContainsKey(unitIndex))
        {
            audioSource.clip = upgradeSound;
            audioSource.Play();
            GameManager.instance.UpgradeUnit(unitIndex);
        }
        else
        {
            audioSource.clip = placingSound;
            audioSource.Play();
            GameManager.instance.SelectUnit(unitIndex);
            GameManager.instance.unitPlacement.OnUnitButtonPress(selectedUnit, unitIndex);
        }
    }

    private void StartCooldown()
    {
        StartCoroutine(Cooldown(cooldownTime));
    }

    private IEnumerator Cooldown(float cooldownTime)
    {
        isCooldown = true;
        unitButton.interactable = false;

        yield return new WaitForSeconds(cooldownTime);

        isCooldown = false;
        unitButton.interactable = true;
    }

    private void OnDestroy()
    {
        GameManager.instance.unitPlacement.onUnitPlaced[unitIndex].RemoveListener(StartCooldown);
    }
}

