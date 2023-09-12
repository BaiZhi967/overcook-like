using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelectBtm : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;

    private void Awake()
    {

        GetComponent<Button>().onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.Instance.ChangePlayerColor(colorId);
        });
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDaraNetworkListChanged += KitchenGameMultiplayer_OnPlayerDaraNetworkListChanged;
        Color color = KitchenGameMultiplayer.Instance.GetPlayerColor(colorId);
        color = new Color(color.r, color.g, color.b, 1f);
        image.color = color;
        UpdateIsSelected();
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDaraNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDaraNetworkListChanged;
    }

    private void KitchenGameMultiplayer_OnPlayerDaraNetworkListChanged(object sender, EventArgs e)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if (KitchenGameMultiplayer.Instance.GetPlayerData().colorId == colorId)
        {
            selectedGameObject.SetActive(true);
            
        }
        else
        {
            selectedGameObject.SetActive(false);
        }
    }
}
