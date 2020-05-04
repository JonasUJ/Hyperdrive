using System;
using UnityEngine;

public class Character : HealthManager
{
    public GameObject HealthBarVarient;
    public bool HasLineOfSight;
    public float GoldValue;
    public GameObject GoldNumber;
    private HealthBar healthBarReference;
    protected void InitHealthBar()
    {
        healthBarReference = Instantiate(this.HealthBarVarient, Utils.GUICanvas.transform).GetComponent<HealthBar>();
        HealthChanged += TookDamage;
        healthBarReference.Origin = this;
        GoldValue *= 1 + Utils.Instance.PlayerData.Stats.Money;
    }

    private void SpawnGoldNumber()
    {
        GoldNumber gold = Instantiate(GoldNumber, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity, Utils.GUICanvas.transform).GetComponent<GoldNumber>();
        gold.Number = GoldValue;
        Utils.Instance.AddGold(GoldValue);
    }

    private void TookDamage(object sender, HealthChangedEventArgs e)
    {
        if (e.CausedDeath)
        {
            Utils.AddDeath();
            SpawnGoldNumber();
            Destroy(healthBarReference.gameObject);
            Destroy(gameObject);
        }
    }
}
