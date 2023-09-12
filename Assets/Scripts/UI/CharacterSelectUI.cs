using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button readyBtm;
    [SerializeField] private Button mainBtm;

    private void Awake()
    {
        readyBtm.onClick.AddListener((() =>
        {
            CharacterselectReady.Instance.SetPlayerReady();
        }));
        
        mainBtm.onClick.AddListener((() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        }));            
    }
}
