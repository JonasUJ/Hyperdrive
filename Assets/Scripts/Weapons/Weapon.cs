using System;
using System.Collections;
using UnityEngine;

public class Weapon : Info.WeaponInfo
{
    private float ammo;
    public float Ammo {
        get => ammo;
        protected set
        {
            float newAmmo = Math.Min(MaxAmmo, Math.Max(0f, value));
            AmmoChangedEventArgs e = new AmmoChangedEventArgs
            {
                OldAmmo = ammo,
                NewAmmo = newAmmo,
                AmmoChange = newAmmo - ammo,
                CausedDepletion = newAmmo == 0f
            };
            ammo = newAmmo;
            OnAmmoChanged(e);
        }
    }
    public float PercentAmmo { get => ammo / MaxAmmo; }

    public float CooldownLeft
    {
        get => cooldownLeft;
        set
        {
            float newCooldown = Math.Min(ShootingCooldown, Math.Max(0f, value));
            CooldownChangedEventArgs e = new CooldownChangedEventArgs
            {
                OldCooldown = cooldownLeft,
                NewCooldown = newCooldown,
                CooldownChange = newCooldown - cooldownLeft,
                CausedRenewal = newCooldown == 0f
            };
            cooldownLeft = newCooldown;
            OnCooldownChanged(e);
        }
    }
    public float PercentCooldownLeft { get => cooldownLeft / ShootingCooldown; }

    [HideInInspector]
    public Transform NozzlePosition;
    public BulletType BulletColor { get => BulletPrefab.GetComponent<Bullet>().Type; }
    public GameObject BulletPrefab;
    public WeaponType Type;
    public bool Reloading { get; protected set; }
    public float TimeOfCooldown { get; protected set; }
    private float cooldownLeft;
    public bool OnCooldown { get => TimeOfCooldown + ShootingCooldown > Time.time; }
    public bool CanFire { get => !OnCooldown && !Reloading; }
    public delegate void AmmoChangedEventHandler(object sender, AmmoChangedEventArgs e);
    public event AmmoChangedEventHandler AmmoChanged;
    public delegate void CooldownChangedEventHandler(object sender, CooldownChangedEventArgs e);
    public event CooldownChangedEventHandler CooldownChanged;

    void Start()
    {
        Ammo = MaxAmmo;
        switch (Type)
        {
            case WeaponType.Basic:
                break;
            case WeaponType.Green:
                break;
            case WeaponType.Red:
                break;
            case WeaponType.Sniper:
                ReloadTime *= Mathf.Max(0, 1 - Utils.Instance.PlayerData.Stats.SniperReload);
                ShootingCooldown *= Mathf.Max(0, 1 - Utils.Instance.PlayerData.Stats.SniperShootingSpeed);
                break;
            case WeaponType.Shotgun:
                ReloadTime *= Mathf.Max(0, 1 - Utils.Instance.PlayerData.Stats.ShotgunReload);
                break;
            case WeaponType.FlameThrower:
                ReloadTime *= Mathf.Max(0, 1 - Utils.Instance.PlayerData.Stats.FlameReload);
                break;
        }
    }

    protected virtual void OnAmmoChanged(AmmoChangedEventArgs e)
    {
        AmmoChangedEventHandler handler = AmmoChanged;
        handler?.Invoke(this, e);
    }

    protected virtual void OnCooldownChanged(CooldownChangedEventArgs e)
    {
        CooldownChangedEventHandler handler = CooldownChanged;
        handler?.Invoke(this, e);
    }

    public virtual IEnumerator Fire()
    {
        throw new NotImplementedException();
    }

    public IEnumerator Reload()
    {
        Reloading = true;
        while (Ammo < MaxAmmo)
        {
            yield return new WaitForSeconds(ReloadTime / MaxAmmo);
            Ammo++;
        }
        Reloading = false;
    }

    public IEnumerator GoOnCooldown()
    {
        TimeOfCooldown = Time.time;
        CooldownLeft = ShootingCooldown;
        while (OnCooldown)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            CooldownLeft = TimeOfCooldown + ShootingCooldown - Time.time;
        }
        CooldownLeft = 0;
    }
}

public class AmmoChangedEventArgs : EventArgs
{
    public float OldAmmo;
    public float NewAmmo;
    public float AmmoChange;
    public bool CausedDepletion;
}

public class CooldownChangedEventArgs : EventArgs
{
    public float OldCooldown;
    public float NewCooldown;
    public float CooldownChange;
    public bool CausedRenewal;
}

public enum WeaponType
{
    Basic,
    Green,
    Red,
    Sniper,
    Shotgun,
    FlameThrower,
    None,
}