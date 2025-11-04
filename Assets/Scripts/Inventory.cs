using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    List<ItemData> items = new List<ItemData>();
    public int maxSize = 2;
    private float money = 0f;
    public float maxMoney = 1000f;

    public bool AddItem(ItemData item)
    {
        if(items.Count < maxSize)
        {
            items.Add(item);
            return true;
        }
        return false;
    }

    // cursed method, do not use
    //public bool RemoveItem()
    //{
    //    if (items.Count > 0)
    //    {
    //        ItemData item = items[0];
    //        items.RemoveAt(0);
    //
    //        money += item.Value;
    //        GameManager.Instance.hudController.UpdateMoneyText(money);
    //
    //        return true;
    //    }
    //
    //    return false;
    //}

    public float SellAll()
    {
        float gains = 0f;

        foreach (ItemData item in items)
        {
            gains += item.Value;
            GameManager.Instance.itemCollection.AddItem(item);
        }
        items.Clear();

        money += gains;
        if(gains > 0f)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Instance.coinClips);
        }
        GameManager.Instance.hudController.UpdateMoneyText(money);

        return gains;
    }

    public void IncreaseInventoryCapacity(int increment = 1)
    {
        if(increment <= 0)
        {
            Debug.LogError("Error: Trying to increase inventory by invalid amount: " + increment);
            return;
        }
        maxSize += increment;
    }

    public ItemData GetItem(int index)
    {
        if(index < 0 || index >= items.Count)
        {
            Debug.LogError("Error: Index out of bounds.");
        }
        return items[index];
    }

    public int GetCurrentSize()
    {
        return items.Count;
    }

    public float GetMoney()
    {
        return money;
    }

    public void DeductMoney(float cost)
    {
        if (money - cost < 0f)
        {
            Debug.LogWarning("Trying to set negative amount.");
            return;
        }

        this.money -= cost;

        GameManager.Instance.hudController.UpdateMoneyText(money);
    }

    public bool IsFull()
    {
        return items.Count >= maxSize;
    }

    public List<Sprite> GetInventorySprites()
    {
        List<Sprite> sprites = new List<Sprite>();
        foreach (ItemData item in items)
            sprites.Add(item.Sprite);
        return sprites;
    }
}
