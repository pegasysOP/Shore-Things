using System;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public Collider col;
    public int digTimes = 3;

    private MeshRenderer mesh;

    private ItemData itemData;
    private float rangeMultiplier;
    private Action<ItemObject> onDestroy;

    public ItemData ItemData { get { return itemData; } }
    public float RangeMultipler { get { return rangeMultiplier; } }

    public void Init(Item itemInfo, Action<ItemObject> onDestroy)
    {
        this.itemData.Name = itemInfo.displayName;
        this.itemData.Description = itemInfo.description;
        this.itemData.Value = itemInfo.GetValue();
        this.itemData.Sprite = itemInfo.icon;
        this.rangeMultiplier = itemInfo.rangeMultiplier;

        this.onDestroy = onDestroy;

        mesh = GetComponent<MeshRenderer>();
    }

    public void OnInteract()
    {
        if (digTimes > 1)
        {
            digTimes--;
            AudioManager.Instance.PlaySfxWithPitchShifting(AudioManager.Instance.digSandClips);

            // move up a bit
            transform.position += new Vector3(0f, GetItemHeight() / 3f, 0f);

            return;
        }

        if (GameManager.Instance.inventory.IsFull())
        {
            GameManager.Instance.hudController.inventoryPanel.Flash();
            return;
        }

        // collect
        bool wasSuccessful = GameManager.Instance.inventory.AddItem(ItemData);
        GameManager.Instance.hudController.UpdateInventory();
        AudioManager.Instance.PlaySfx(AudioManager.Instance.coinClips);

        onDestroy?.Invoke(this);

        Destroy(gameObject);
    }

    public float GetItemHeight()
    {
        if (mesh == null)
            return 0f;
        return mesh.bounds.size.y;
    }
}
