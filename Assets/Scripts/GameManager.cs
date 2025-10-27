using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public AudioManager audioManager;

    public PlayerController playerController;
    public CameraController cameraController;
    public HudController hudController;

    public Inventory inventory;
    public ItemCollection itemCollection;
    public MetalDetector metalDetector;
    public List<UpgradeData> upgrades = new List<UpgradeData>();

    private Keyboard keyboard;

    public bool LOCKED = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        keyboard = Keyboard.current;
        audioManager.Init();

        inventory = FindFirstObjectByType<Inventory>();
        metalDetector = FindFirstObjectByType<MetalDetector>();

        upgrades.Add(new UpgradeData { upgrade = Upgrade.Battery,           name = "Battery Upgrade",   cost = 10f });
        upgrades.Add(new UpgradeData { upgrade = Upgrade.Recharge,          name = "Battery Recharge", cost = 0f });
        upgrades.Add(new UpgradeData { upgrade = Upgrade.SolarPanel,        name = "Solar Recharger", cost = 30f });

        upgrades.Add(new UpgradeData { upgrade = Upgrade.Range,             name = "Range Upgrade",     cost = 10f });
        upgrades.Add(new UpgradeData { upgrade = Upgrade.Backpack,          name = "Backpack Upgrade",  cost = 5f });
        //upgrades.Add(new UpgradeData { upgrade = Upgrade.WalletSize,        name = "Wallet Size",       cost = 10f });
        //upgrades.Add(new UpgradeData { upgrade = Upgrade.Rarity,            name = "Rarity Boost",      cost = 25f });
        
        upgrades.Add(new UpgradeData { upgrade = Upgrade.SilverDetector,    name = "Silver Detector", cost = 15f });
        upgrades.Add(new UpgradeData { upgrade = Upgrade.GoldDetector,      name = "Gold Detector",     cost = 35f });
        //upgrades.Add(new UpgradeData { upgrade = Upgrade.HeartbeatSensor,   name = "Heartbeat Sensor",  cost = 1f });
        //upgrades.Add(new UpgradeData { upgrade = Upgrade.Fossils,           name = "Fossil Finder",     cost = 1f });
        //upgrades.Add(new UpgradeData { upgrade = Upgrade.AlienTech,         name = "Alien Tech",        cost = 1f });

        hudController.UpdateMoneyText(inventory.GetMoney());
        hudController.UpdateBatteryText(metalDetector.maxBattery);
        hudController.UpdateInventory();

        SetLocked(false);
    }

    private void Update()
    {
        if (keyboard.tabKey.wasPressedThisFrame)
        {
            hudController.pauseMenu.Toggle();
        }
    }

    public void DestroySelf()
    {
        Time.timeScale = 1;

        Destroy(gameObject);
    }

    public void SetLocked(bool locked)
    {
        LOCKED = locked;
        Cursor.visible = locked;
        Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
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
