using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoldNumber : MonoBehaviour
{
    public float Number
    {
        get => float.Parse(text.text);
        set
        {
            if (text == null)
                text = GetComponent<TextMeshProUGUI>();
            text.text = (Mathf.FloorToInt(value * 10) / 10.0f).ToString();

            if (goldIcon == null)
                goldIcon = GetComponentInChildren<Image>();
            goldIcon.sprite = Utils.GetGoldIcon(value);
        }
    }

    private Image goldIcon;
    private TextMeshProUGUI text;
    private float spawnedAt;
    private Vector3 startPos;

    void Start()
    {
        goldIcon = GetComponentInChildren<Image>();
        text = GetComponent<TextMeshProUGUI>();
        spawnedAt = Time.timeSinceLevelLoad;
        startPos = Camera.main.ScreenToWorldPoint(transform.position);
    }

    void Update()
    {
        float time = Time.timeSinceLevelLoad - spawnedAt;
        float alpha = Utils.HitNumberAlpha(time);
        if (alpha <= 0.001f)
            Destroy(gameObject);
        Color c = new Color(text.color.r, text.color.g, text.color.b, alpha);
        text.color = c;
        goldIcon.color = c;
        transform.position = Camera.main.WorldToScreenPoint(startPos + Vector3.up * Time.deltaTime * time * 30f);
    }
}
