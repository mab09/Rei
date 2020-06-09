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
    public static float pos = 0, timeR;
    public Button /* button0 */ button4, button5, button6;
    public static bool rlose = false;
    public static Animator anim;
    private GameObject katanaOn, katanaOff;

    CharacterController controller;
    Vector3 movement, finalMovement;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = gameObject.GetComponent<Animator>();
        katanaOn = GameObject.Find("Players/Rei/Root/Ribs/Right_Shoulder_Joint_01/Right_Upper_Arm_Joint_01/Right_Forearm_Joint_01/Right_Wrist_Joint_01/KatanaOn");
        katanaOff = GameObject.Find("Players/Rei/Root/Hip/Left_Thigh_Joint_01/KatanaOff");
        button4.onClick.AddListener(() => ButtonClicked(1)); //rock
        button5.onClick.AddListener(() => ButtonClicked(2)); //paper
        button6.onClick.AddListener(() => ButtonClicked(3)); //scissor
       // button0.onClick.AddListener(() => ButtonClicked(0)); //nothing
        katanaOn.SetActive(false);
    }

    void ButtonClicked(int a)
    {
        if (GameLogic.ready == true)
        {
            if (GameLogic.begin == true)
            {
                strikeR = a;
                timeR = Time.deltaTime;
                anim.SetTrigger("TriggerRun");
                katanaOff.SetActive(false);
                katanaOn.SetActive(true);
            }
            else
            {
                //rei is impatient
                rlose = true;
            }
        }
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
