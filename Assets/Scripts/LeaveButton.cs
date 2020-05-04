using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveButton : MonoBehaviour
{
    void OnMouseDown()
    {
        Utils.Instance.ExitShop();
    }
}
