using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class RightPlayer : MonoBehaviour
{
    //public CursorLockMode cursorLockMode = CursorLockMode.Locked;
    //public bool cursorVisible = false;
    public static int strikeR = 0; //no action
    [Header("Movement")]
    public float speed = 4;

    CharacterController controller;
    Vector3 movement, finalMovement;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
       // Cursor.lockState = cursorLockMode;
        //Cursor.visible = cursorVisible;
    }

    void FixedUpdate()
    {
        var translation = new Vector3(0, 0, 0);
        if (Input.GetKeyDown(KeyCode.J))
        {
            translation = new Vector3(0, 0, 60);
            strikeR = 1; //rock //Up
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            translation = new Vector3(0, 0, 60);
            strikeR = 2; //paper //Down
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            translation = new Vector3(0, 0, 60);
            strikeR = 3; //scissor //Thrust
        }
        movement = transform.TransformDirection(translation * speed);
        finalMovement = Vector3.Lerp(finalMovement, movement, Time.fixedDeltaTime); 
        controller.Move(finalMovement * Time.fixedDeltaTime);
    }
}
