using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SCR_PlayerLook : MonoBehaviour
{
    [SerializeField] private float lookSensitivity;
    [SerializeField] private Transform playerBody;

    private float xRotation = 0f;
    private bool bCanLookAround = true;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (bCanLookAround)
        {
            float xLook = Input.GetAxisRaw("Mouse X") * lookSensitivity * Time.deltaTime;
            float yLook = Input.GetAxisRaw("Mouse Y") * lookSensitivity * Time.deltaTime;

            xRotation -= yLook;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * xLook);
        }
    }

    private void SetLookState(bool state)
    {
        bCanLookAround = state;
    }

    private void OnEnable()
    {
        SCR_CustomEvents.Player.OnSetPlayerLookState += SetLookState;
    }

    private void OnDisable()
    {
        SCR_CustomEvents.Player.OnSetPlayerLookState -= SetLookState;
    }
}
