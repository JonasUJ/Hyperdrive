using UnityEngine;
using TMPro;

public class HitNumber : MonoBehaviour
{
    public float Number
    {
        get => float.Parse(text.text);
        set
        {
            if (text == null)
                text = GetComponent<TextMeshProUGUI>();
            text.text = (Mathf.FloorToInt(value * 10) / 10).ToString();
            text.fontSize = Utils.HitNumberSize(value);
        }
    }

    public Color Color
    {
        get => text.color;
        set
        {
            if (text == null)
                text = GetComponent<TextMeshProUGUI>();
            text.color = value;
        }
    }

    private TextMeshProUGUI text;
    private float spawnedAt;
    private float direcion;
    private bool increasing;
    private Vector3 startPos;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        spawnedAt = Time.timeSinceLevelLoad;
        direcion = Utils.HitNumberRandomDirection();
        increasing = direcion >= 0f;
        startPos = Camera.main.ScreenToWorldPoint(transform.position);
    }

    void Update()
    {
        float time = Time.timeSinceLevelLoad - spawnedAt;
        float alpha = Utils.HitNumberAlpha(time);
        if (alpha <= 0.001f)
            Destroy(gameObject);
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        transform.position = PositionFromTime(time);
    }

    private Vector3 PositionFromTime(float time)
    {
        time = (increasing ? time : -time) / 3;
        return Camera.main.WorldToScreenPoint(startPos + new Vector3(time, -20 * time * time + direcion * time, 0f));
    }
}
