using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class LeftPlayer : MonoBehaviour
{
    public static int strikeL = 0; //no action
    [Header("Movement")]
    public float speed = 50;
    public static float pos = 0, timeL;
    public Button button3, button0, button1, button2;
    public static bool llose = false;

    CharacterController controller;
    Vector3 movement, finalMovement;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        button0.onClick.AddListener(() => ButtonClicked(0)); //nothing
        button1.onClick.AddListener(() => ButtonClicked(1)); //rock
        button2.onClick.AddListener(() => ButtonClicked(2)); //paper
        button3.onClick.AddListener(() => ButtonClicked(3)); //scissor
    }

    void ButtonClicked(int a)
    {
        if (GameLogic.begin == true)
        {
            strikeL = a;
            timeL = Time.deltaTime;
        }
        else
        {
            //shogun is impatient
            llose = true;
        }
            
    }

    void FixedUpdate()
    {
        pos = transform.position.x;
        movement = new Vector3(speed * Time.fixedDeltaTime, 0, 0);
        //movement = transform.TransformDirection(translation * speed * -1);
        if (strikeL != 0)
            finalMovement = Vector3.Lerp(finalMovement, movement, Time.fixedDeltaTime);
        else
            finalMovement = new Vector3(0, 0, 0);
        controller.Move(finalMovement);
    }
}
