using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockChildRotation : MonoBehaviour
{
    // [SerializeField] private bool _lockXRotation;
    // [SerializeField] private bool _lockYRotation;
    // [SerializeField] private bool _lockZRotation;

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(
            transform.parent.rotation.x * -1.0f,
            transform.parent.rotation.y * -1.0f,
            transform.parent.rotation.z * -1.0f
        );
        // transform.localRotation = Quaternion.Euler(
        //     (_lockXRotation ? transform.rotation.x : 0f),
        //     (_lockYRotation ? transform.rotation.y : 0f),
        //     (_lockZRotation ? transform.rotation.z : 0f)
        // );
    }
}