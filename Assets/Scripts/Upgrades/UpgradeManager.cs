using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public List<UpgradeData> upgrades = new List<UpgradeData>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateUpgrades();
    }

    private void CreateUpgrades()
    {
        upgrades.Add(new UpgradeData { upgrade = Upgrade.Battery, name = "Battery Upgrade", cost = 10f });
        upgrades.Add(new UpgradeData { upgrade = Upgrade.Recharge, name = "Battery Recharge", cost = 0f });
        upgrades.Add(new UpgradeData { upgrade = Upgrade.SolarPanel, name = "Solar Recharger", cost = 30f });

        upgrades.Add(new UpgradeData { upgrade = Upgrade.Range, name = "Range Upgrade", cost = 10f });
        upgrades.Add(new UpgradeData { upgrade = Upgrade.Backpack, name = "Backpack Upgrade", cost = 5f });
        //upgrades.Add(new UpgradeData { upgrade = Upgrade.WalletSize,        name = "Wallet Size",       cost = 10f });
        //upgrades.Add(new UpgradeData { upgrade = Upgrade.Rarity,            name = "Rarity Boost",      cost = 25f });

        upgrades.Add(new UpgradeData { upgrade = Upgrade.SilverDetector, name = "Silver Detector", cost = 15f });
        upgrades.Add(new UpgradeData { upgrade = Upgrade.GoldDetector, name = "Gold Detector", cost = 35f });

        upgrades.Add(new UpgradeData { upgrade = Upgrade.Roomba, name = "For legal reasons not a roomba", cost = 9999f });
        //upgrades.Add(new UpgradeData { upgrade = Upgrade.HeartbeatSensor,   name = "Heartbeat Sensor",  cost = 1f });
        //upgrades.Add(new UpgradeData { upgrade = Upgrade.Fossils,           name = "Fossil Finder",     cost = 1f });
        //upgrades.Add(new UpgradeData { upgrade = Upgrade.AlienTech,         name = "Alien Tech",        cost = 1f });
    }

    //TODO: We probably want to rewrite these once this is event driven. 
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
                RemoveUpgrade(upgradeData.upgrade);
                break;

            case Upgrade.Recharge:
                //Do nothing. Just don't call default
                break;

            default:
                for (int i = 0; i < upgrades.Count; i++)
                {
                    if (upgrades[i].upgrade == upgradeData.upgrade)
                    {
                        UpgradeData temp = upgrades[i];
                        temp.cost *= 1.5f; // increase for next time
                        upgrades[i] = temp;
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
                GameManager.Instance.metalDetector.maxBattery += 50f;
                GameManager.Instance.metalDetector.RechargeBattery(GameManager.Instance.metalDetector.maxBattery);
                break;
            case Upgrade.Recharge:
                GameManager.Instance.metalDetector.RechargeBattery(GameManager.Instance.metalDetector.maxBattery);
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
                GameManager.Instance.metalDetector.rechargeRate = 1f;
                break;
            case Upgrade.SilverDetector:
                SpawnerManager.Instance.EnableSilver();
                break;

            default:
                Debug.LogError("Upgrade " + upgrade.ToString() + " is not yet implemented");
                break;
        }
    }

    public void RemoveUpgrade(Upgrade upgradeType)
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            if (upgrades[i].upgrade == upgradeType)
            {
                upgrades.RemoveAt(i);
                return;
            }
        }
    }
}
