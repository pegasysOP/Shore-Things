using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shop : MonoBehaviour
{
    public LayerMask shopMask;
    public float range = 3f;

    private Keyboard keyboard;

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
    }

    public float SellItems()
    {
        return GameManager.Instance.inventory.SellAll();
    }
} 
