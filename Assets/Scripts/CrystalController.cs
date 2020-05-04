using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CrystalController : MonoBehaviour
{
    public Image HealthBarImage;
    public TextMeshProUGUI HealthText;
    private HealthManager hpMngr;

    void Start()
    {
        hpMngr = GetComponent<HealthManager>();
        hpMngr.Health = Utils.Instance.PlayerData.CrystalHealth;
        hpMngr.HealthChanged += UpdateHealthBar;
        HealthText.text = Utils.FormatHealth(hpMngr);
        StartCoroutine(Regen());
    }

    void UpdateHealthBar(object sender, HealthChangedEventArgs e)
    {
        HealthText.text = Utils.FormatHealth(hpMngr);
        hpMngr.SpawnHitNumber();
        HealthBarImage.fillAmount = hpMngr.PercentHealth;
        if (e.CausedDeath)
            SceneManager.LoadScene("EndScene");
    }

    IEnumerator Regen()
    {
        float lastTimeHealed = Time.time;
        while (true)
        {
            if (lastTimeHealed + 5 < Time.time)
            {
                hpMngr.Heal(Utils.Instance.PlayerData.Stats.CrystalRegen);
                lastTimeHealed = Time.time;
            }
            yield return new WaitForSeconds(1);
        }
    }
}
