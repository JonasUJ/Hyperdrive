using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunWeapon : Weapon
{
    public override IEnumerator Fire()
    {
        StartCoroutine(GoOnCooldown());
        for (int i = 0; i < BulletsInVolley + Utils.Instance.PlayerData.Stats.ShotgunShells; i++)
        {
            if (Ammo <= 0)
            {
                StartCoroutine(Reload());
                yield break;
            }
            Ammo--;
            GameObject bullet = Instantiate(BulletPrefab);
            bullet.GetComponent<Bullet>().Init(NozzlePosition, BulletSpreadAngle);
            if (TimeBetweenBullets > 0)
                yield return new WaitForSeconds(TimeBetweenBullets);
        }
        yield return new WaitForSeconds(0);
        if (Ammo <= 0)
            StartCoroutine(Reload());
    }
}
