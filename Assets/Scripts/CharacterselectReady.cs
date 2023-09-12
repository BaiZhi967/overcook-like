using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterselectReady : NetworkBehaviour
{
    public static CharacterselectReady Instance { get; private set; }
    
    private Dictionary<ulong, bool> playerReadyDictionary;

    public event EventHandler OnReadyChanged;

    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        bool allClientReady = true;
        foreach (var clientsId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientsId) || !playerReadyDictionary[clientsId])
            {
                allClientReady = false;
                break;
            }
        }

        if (allClientReady)
        {
           Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }


    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong cliendId)
    {
        playerReadyDictionary[cliendId] = true;
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId)&&
               playerReadyDictionary[clientId];
    }

    
}
