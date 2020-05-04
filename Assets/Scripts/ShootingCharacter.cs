using System;
using System.Collections;
using UnityEngine;

public class ShootingCharacter : Character
{
    public AudioSource src;
    public Weapon EquippedWeapon;
    public Transform Nozzle;
    void Start()
    {
        InitHealthBar();
        EquippedWeapon = Instantiate(EquippedWeapon, transform);
        EquippedWeapon.NozzlePosition = Nozzle;
    }

    void Update()
    {
        if (HasLineOfSight && EquippedWeapon.CanFire)
        {
            StartCoroutine(EquippedWeapon.Fire());
        }
    }
}
