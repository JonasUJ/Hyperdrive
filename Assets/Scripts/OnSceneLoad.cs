using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSceneLoad : MonoBehaviour
{
    void Start()
    {
        Utils.Instance.SceneLoad();
        Utils.Instance.NextWave();
    }
}
