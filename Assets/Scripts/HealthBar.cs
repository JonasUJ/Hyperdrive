using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject HitNumber;
    private Image img;
    private Vector3 offset;
    private HealthManager hpMngr;
    private Character origin;
    public Character Origin
    {
        get => origin;
        set
        {
            if (origin != null)
                origin.HealthChanged -= ChangeFillAmount;

            origin = value;
            hpMngr = origin.GetComponent<HealthManager>();

            origin.HealthChanged += ChangeFillAmount;
            offset = new Vector3(0f, Utils.GUISpriteObjectScreenPixelSize(origin.gameObject).y / 4, 0f);

            if (img == null)
                img = GetComponent<Image>();
            transform.localScale = Vector3.one / Camera.main.orthographicSize * 3f;
        }
    }

    void Start()
    {
        img = GetComponent<Image>();
        img.enabled = false;
    }

    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(origin.transform.position) + offset;
    }

    void ChangeFillAmount(object sender, HealthChangedEventArgs e)
    {
        img.fillAmount = hpMngr.PercentHealth;
        img.enabled = e.NewHealth != origin.MaxHealth;
        hpMngr.SpawnHitNumber();
    }
}
