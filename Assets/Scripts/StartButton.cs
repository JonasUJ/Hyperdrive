using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    void Start()
    {
        Destroy(GameObject.Find("Utils"));
    }

    void OnMouseDown()
    {
        SceneManager.LoadScene("PlayerTestScene");
    }
}
