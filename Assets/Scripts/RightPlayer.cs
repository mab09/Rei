using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class RightPlayer : MonoBehaviour
{
    /* ------------------------------------------------------------------------------------------- */

    public static int strikeR = 0; //no action
    public static float pos = 0; //player x axis position indicator
    public static bool rlose = false, rcount = true;
    public static Animator anim;

    public float speed = 50; //movement speed
    public Button button4, button5, button6; 

    private GameObject katanaOn, katanaOff;

    CharacterController controller;
    Vector3 movement, finalMovement;

    /* ------------------------------------------------------------------------------------------- */

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = gameObject.GetComponent<Animator>();

        katanaOn = GameObject.Find("Players/Rei/Root/Ribs/Right_Shoulder_Joint_01/Right_Upper_Arm_Joint_01/Right_Forearm_Joint_01/Right_Wrist_Joint_01/KatanaOn");
        katanaOff = GameObject.Find("Players/Rei/Root/Hip/Left_Thigh_Joint_01/KatanaOff");

        button4.onClick.AddListener(() => ButtonClicked(1)); //rock - taka
        button5.onClick.AddListener(() => ButtonClicked(2)); //paper - tora
        button6.onClick.AddListener(() => ButtonClicked(3)); //scissor - hebi

        katanaOn.SetActive(false);
    }

    /* ------------------------------------------------------------------------------------------- */

    void ButtonClicked(int a)
    {
        if (GameLogic.ready == true) //countdown starts
        {
            if (GameLogic.begin == true) //third heartbeat
            {
                strikeR = a;
                rcount = false;
                anim.SetTrigger("TriggerRun");
                katanaOff.SetActive(false);
                katanaOn.SetActive(true);
            }
            else
            {
                //rei is impatient
                strikeR = a;
                anim.SetTrigger("TriggerBad");
                katanaOff.SetActive(false);
                katanaOn.SetActive(true);
                rlose = true;
            }
        }
    }

    /* ------------------------------------------------------------------------------------------- */

    void FixedUpdate()
    {
        pos = transform.position.x;
        movement = new Vector3(-1 * speed * Time.fixedDeltaTime, 0, 0);

        //check player choice if player wasn't impatient

        if (strikeR != 0 && rlose == false)
        {
            finalMovement = Vector3.Lerp(finalMovement, movement, Time.fixedDeltaTime);
            if (strikeR == 1)
            {
                button4.gameObject.SetActive(true);
                button5.gameObject.SetActive(false);
                button6.gameObject.SetActive(false);
            }
            else if (strikeR == 2)
            {
                button4.gameObject.SetActive(false);
                button5.gameObject.SetActive(true);
                button6.gameObject.SetActive(false);
            }
            else if (strikeR == 3)
            {
                button4.gameObject.SetActive(false);
                button5.gameObject.SetActive(false);
                button6.gameObject.SetActive(true);

            }
        }
        else
            finalMovement = new Vector3(0, 0, 0);

        //move player with the set speed

        controller.Move(finalMovement);

        //reset

        if (GameLogic.reset == true)
        {
            button4.gameObject.SetActive(true);
            button5.gameObject.SetActive(true);
            button6.gameObject.SetActive(true);
        }
    }
}
