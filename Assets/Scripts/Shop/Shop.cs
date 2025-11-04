using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shop : MonoBehaviour
{
    public LayerMask shopMask;
    public float range = 3f;

    private Keyboard keyboard;
    public MetalDetector metalDetector;

    private void Start()
    {
        keyboard = Keyboard.current;
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.Instance.LOCKED)
        {
            GameManager.Instance.hudController.ShowInteractPrompt(false);
            GameManager.Instance.hudController.ShowRadioPrompt(false);
            return;
        }

        Physics.Raycast(GameManager.Instance.cameraController.playerCamera.transform.position, GameManager.Instance.cameraController.playerCamera.transform.forward, out RaycastHit hitInfo, range, shopMask);
        if (hitInfo.collider == null)
        {
            GameManager.Instance.hudController.ShowInteractPrompt(false);
            return;
        }

        GameManager.Instance.hudController.ShowInteractPrompt(true);

        if (keyboard.eKey.wasPressedThisFrame)
            OpenShop();;
    }

    private void OpenShop()
    {
        GameManager.Instance.hudController.ShowShop(this);
        metalDetector = FindFirstObjectByType<MetalDetector>();
    }

    public float SellItems()
    {
        return GameManager.Instance.inventory.SellAll();
    }

    public void BuyUpgrade(UpgradeData upgradeData) 
    {
        if (GameManager.Instance.inventory.GetMoney() < upgradeData.cost)
            return;

        GameManager.Instance.inventory.DeductMoney(upgradeData.cost);

        switch (upgradeData.upgrade)
        {
            case Upgrade.SilverDetector:
            case Upgrade.SolarPanel:
            case Upgrade.GoldDetector:
                GameManager.Instance.RemoveUpgrade(upgradeData.upgrade);
                break;

            case Upgrade.Recharge:
                //Do nothing. Just don't call default
                break;

            default:
                for (int i = 0; i < GameManager.Instance.upgrades.Count; i++)
                {
                    if (GameManager.Instance.upgrades[i].upgrade == upgradeData.upgrade)
                    {
                        UpgradeData temp = GameManager.Instance.upgrades[i];
                        temp.cost *= 1.5f; // increase for next time
                        GameManager.Instance.upgrades[i] = temp;
                        break;
                    }
                }
                break;
        }

        ApplyUpgrade(upgradeData.upgrade);  
    }

    private void ApplyUpgrade(Upgrade upgrade)
    {
        switch (upgrade)
        {
            case Upgrade.Range:
                GameManager.Instance.metalDetector.range += 2;
                Debug.Log("Range: " + GameManager.Instance.metalDetector.range);
                break;
            case Upgrade.Battery:
                metalDetector.maxBattery += 50f;
                metalDetector.RechargeBattery(metalDetector.maxBattery);
                break;
            case Upgrade.Recharge:
                metalDetector.RechargeBattery(metalDetector.maxBattery);
                break;

            case Upgrade.Backpack:
                GameManager.Instance.inventory.maxSize += 1;
                Debug.Log(GameManager.Instance.inventory.maxSize);
                break;

            case Upgrade.WalletSize:
                GameManager.Instance.inventory.maxMoney *= 2;
                Debug.Log(GameManager.Instance.inventory.maxMoney);
                break;
                
            /*case Upgrade.Rarity:
                foreach(Spawner spawner in spawners)
                {
                    foreach(Item item in spawner.items)
                    {
                        //TODO: We probably want to do this in a better way
                        if(item.rarity == 1)
                        {
                            item.rarity *= 2;
                        }
                        else
                        {
                            item.rarity /= 2;
                        }
                    }
                }
                break;*/
            case Upgrade.GoldDetector:
                SpawnerManager.Instance.EnableGold();
                break;

            case Upgrade.HeartbeatSensor:
                Debug.LogError("Upgrade " + upgrade.ToString() + " is not yet implemented");
                break;

            case Upgrade.Fossils:
                Debug.LogError("Upgrade " + upgrade.ToString() + " is not yet implemented");
                break;

            case Upgrade.AlienTech:
                Debug.LogError("Upgrade " + upgrade.ToString() + " is not yet implemented");
                break;
            case Upgrade.SolarPanel:
                metalDetector.rechargeRate = 1f;
                break;
            case Upgrade.SilverDetector:
                SpawnerManager.Instance.EnableSilver();
                break;

            default:
                Debug.LogError("Upgrade " + upgrade.ToString() + " is not yet implemented");
                break;
        }
    }
} 
