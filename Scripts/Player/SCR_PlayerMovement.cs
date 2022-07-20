using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playerSpeed;

    [SerializeField] private float gravity = -9.81f;

    [SerializeField] private float jumpHeight;
    
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float groundDistance;
    [SerializeField] private LayerMask groundLayer;
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool bIsGrounded;

    private bool bCanMove = true;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        SCR_CustomEvents.Player.OnSetPlayerSpawnState?.Invoke(true);
    }

    private void Update()
    {
        if (bCanMove)
        {
            bIsGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, groundLayer);
            if (bIsGrounded && velocity.y < 0f)
            {
                velocity.y = -2f;
            }

            float xMove = Input.GetAxisRaw("Horizontal");
            float zMove = Input.GetAxisRaw("Vertical");

            Vector3 move = transform.right * xMove + transform.forward * zMove;
            move.Normalize();
            controller.Move(move * playerSpeed * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Space) && bIsGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SCR_CustomEvents.Menus.OnGetPauseState?.Invoke() == true)
            {
                SCR_CustomEvents.Menus.OnPauseGame?.Invoke(false);
            }
            else
            {
                SCR_CustomEvents.Menus.OnPauseGame?.Invoke(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            SCR_CustomEvents.Cameras.OnChangeCamPerspective?.Invoke();
        }
    }

    private void SetPlayerMovement(bool state)
    {
        bCanMove = state;
    }

    private void OnEnable()
    {
        SCR_CustomEvents.Player.OnSetPlayerMovement += SetPlayerMovement;
    }

    private void OnDisable()
    {
        SCR_CustomEvents.Player.OnSetPlayerMovement -= SetPlayerMovement;
    }
}
