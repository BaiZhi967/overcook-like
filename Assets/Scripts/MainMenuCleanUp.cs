using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

public class MainMenuCleanUp : MonoBehaviour
{

    private void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (KitchenGameMultiplayer.Instance != null)
        {
            Destroy(KitchenGameMultiplayer.Instance.gameObject);
        }
    }


}
