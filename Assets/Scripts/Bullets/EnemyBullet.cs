using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{
    public string CrystalTag = "Crystal";
    protected override void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == HitsTag ||other.gameObject.tag == CrystalTag)
        {
            other.GetComponent<HealthManager>().DealDamageTo(BulletDamage);
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == wallLayer)
        {
            Destroy(gameObject);
        }
    }
}
