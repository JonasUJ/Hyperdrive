using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XButton : MonoBehaviour
{
    public bool IsLeft;
    public ShopHandler shop;

    void OnMouseDown()
    {
        if (IsLeft)
            Utils.Instance.PlayerData.Left = null;
        else
            Utils.Instance.PlayerData.Right = null;

        shop.UpdateWeaponDisplay();
    }
}
