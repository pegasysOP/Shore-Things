using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour
{
    public Button closeButton;
    public Button sellButton;
    public Transform upgradeComponentContainer;
    public ShopUpgrade upgradeComponentPrefab;

    private List<GameObject> upgradeComponents = new List<GameObject>();

    private Shop currentShop;

    private void OnEnable()
    {
        closeButton.onClick.AddListener(CloseShop);
        sellButton.onClick.AddListener(OnSellButtonClick);
    }
    
    private void OnDisable()
    {
        closeButton.onClick.RemoveListener(CloseShop);
        sellButton.onClick.RemoveListener(OnSellButtonClick);
    }

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
            CloseShop();
    }

    public void OpenShop(Shop shop)
    {
        currentShop = shop;  

        foreach (UpgradeData upgradeData in UpgradeManager.Instance.upgrades)
        {
            ShopUpgrade upgradeComponent = Instantiate(upgradeComponentPrefab, upgradeComponentContainer);
            upgradeComponent.Init(upgradeData, OnBuyButtonClick);
            upgradeComponents.Add(upgradeComponent.gameObject);
        }

        GameManager.Instance.SetLocked(true);
    }

    private void RefreshShop()
    {
        foreach (GameObject upgradeComponent in upgradeComponents)
            Destroy(upgradeComponent);

        upgradeComponents.Clear();

        foreach (UpgradeData upgradeData in UpgradeManager.Instance.upgrades)
        {
            ShopUpgrade upgradeComponent = Instantiate(upgradeComponentPrefab, upgradeComponentContainer);
            upgradeComponent.Init(upgradeData, OnBuyButtonClick);
            upgradeComponents.Add(upgradeComponent.gameObject);
        }
    }

    public void CloseShop()
    {
        foreach (GameObject upgradeComponent in upgradeComponents)
            Destroy(upgradeComponent);

        upgradeComponents.Clear();

        GameManager.Instance.SetLocked(false);
        gameObject.SetActive(false);
    }

    private void OnSellButtonClick()
    {
        currentShop.SellItems();
        RefreshShop();

        GameManager.Instance.hudController.UpdateInventory();
    }

    private void OnBuyButtonClick(UpgradeData upgradeData)
    {
        UpgradeManager.Instance.BuyUpgrade(upgradeData);
        RefreshShop();

        GameManager.Instance.hudController.UpdateInventory();
    }
}
