using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class maxhpButton : MonoBehaviour
{
    public float price = 0f;
    public float hpPlus = 0f;
    public TextMeshProUGUI GoldText;
    public ShopHandler Shop;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void OnMouseOver()
    {
        Shop.TitleText.text = "Max HP up";
        Shop.ItemPicture.sprite = spriteRenderer.sprite;
        Shop.PriceText.text = "Price: " + price.ToString();
        Shop.ItemText.text = "Add " + hpPlus + " HP to your max health.";
    }

    void OnMouseExit()
    {
        Shop.ClearDisplay();
    }

    void OnMouseDown()
    {
        if (Utils.Instance.PlayerData.Gold >= price)
        {
            Utils.Instance.PlayerData.Gold -= price;
            Shop.UpdateGold();
            Utils.Instance.PlayerData.MaxHealth += hpPlus;
            Utils.Instance.PlayerData.Health += hpPlus;
            Shop.UpdateStatDisplay();
        }
    }
}
