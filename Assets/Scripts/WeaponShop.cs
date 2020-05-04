using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponShop : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public int sprNumber;
    public ShopHandler shop;
    private Weapon weapon;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameObject prefab = ShopHandler.weapons[Mathf.RoundToInt(Random.value * (sprNumber - 1))];
        spriteRenderer.sprite = prefab.GetComponent<Weapon>().Icon;
        weapon = Utils.GetWeapon(spriteRenderer.sprite);
    }

    void OnMouseOver()
    {
        shop.ItemText.text = weapon.Description;
        shop.TitleText.text = weapon.Name;
        shop.PriceText.text = "Price: " + weapon.Price.ToString();
        shop.ItemPicture.sprite = weapon.Icon;
    }

    void OnMouseExit() {
        shop.ClearDisplay();
    }

    void OnMouseDown()
    {
        if (Utils.Instance.PlayerData.Gold >= weapon.Price)
        {
            if (Utils.Instance.PlayerData.Left == null)
            {
                Utils.Instance.PlayerData.Gold -= weapon.Price;
                Utils.Instance.PlayerData.Left = weapon;
                shop.UpdateGold();
                Destroy(gameObject);
            }
            else if (Utils.Instance.PlayerData.Right == null)
            {
                Utils.Instance.PlayerData.Gold -= weapon.Price;
                Utils.Instance.PlayerData.Right = weapon;
                shop.UpdateGold();
                Destroy(gameObject);
            }
            shop.UpdateWeaponDisplay();
        }
    }
}
