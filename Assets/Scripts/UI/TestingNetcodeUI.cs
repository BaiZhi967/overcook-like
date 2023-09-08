using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class TestingNetcodeUI : MonoBehaviour
{
    [SerializeField] private Button startHostBtm;
    [SerializeField] private Button startClientBtm;

    private void Awake()
    {
        startHostBtm.onClick.AddListener(() =>
        {
            Debug.Log("Start Host");
            KitchenGameMultiplayer.Instance.StartHost();
            Hide();
        });
        startClientBtm.onClick.AddListener(() =>
        {
            Debug.Log("Start Client");
            KitchenGameMultiplayer.Instance.StartClient();
            Hide();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
