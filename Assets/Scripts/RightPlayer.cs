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
    public float speed = 4;
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
        var translation = new Vector3(0, 0, 0);
        if (strikeR != 0) translation = new Vector3(0, 0, 60);
        movement = transform.TransformDirection(translation * speed);
        finalMovement = Vector3.Lerp(finalMovement, movement, Time.fixedDeltaTime); 
        controller.Move(finalMovement * Time.fixedDeltaTime);
    }
}
