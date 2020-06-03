using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class RightPlayer : MonoBehaviour
{
    public static int strikeR = 0; //no action
    [Header("Movement")]
    public float speed = 50;
    public static float pos = 0;
    public Button button0, button4, button5, button6;

    CharacterController controller;
    Vector3 movement, finalMovement;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        button4.onClick.AddListener(() => ButtonClicked(1)); //rock
        button5.onClick.AddListener(() => ButtonClicked(2)); //paper
        button6.onClick.AddListener(() => ButtonClicked(3)); //scissor
        button0.onClick.AddListener(() => ButtonClicked(0)); //nothing
    }

    void ButtonClicked(int a)
    {
        strikeR = a;
    }

    void FixedUpdate()
    {
        pos = transform.position.x;
        movement = new Vector3(-1 * speed * Time.fixedDeltaTime, 0, 0);
        //movement = transform.TransformDirection(translation * speed);
        if (strikeR != 0)
            finalMovement = Vector3.Lerp(finalMovement, movement, Time.fixedDeltaTime);
        else
            finalMovement = new Vector3(0, 0, 0);
        controller.Move(finalMovement);
    }
}
