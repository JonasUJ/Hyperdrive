using UnityEngine;

public class Bullet : MonoBehaviour
{
    public string HitsTag;
    public string WallLayerName;
    public float BulletSpeed;
    public float BulletDamage;
    public BulletType Type;
    protected LayerMask wallLayer;
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected float spawnedAt;

    void Start()
    {
        spawnedAt = Time.timeSinceLevelLoad;
        wallLayer = LayerMask.NameToLayer(WallLayerName);
        spriteRenderer = GetComponent<SpriteRenderer>();
        switch (Type)
        {
            case BulletType.Blue:
                BulletDamage += Utils.Instance.PlayerData.Stats.BlueDamage;
                break;
            case BulletType.Green:
                BulletDamage += Utils.Instance.PlayerData.Stats.GreenDamage;
                break;
            case BulletType.Red:
                BulletDamage += Utils.Instance.PlayerData.Stats.RedDamage;
                break;
            case BulletType.Purple:
                BulletDamage += Utils.Instance.PlayerData.Stats.PurpleDamage;
                break;
            case BulletType.Fire:
                BulletDamage += Utils.Instance.PlayerData.Stats.FireDamage;
                break;
        }
        BulletDamage *= Utils.Instance.PlayerData.Stats.Strength;
    }

    public void Init(Transform origin)
    {
        Init(origin, 0f);
    }

    public void Init(Transform origin, float SpreadAngle)
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = origin.transform.position;
        transform.eulerAngles = (Random.value - 0.5f) * SpreadAngle * Vector3.forward + origin.eulerAngles;
        rb.velocity = transform.up * BulletSpeed;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == HitsTag)
            other.GetComponent<HealthManager>().DealDamageTo(BulletDamage);
        Destroy(gameObject);
    }
}

public enum BulletType
{
    Green,
    Blue,
    Red,
    Purple,
    Fire,
    Enemy,
    Rocket,
    None
}
