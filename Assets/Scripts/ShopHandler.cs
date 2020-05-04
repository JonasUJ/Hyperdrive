using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopHandler : MonoBehaviour
{
    public GameObject ItemPrefab;
    public GameObject WeaponPrefab;
    public SpriteRenderer ItemPicture;
    public TextMeshProUGUI GoldText;
    public Image GoldImage;
    public TextMeshProUGUI ItemText;
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI PriceText;
    public TextMeshProUGUI BoughtItems;
    public TextMeshProUGUI ItemStatText;
    public TextMeshProUGUI StatText;
    public float Weapon1Position;
    public float Weapon2Position;
    public float Item1Position;
    public float Item2Position;
    public float Item3Position;
    public static GameObject[] weapons;
    public SpriteRenderer left;
    public SpriteRenderer right;

    void Start()
    {
        GoldText.text = "Gold: " + Utils.Instance.PlayerData.Gold.ToString();

        LoadWeapons();

        Item item;
        item = Instantiate(ItemPrefab, new Vector3(Item1Position, 0.9f, -1f), Quaternion.identity).GetComponent<Item>();
        item.shop = this;
        item = Instantiate(ItemPrefab, new Vector3(Item2Position, 0.9f, -1f), Quaternion.identity).GetComponent<Item>();
        item.shop = this;
        item = Instantiate(ItemPrefab, new Vector3(Item3Position, 0.9f, -1f), Quaternion.identity).GetComponent<Item>();
        item.shop = this;

        WeaponShop weapon;
        weapon = Instantiate(WeaponPrefab, new Vector3(Weapon1Position, 0.9f, -1f), Quaternion.identity).GetComponent<WeaponShop>();
        weapon.shop = this;
        weapon = Instantiate(WeaponPrefab, new Vector3(Weapon2Position, 0.9f, -1f), Quaternion.identity).GetComponent<WeaponShop>();
        weapon.shop = this;

        UpdateWeaponDisplay();
        UpdateItemDisplay();
        UpdateStatDisplay();
        UpdateGold();
    }

    public void UpdateWeaponDisplay()
    {
        left.sprite = Utils.Instance.PlayerData.Left == null ? null : Utils.Instance.PlayerData.Left.Icon;
        right.sprite = Utils.Instance.PlayerData.Right == null ? null : Utils.Instance.PlayerData.Right.Icon;
    }

    public void UpdateItemDisplay()
    {
        StringBuilder sb = new StringBuilder();
        Dictionary<string, int> dict = new Dictionary<string, int>();
        foreach (Info.ItemInfo ii in Utils.Instance.PlayerData.Items)
        {
            if (dict.ContainsKey(ii.Name))
                dict[ii.Name]++;
            else
                dict.Add(ii.Name, 1);
        }
        foreach (var kv in dict)
        {
            sb.Append(kv.Value);
            sb.Append("x ");
            sb.Append(kv.Key);
            sb.Append(", ");
        }
        BoughtItems.text = sb.ToString();
    }

    public void UpdateStatDisplay()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Health: ");
        sb.Append(Utils.Instance.PlayerData.Health);
        sb.Append("\n");
        sb.Append("Max Health: ");
        sb.Append(Utils.Instance.PlayerData.MaxHealth);
        sb.Append("\n");
        sb.Append(Utils.GetItemStatText(Utils.Instance.PlayerData.Stats));
        StatText.text = sb.ToString();
    }

    public void UpdateGold()
    {
        GoldText.text = "Gold: " + Utils.Instance.PlayerData.Gold.ToString();
        GoldImage.sprite = Utils.GetTotalGoldIcon(Utils.Instance.PlayerData.Gold);
    }

    public void ClearDisplay()
    {
        ItemText.text = "";
        TitleText.text = "";
        PriceText.text = "";
        ItemStatText.text = "";
        ItemPicture.sprite = null;
    }

    void LoadWeapons()
    {
        object[] loadedWeapons = Resources.LoadAll("Prefabs/Weapons", typeof(GameObject));
        weapons = new GameObject[loadedWeapons.Length];
        for(int x = 0; x < loadedWeapons.Length; x++)
        {
            weapons[x] = (GameObject)loadedWeapons[x];
        }
    }
}


