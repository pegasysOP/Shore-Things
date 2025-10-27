using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUpgrade : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public Button upgradeButton;

    private UpgradeData upgradeData;
    private Action<UpgradeData> onUpgrade;

    private void OnEnable()
    {
        upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
    }
    
    private void OnDisable()
    {
        upgradeButton.onClick.RemoveListener(OnUpgradeButtonClick);
    }

    public void Init(UpgradeData upgradeData, Action<UpgradeData> onUpgrade)
    {
        this.upgradeData = upgradeData;
        this.onUpgrade = onUpgrade;

        nameText.text = upgradeData.name;
        costText.text = "$" + upgradeData.cost.ToString("F2");

        if (GameManager.Instance.inventory.GetMoney() < upgradeData.cost)
        {
            upgradeButton.interactable = false;
            costText.color = Color.red;
        }
        else
        {
            upgradeButton.interactable = true;
            costText.color = Color.white;
        }
    }

    private void OnUpgradeButtonClick()
    {
        if (GameManager.Instance.inventory.GetMoney() < upgradeData.cost)
            return;

        AudioManager.Instance.PlaySfx(AudioManager.Instance.coinClips);

        onUpgrade?.Invoke(upgradeData);
    }
}
