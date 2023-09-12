using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnerctUI : MonoBehaviour
{
     [SerializeField] private Button playAgainBtm;

     private void Start()
     {
          NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
          Hide();
          
     }

     private void OnDestroy()
     {
          //NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
     }

     private void OnClientDisconnect(ulong clientId)
     {
          if (clientId == NetworkManager.ServerClientId)
          {
               Show();
          }
     }

     public void Show() {

          gameObject.SetActive(true);

     }

     private void Hide() {
          gameObject.SetActive(false);
     }
}
