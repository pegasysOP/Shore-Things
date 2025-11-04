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
        
        UpdateUI();

        SetLocked(false);
    }

    private void UpdateUI()
    {
        hudController.UpdateMoneyText(inventory.GetMoney());
        hudController.UpdateBatteryText(metalDetector.maxBattery);
        hudController.UpdateInventory();
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
}
