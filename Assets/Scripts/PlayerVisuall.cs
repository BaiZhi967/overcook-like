using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuall : MonoBehaviour
{
    [SerializeField] private MeshRenderer headMesh;
    [SerializeField] private MeshRenderer bodyMesh;

    private Material material;

    private void Awake()
    {
        material = new Material(headMesh.material);
        headMesh.material = material;
        bodyMesh.material = material;
    }

    private void Start()
    {
       // SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor());
    }

    public void SetPlayerColor(Color color)
    {
        material.color = color;
    }
}
