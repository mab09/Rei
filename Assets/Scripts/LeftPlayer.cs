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
    public Button button1, /* button0 */ button2, button3;
    public static bool llose = false;
    public static Animator anim;
    public static GameObject katanaOn, katanaOff;

    CharacterController controller;
    Vector3 movement, finalMovement;


    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        katanaOn = GameObject.Find("Players/Shogun/Root/Ribs/Right_Shoulder_Joint_01/Right_Upper_Arm_Joint_01/Right_Forearm_Joint_01/Right_Wrist_Joint_01/KatanaOn");
        katanaOff = GameObject.Find("Players/Shogun/Root/Hip/Left_Thigh_Joint_01/KatanaOff");
        controller = GetComponent<CharacterController>();
       // button0.onClick.AddListener(() => ButtonClicked(0)); //nothing
        button1.onClick.AddListener(() => ButtonClicked(1)); //rock
        button2.onClick.AddListener(() => ButtonClicked(2)); //paper
        button3.onClick.AddListener(() => ButtonClicked(3)); //scissor
        katanaOn.SetActive(false);
    }

    void ButtonClicked(int a)
    {
        if (GameLogic.ready == true)
        {
            if (GameLogic.begin == true)
            {
                strikeL = a;
                timeL = Time.deltaTime;
                anim.SetTrigger("TriggerRun");
                katanaOff.SetActive(false);
                katanaOn.SetActive(true);
            }
            else
            {
                //shogun is impatient
                llose = true;
            }
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
