using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class LeftPlayer : MonoBehaviour
{
    public CursorLockMode cursorLockMode = CursorLockMode.Locked;
    public bool cursorVisible = false;
    [Header("Movement")]
    public float speed = 4;

    CharacterController controller;
    Vector3 movement, finalMovement;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = cursorLockMode;
        Cursor.visible = cursorVisible;
    }

    void FixedUpdate()
    {
        var translation = new Vector3(0, 0, 0);
        if (Input.GetKeyDown(KeyCode.A))
        {
            translation = new Vector3(0, 0, -60);
        }
        movement = transform.TransformDirection(translation * speed * -1);
        finalMovement = Vector3.Lerp(finalMovement, movement, Time.fixedDeltaTime);
        controller.Move(finalMovement * Time.fixedDeltaTime);
    }
}
