using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : Bullet
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

    void Update()
    {
        float time = Time.timeSinceLevelLoad - spawnedAt;
        float alpha = Utils.HitNumberAlpha(time);
        if (alpha <= 0.001f)
            Destroy(gameObject);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
    }
}