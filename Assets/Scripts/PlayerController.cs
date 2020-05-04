using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Range(0f, 1f)]
    public float TurnSpeed;
    public Image HealthBarImage;
    public TextMeshProUGUI HealthText;
    public Image AmmoLeftImage;
    public TextMeshProUGUI AmmoLeftText;
    public Image AmmoLeftBar;
    public Image AmmoLeftCooldownBar;
    public Image AmmoRightImage;
    public TextMeshProUGUI AmmoRightText;
    public Image AmmoRightBar;
    public Image AmmoRightCooldownBar;
    private float walkSpeed;
    private float maxSpeed;
    private float shootingWalkSpeed;
    private HealthManager hpMngr;
    private Rigidbody2D body;
    private Weapon leftWeapon;
    public Weapon LeftWeaponUnity;
    public Weapon LeftWeapon
    {
        get => leftWeapon;
        set
        {
            if (leftWeapon != null)
                Destroy(leftWeapon.gameObject);

            if (value == null)
            {
                AmmoLeftImage.gameObject.SetActive(false);
                return;
            }

            AmmoLeftImage.gameObject.SetActive(true);
            AmmoLeftImage.sprite = value.Icon;
            leftWeapon = Instantiate(value, transform);
            leftWeapon.NozzlePosition = LeftHand;
            leftWeapon.AmmoChanged += UpdateLeftAmmo;
            leftWeapon.CooldownChanged += UpdateLeftCooldown;
        }
    }
    public Transform LeftHand;
    private Weapon rightWeapon;
    public Weapon RightWeaponUnity;
    public Weapon RightWeapon
    {
        get => rightWeapon;
        set
        {
            if (rightWeapon != null)
                Destroy(rightWeapon.gameObject);

            if (value == null)
            {
                AmmoRightImage.gameObject.SetActive(false);
                return;
            }

            AmmoRightImage.gameObject.SetActive(true);
            AmmoRightImage.sprite = value.Icon;
            rightWeapon = Instantiate(value, transform);
            rightWeapon.NozzlePosition = RightHand;
            rightWeapon.AmmoChanged += UpdateRightAmmo;
            rightWeapon.CooldownChanged += UpdateRightCooldown;
        }
    }
    public Transform RightHand;

    void Start()
    {
        hpMngr = GetComponent<HealthManager>();
        body = GetComponent<Rigidbody2D>();
        walkSpeed = 5 * Utils.Instance.PlayerData.Stats.Speed;

        hpMngr.MaxHealth = Utils.Instance.PlayerData.MaxHealth;
        hpMngr.Health = Utils.Instance.PlayerData.Health;

        hpMngr.HealthChanged += TakeDamage;
        HealthText.text = Utils.FormatHealth(hpMngr);
        HealthBarImage.fillAmount = hpMngr.PercentHealth;

        LeftWeapon = Utils.Instance.PlayerData.Left;
        RightWeapon = Utils.Instance.PlayerData.Right;
    }

    void Update()
    {
        // Check if weapons should be fired
        void CheckFireWeapon(string key, Weapon weapon)
        {
            if (weapon != null && weapon.CanFire)
            {
                if (weapon.Continuous && Input.GetButton(key))
                    StartCoroutine(weapon.Fire());
                else if (Input.GetButtonDown(key))
                    StartCoroutine(weapon.Fire());
            }
        }

        CheckFireWeapon("FireLeft", LeftWeapon);
        CheckFireWeapon("FireRight", RightWeapon);

        // Movement
        Vector2 axisvec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        axisvec /= axisvec.magnitude != 0 ? Mathf.Sqrt(Mathf.Pow(axisvec.x, 2) + Mathf.Pow(axisvec.y, 2)) : 1;
        body.velocity = axisvec * walkSpeed;
        if (leftWeapon != null && leftWeapon.OnCooldown) body.velocity *= Utils.Instance.PlayerData.Stats.Dexterity;
        if (rightWeapon != null && rightWeapon.OnCooldown) body.velocity *= Utils.Instance.PlayerData.Stats.Dexterity;

        // Rotation
        transform.up = Vector2.Lerp(transform.up, (((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized), TurnSpeed);

        if (Input.GetButtonDown("Reload"))
        {
            if (LeftWeapon != null) StartCoroutine(LeftWeapon.Reload());
            if (RightWeapon != null) StartCoroutine(RightWeapon.Reload());
        }
    }

    void TakeDamage(object sender, HealthChangedEventArgs e)
    {
        Utils.Instance.PlayerData.Health = hpMngr.Health;
        HealthText.text = Utils.FormatHealth(hpMngr);
        hpMngr.SpawnHitNumber();
        HealthBarImage.fillAmount = hpMngr.PercentHealth;
        if (e.CausedDeath)
        {
            SceneManager.LoadScene("EndScene");
        }
    }

    void UpdateLeftAmmo(object sender, AmmoChangedEventArgs e)
    {
        AmmoLeftText.text = Utils.FormatAmmo(LeftWeapon);
        AmmoLeftBar.fillAmount = LeftWeapon.PercentAmmo;
    }

    void UpdateRightAmmo(object sender, AmmoChangedEventArgs e)
    {
        AmmoRightText.text = Utils.FormatAmmo(RightWeapon);
        AmmoRightBar.fillAmount = RightWeapon.PercentAmmo;
    }

    void UpdateLeftCooldown(object sender, CooldownChangedEventArgs e)
    {
        if (!LeftWeapon.Continuous)
            AmmoLeftCooldownBar.fillAmount = LeftWeapon.PercentCooldownLeft;
    }

    void UpdateRightCooldown(object sender, CooldownChangedEventArgs e)
    {
        if (!RightWeapon.Continuous)
            AmmoRightCooldownBar.fillAmount = RightWeapon.PercentCooldownLeft;
    }
}