using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class RightPlayer : MonoBehaviour
{
    //public CursorLockMode cursorLockMode = CursorLockMode.Locked;
    //public bool cursorVisible = false;
    public static int strikeR = 0; //no action
    [Header("Movement")]
    public float speed = 4;
    public static int clickR = 9;
    public Button button9, button3, button4, button5;

    CharacterController controller;
    Vector3 movement, finalMovement;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        button3.onClick.AddListener(() => ButtonClicked(3));
        button4.onClick.AddListener(() => ButtonClicked(4));
        button5.onClick.AddListener(() => ButtonClicked(5));
        button9.onClick.AddListener(() => ButtonClicked(9));
        // Cursor.lockState = cursorLockMode;
        //Cursor.visible = cursorVisible;
    }

    void ButtonClicked(int a)
    {
        clickR = a;
    }

    void FixedUpdate()
    {
        var translation = new Vector3(0, 0, 0);
        if (clickR == 5)
        {
            translation = new Vector3(0, 0, 60);
            strikeR = 1; //rock //Up
        }
        else if (clickR == 4)
        {
            translation = new Vector3(0, 0, 60);
            strikeR = 2; //paper //Down
        }
        else if (clickR == 3)
        {
            translation = new Vector3(0, 0, 60);
            strikeR = 3; //scissor //Thrust
        }
        movement = transform.TransformDirection(translation * speed);
        finalMovement = Vector3.Lerp(finalMovement, movement, Time.fixedDeltaTime); 
        controller.Move(finalMovement * Time.fixedDeltaTime);
    }
}
