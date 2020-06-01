using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class LeftPlayer : MonoBehaviour
{
    //public CursorLockMode cursorLockMode = CursorLockMode.Locked;
    //public bool cursorVisible = false;
    public static int strikeL = 0; //no action
    [Header("Movement")]
    public float speed = 4;
    public static int clickL = 9;
    public Button button9, button0, button1, button2;
    CharacterController controller;
    Vector3 movement, finalMovement;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        button0.onClick.AddListener(() => ButtonClicked(0));
        button1.onClick.AddListener(() => ButtonClicked(1));
        button2.onClick.AddListener(() => ButtonClicked(2));
        button9.onClick.AddListener(() => ButtonClicked(9));
        //Cursor.lockState = cursorLockMode;
        //Cursor.visible = cursorVisible;
    }

    void ButtonClicked(int a)
    {
       clickL = a;
    }

    void FixedUpdate()
    {
        var translation = new Vector3(0, 0, 0);
        if (clickL == 2)
        {
            strikeL = 1; //rock //Up
            translation = new Vector3(0, 0, -60);
        }
        else if (clickL == 1)
        {
            strikeL = 2; //paper //Down
            translation = new Vector3(0, 0, -60);
        }
        else if (clickL == 0)
        {
            strikeL = 3; //scissor //Thrust
            translation = new Vector3(0, 0, -60);
        }
        movement = transform.TransformDirection(translation * speed * -1);
        finalMovement = Vector3.Lerp(finalMovement, movement, Time.fixedDeltaTime);
        controller.Move(finalMovement * Time.fixedDeltaTime);
    }
}
