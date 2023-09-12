using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{

    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private PlayerVisuall playerVisuall;
    [SerializeField] private Button kickBtm;

    private void Awake()
    {
        
    }

    private void KickPlayer()
    {
        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
        if (playerData.clientId == NetworkManager.Singleton.LocalClientId)
        {
            return;
        }
        KitchenGameMultiplayer.Instance.KickPlayer(playerData.clientId);
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDaraNetworkListChanged += KitchenGameMultiplayer_OnPlayerDaraNetworkListChanged;
        CharacterselectReady.Instance.OnReadyChanged += CharacterselectReady_OnReadyChanged;
        
        UpdatePlayer(); 
    
        
        kickBtm.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        kickBtm.onClick.AddListener(KickPlayer);

    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDaraNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDaraNetworkListChanged;
    }

    private void CharacterselectReady_OnReadyChanged(object sender, EventArgs e)
    {
        UpdatePlayer(); 
    }

    private void KitchenGameMultiplayer_OnPlayerDaraNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyGameObject.SetActive(CharacterselectReady.Instance.IsPlayerReady(playerData.clientId));
            playerVisuall.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
        }
        else
        {
            Hide();
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
}
