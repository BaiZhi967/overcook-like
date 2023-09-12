using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseMessageUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI msgText;
    [SerializeField] private Button closeBtm;

    private void Awake()
    {
        closeBtm.onClick.AddListener(Hide);
    }
    
    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnOnFailedToJoinGame;
        Hide();
    }

    private void KitchenGameMultiplayer_OnOnFailedToJoinGame(object sender, EventArgs e)
    {
        Show();
        msgText.text = NetworkManager.Singleton.DisconnectReason;
        if (msgText.text == "")
        {
            msgText.text = "Connection failed";
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnOnFailedToJoinGame;
    }
}
