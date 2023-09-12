using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnTryToJoinGame += KitchenGameMultiplayer_OnOnTryToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnOnFailedToJoinGame;
        Hide();
    }

    private void KitchenGameMultiplayer_OnOnFailedToJoinGame(object sender, EventArgs e)
    {
        Hide();
    }

    private void KitchenGameMultiplayer_OnOnTryToJoinGame(object sender, EventArgs e)
    {
        Show();

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
        KitchenGameMultiplayer.Instance.OnTryToJoinGame -= KitchenGameMultiplayer_OnOnTryToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnOnFailedToJoinGame;
    }
}
