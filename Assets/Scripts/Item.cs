using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public ShopHandler shop;
    private Info.ItemInfo itemInfo;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemInfo = Utils.GetItemInfo(Utils.Instance.ItemSprites[Mathf.RoundToInt(Random.value * (Utils.Instance.ItemSprites.Length - 1))]);
        spriteRenderer.sprite = itemInfo.Icon;
    }

    void OnMouseOver()
    {
        shop.ItemText.text = itemInfo.Description;
        shop.TitleText.text = itemInfo.Name;
        shop.PriceText.text = "Price: " + itemInfo.Price.ToString();
        shop.ItemPicture.sprite = itemInfo.Icon;
        shop.ItemStatText.text = Utils.GetItemStatText(itemInfo);
    }

    void OnMouseExit() {
        shop.ClearDisplay();
    }

    void OnMouseDown()
    {
        if (Utils.Instance.PlayerData.Gold >= itemInfo.Price)
        {
            Utils.Instance.PlayerData.Gold -= itemInfo.Price;
            Utils.Instance.PlayerData.Items.Add(itemInfo);
            shop.UpdateItemDisplay();
            shop.UpdateGold();
            Utils.Instance.UpdateStats();
            shop.UpdateStatDisplay();
            Destroy(gameObject);
        }
    }
}
