using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    private Transform targetTransform;

    public void SetTargetTransform(Transform target)
    {
        this.targetTransform = target;
    }

    private void LateUpdate()
    {
        if (targetTransform is null)
        {
            return;
        }
        
        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }
}
