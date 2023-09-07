using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMultiplayerUI : MonoBehaviour
{
    private void Start()
    {
        KitchenGameManager.Instance.OnMultiplayerGamePause += InstanceOnOnMultiplayerGamePause;
        KitchenGameManager.Instance.OnMultiplayerGameUnpause += InstanceOnOnMultiplayerGameUnpause;
        Hide();
    }

    private void InstanceOnOnMultiplayerGameUnpause(object sender, EventArgs e)
    {
        Hide();
    }

    private void InstanceOnOnMultiplayerGamePause(object sender, EventArgs e)
    {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);

    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
