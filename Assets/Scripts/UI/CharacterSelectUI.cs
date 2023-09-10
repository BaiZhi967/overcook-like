using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button readyBtm;

    private void Awake()
    {
        readyBtm.onClick.AddListener((() =>
        {
            CharacterselectReady.Instance.SetPlayerReady();
        }));
    }
}
