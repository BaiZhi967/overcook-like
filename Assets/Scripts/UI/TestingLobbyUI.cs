using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingLobbyUI : MonoBehaviour
{
    [SerializeField] private Button createBtm;
    [SerializeField] private Button joinBtm;

    private void Awake()
    {
        createBtm.onClick.AddListener(CreateLobby);
        joinBtm.onClick.AddListener(JoinLobby);
    }

    private void JoinLobby()
    {
        KitchenGameMultiplayer.Instance.StartClient();
    }

    private void CreateLobby()
    {
        KitchenGameMultiplayer.Instance.StartHost();
        Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
    }
}
