using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour
{
    public Image unitIcon;
    private int unitIndex;
    public Button unitButton;
    private bool isCooldown = false;

    public void Initialize(int unitIndex, Sprite unitIconSprite)
    {
        this.unitIndex = unitIndex;
        unitIcon.sprite = unitIconSprite;
        unitButton.onClick.AddListener(OnUnitButtonClicked);
    }

    public void OnUnitButtonClicked()
    {
        if (isCooldown)
        {
            return;
        }

        UnitData selectedUnitData = GameManager.instance.playerUnits[unitIndex];
        float cooldownTime = selectedUnitData.cooldown;

        if (GameManager.instance.placedUnits.Contains(selectedUnitData))
        {
            GameManager.instance.UpgradeUnit(selectedUnitData);
        }
        else
        {
            GameManager.instance.SelectUnit(unitIndex);
            GameManager.instance.OnPlaceUnitButtonClicked();
            GameManager.instance.PlaceUnit(selectedUnitData);
        }

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
        // Remove the listener when the button is destroyed.
        unitButton.onClick.RemoveListener(OnUnitButtonClicked);
    }
}
