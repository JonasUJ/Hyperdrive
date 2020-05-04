using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class healButton : MonoBehaviour
{
    public float price = 0f;
    public float hpBack = 0f;
    public TextMeshProUGUI GoldText;
    public ShopHandler Shop;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void OnMouseOver()
    {
        Shop.TitleText.text = "Heal";
        Shop.ItemPicture.sprite = spriteRenderer.sprite;
        Shop.PriceText.text = "Price: " + price.ToString();
        Shop.ItemText.text = "Regain " + hpBack + " HP.";
    }

    void OnMouseExit()
    {
        Shop.ClearDisplay();
    }

    void OnMouseDown()
    {
        if (Utils.Instance.PlayerData.Gold >= price && Utils.Instance.PlayerData.Health != Utils.Instance.PlayerData.MaxHealth)
        {
            Utils.Instance.PlayerData.Gold -= price;
            Shop.UpdateGold();
            Utils.Instance.PlayerData.Health += hpBack;
            if (Utils.Instance.PlayerData.Health > Utils.Instance.PlayerData.MaxHealth)
            {
                Utils.Instance.PlayerData.Health = Utils.Instance.PlayerData.MaxHealth;
            }
            Shop.UpdateStatDisplay();
        }
    }
}
