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
    public float speed = 4;

    public Button button3, button0, button1, button2;
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
       strikeL = a;
    }

    void FixedUpdate()
    {
        var translation = new Vector3(0, 0, 0);
        if (strikeL != 0) translation = new Vector3(0, 0, -60);
        movement = transform.TransformDirection(translation * speed * -1);
        finalMovement = Vector3.Lerp(finalMovement, movement, Time.fixedDeltaTime);
        controller.Move(finalMovement * Time.fixedDeltaTime);
    }
}
