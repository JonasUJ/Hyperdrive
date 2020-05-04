using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperBullet : Bullet
{
    protected override void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == HitsTag)
        {
            other.GetComponent<HealthManager>().DealDamageTo(BulletDamage);
        }
        else if (other.gameObject.layer == wallLayer)
        {
            Destroy(gameObject);
        }
    }
}