using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject shotgun;
    public bool isDangerous = false;
    public bool alertAll = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Guns out (all who see you will be alerted)");
            shotgun.SetActive(true);
            isDangerous = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Going loud (all alerted to your presence)");
            shotgun.SetActive(true);
            alertAll = true;
        }
    }

    public bool GetPlayerDangerous()
    {
        return isDangerous;
    }

    public bool GetAlertAll()
    {
        return alertAll;
    }
}
