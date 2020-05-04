using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndSceneWaveText : MonoBehaviour
{
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = "Died on wave " + Utils.Instance.PlayerData.Wave;
    }
}
